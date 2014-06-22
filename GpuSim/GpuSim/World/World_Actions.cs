using System;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
        void PlaceBuilding()
        {
            CanPlaceBuilding = false;

            if (!GameClass.HasFocus) return;

            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            int _w = 3, _h = 3;

            Render.UnsetDevice();

            CanPlaceBuilding = false;
            for (int i = 0; i < _w; i++)
            for (int j = 0; j < _h; j++)
            {
                CanPlace[i + j * _w] = false;
            }

            if (BuildingType == UnitType.Barracks)
            {
                var _data = DataGroup.CurrentData.GetData<building>(GridCoord, new vec2(_w, _h));
                var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                color clr = color.TransparentBlack;
                if (_data != null)
                {
                    CanPlaceBuilding = true;
                    for (int i = 0; i < _w; i++)
                    for (int j = 0; j < _h; j++)
                    {
                        var building_here = _data[i + j * _w];
                        var distance_to = _dist[i + j * _w];

                        var distance = Get(distance_to, PlayerNumber);

                        bool occupied = building_here.direction > 0;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = !occupied && (in_territory || MapEditor);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceBuilding = false;
                    }
                }
            }

            if (BuildingType == UnitType.GoldMine)
            {
                var _data = DataGroup.CurrentUnits.GetData<unit>(GridCoord, new vec2(_w, _h));
                var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                color clr = color.TransparentBlack;
                if (_data != null)
                {
                    CanPlaceBuilding = true;
                    for (int i = 0; i < _w; i++)
                    for (int j = 0; j < _h; j++)
                    {
                        var unit_here = _data[i + j * _w];
                        var distance_to = _dist[i + j * _w];

                        var distance = Get(distance_to, PlayerNumber);

                        bool is_gold_source = unit_here.team == Team.None && unit_here.type == UnitType.GoldSource;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = is_gold_source && (in_territory || MapEditor);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceBuilding = false;
                    }
                }
            }

            if (Input.LeftMousePressed)
            {
                if (!CanPlaceBuilding)
                {
                    Message_CanNotPlaceHere();
                }
                else if (!CanAffordBuilding(BuildingType, PlayerNumber))
                {
                    Message_InsufficientGold();
                }
                else try
                {
                    Create.PlaceBuilding(DataGroup, GridCoord, BuildingType, PlayerValue, TeamValue);

                    SubtractGold(Params.BuildingCost(BuildingType), PlayerNumber);
                    CanPlaceBuilding = false;
                }
                catch
                {
                }
            }
        }
        
        void DeleteUnits()
        {
            ActionDelete_Data.Apply(DataGroup.CurrentData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);
        }

        void CreateUnits()
        {
            float player = 0, team = 0;

            if (Keys.R.Down()) { player = Player.One; team = Team.One; }
            if (Keys.T.Down()) { player = Player.Two; team = Team.Two; }
            if (Keys.Y.Down()) { player = Player.Three; team = Team.Three; }
            if (Keys.U.Down()) { player = Player.Four; team = Team.Four; }

            ActionSpawn_Unit.Apply(DataGroup.CurrentData, DataGroup.CurrentUnits, DataGroup.SelectField, player, team, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);
            ActionSpawn_Target.Apply(DataGroup.CurrentData, DataGroup.TargetData, DataGroup.SelectField, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TargetData);
            ActionSpawn_Data.Apply(DataGroup.CurrentData, DataGroup.SelectField, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);
        }

        void SelectionUpdate()
        {
            if (!GameClass.HasFocus) return;

            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            vec2 WorldCordPrev = ScreenToWorldCoord(Input.PrevMousePos);

            bool Deselect = Input.LeftMousePressed && !Keys.LeftShift.Down() && !Keys.RightShift.Down()
                || CurUserMode != UserMode.Select
                || Keys.Back.Down() || Keys.Escape.Down();
            bool Selecting = Input.LeftMouseDown && CurUserMode == UserMode.Select;

            vec2 size = vec2.Ones * .2f / CameraZoom;
            DataGroup.SelectAlongLine(WorldCord, WorldCordPrev, size, Deselect, Selecting, PlayerOrNeutral);

            if (CurUserMode != UserMode.Select) return;

            if (MapEditor)
            {
                if (Keys.R.Down() || Keys.T.Down() || Keys.Y.Down() || Keys.U.Down())
                {
                    CreateUnits();
                }

                if (Keys.Delete.Down() || Keys.Back.Down())
                {
                    DeleteUnits();
                }
            }
            
            if (!SimulationPaused)
            {
                if (Input.RightMousePressed)
                {
                    AttackMove();
                }
            }
        }

        void AttackMove()
        {
            DataGroup.SelectedUnitsBounds();

            if (DataGroup.SelectedUnits == 0 && DataGroup.SelectedBarracks == 0) return;

            vec2 pos = ScreenToGridCoord(Input.CurMousePos);

            vec2 Selected_BL = DataGroup.SelectedBound_BL;
            vec2 Selected_Size = DataGroup.SelectedBound_TR - DataGroup.SelectedBound_BL;
            if (Selected_Size.x < 1) Selected_Size.x = 1;
            if (Selected_Size.y < 1) Selected_Size.y = 1;

            float SquareWidth = (float)Math.Sqrt(DataGroup.SelectedUnits);
            if (SquareWidth < 2) SquareWidth = 0;
            pos = floor(pos);

            //vec2 Destination_Size = new vec2(SquareWidth, SquareWidth) * 1.25f;
            vec2 Destination_Size = new vec2(SquareWidth, SquareWidth) * .8f;
            vec2 Destination_BL = pos - Destination_Size / 2;

            Destination_Size = floor(Destination_Size);
            Destination_BL = floor(Destination_BL);
            Destination_BL = max(Destination_BL, vec2.Zero);

            DataGroup.AttackMoveApply(pos, Selected_BL, Selected_Size, Destination_Size, Destination_BL);

            AddAttackMarker();
        }
    }
}
