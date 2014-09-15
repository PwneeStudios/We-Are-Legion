using System;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
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

            if (BuildingType == UnitType.Barracks && PlayerNumber > 0)
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

                        bool occupied = unit_here.type > 0;
                        bool is_gold_source = unit_here.team == Team.None && unit_here.type == UnitType.GoldMine;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = (is_gold_source || MapEditor && !occupied) && (in_territory || MapEditor);
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

            ActionDelete_Data.Apply(DataGroup.PreviousData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);

            BuildingInfusion_Delete.Apply(DataGroup.CurrentUnits, DataGroup.CurrentData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.CurrentData, ref DataGroup.Temp1);

            DataGroup.Building_InfusionDiffusion();
        }

        void PaintTiles()
        {
            float tile = TileType.None;

            if (Keys.C.Down()) { tile = TileType.Dirt; }
            if (Keys.V.Down()) { tile = TileType.Grass; }
            if (Keys.N.Down()) { tile = TileType.Trees; }

            Action_PaintTiles.Apply(DataGroup.Tiles, DataGroup.SelectField, DataGroup.RandomField, tile, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Tiles);

            PaintTiles_UpdateData.Apply(DataGroup.Tiles, DataGroup.CurrentUnits, DataGroup.CurrentData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);

            PaintTiles_UpdateUnits.Apply(DataGroup.Tiles, DataGroup.CurrentUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            PaintTiles_UpdateTiles.Apply(DataGroup.Tiles, DataGroup.SelectField, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Tiles);

            Render.UnsetDevice();
            DataGroup.Geo.Clear();
            DataGroup.AntiGeo.Clear();
            DataGroup.MockTiles.Clear();
            DataGroup.OuterGeo.Clear();
            DataGroup.TempGeo.Clear();
            DataGroup.GeoInfo.Clear();
            foreach (var dir in Dir.Vals) DataGroup.Dirward[dir].Clear();
            UpdateGeo(false);
            UpdateGeo(true);
        }

        void SwapTempGeo(bool Anti)
        {
            if (Anti)
                CoreMath.Swap(ref DataGroup.AntiGeo, ref DataGroup.TempGeo);
            else
                CoreMath.Swap(ref DataGroup.Geo, ref DataGroup.TempGeo);
        }

        void UpdateGeo(bool Anti)
        {
            Geodesic_Outline.Apply(DataGroup.Tiles, Anti, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);

            for (int i = 0; i < 5; i++)
            {
                Geodesic_OutlineCleanup.Apply(DataGroup.Tiles, DataGroup.TempGeo, Anti, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);
            }

            Geodesic_StorePos.Apply(DataGroup.TempGeo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);

            SwapTempGeo(Anti);
        }

        void PropagateFullGeoId(bool Anti)
        {
            SwapTempGeo(Anti);

            for (int i = 0; i < 1024; i++)
            {
                Geodesic_ExtremityPropagation.Apply(DataGroup.TempGeo, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);
            }

            SwapTempGeo(Anti);
        }

        void SetReducedGeoId(bool Anti)
        {
            SwapTempGeo(Anti);

            Geodesic_SetGeoId.Apply(DataGroup.TempGeo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);

            SwapTempGeo(Anti);
        }

        void CalculatePolarDistance()
        {
            for (int i = 0; i < 1024; i++)
            {
                Geodesic_PolarDistance.Apply(DataGroup.Geo, DataGroup.GeoInfo, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.GeoInfo);
            }

            Geodesic_SetCircumference.Apply(DataGroup.Geo, DataGroup.GeoInfo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.GeoInfo);


            foreach (var dir in Dir.Vals)
            {
                Identity.Apply(DataGroup.Geo, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.ShiftedGeo);
                Identity.Apply(DataGroup.GeoInfo, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.ShiftedGeoInfo);


                for (int i = 0; i < 256; i++)
                {
                    Shift.Apply(DataGroup.ShiftedGeo, dir, Output: DataGroup.Temp1);
                    CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.ShiftedGeo);
                    Shift.Apply(DataGroup.ShiftedGeoInfo, dir, Output: DataGroup.Temp1);
                    CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.ShiftedGeoInfo);

                    Geodesic_Polarity.Apply(DataGroup.Dirward[dir], DataGroup.Geo, DataGroup.ShiftedGeo, DataGroup.GeoInfo, DataGroup.ShiftedGeoInfo, dir, Output: DataGroup.Temp1);
                    DataGroup.Dirward[dir] = CoreMath.SwapReturn(ref DataGroup.Temp1, DataGroup.Dirward[dir]);
                }

                for (int i = 0; i < 50; i++)
                {
                    Geodesic_FillMissingPolarity.Apply(DataGroup.Dirward[dir], DataGroup.Geo, Output: DataGroup.Temp1);
                    DataGroup.Dirward[dir] = CoreMath.SwapReturn(ref DataGroup.Temp1, DataGroup.Dirward[dir]);
                }

                Geodesic_ClearImportance.Apply(DataGroup.Dirward[dir], Output: DataGroup.Temp1);
                DataGroup.Dirward[dir] = CoreMath.SwapReturn(ref DataGroup.Temp1, DataGroup.Dirward[dir]);
            }
            // probably want a larger total shift than 256 eventually, but keep it less than the width of the map (maybe width minus 1, to avoid boundary condition)
            // set signal on movement data output once polarity is set, then capture signal and store polarity and geo_id
        }

        void DirwardExtend(bool Anti)
        {
            SwapTempGeo(Anti);

            for (int i = 0; i <= 100; i++)
            {
                foreach (var dir in Dir.Vals)
                {
                    Geodesic_DirwardExtend.Apply(DataGroup.Tiles, DataGroup.TempGeo, DataGroup.Dirward[dir], dir, Output: DataGroup.Temp1);
                    DataGroup.Dirward[dir] = CoreMath.SwapReturn(ref DataGroup.Temp1, DataGroup.Dirward[dir]);
                }
            }

            SwapTempGeo(Anti);
        }

        void GrowGeo(bool Anti)
        {
            for (int i = 0; i < 50; i++)
                _GrowGeo(Anti);
        }

        void _GrowGeo(bool Anti)
        {
            SwapTempGeo(Anti);

            Geodesic_ConvertToBlocking.Apply(DataGroup.Tiles, DataGroup.TempGeo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.MockTiles);

            Geodesic_Outline.Apply(DataGroup.MockTiles, Anti, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.OuterGeo);

            for (int i = 0; i < 5; i++)
            {
                Geodesic_OutlineCleanup.Apply(DataGroup.MockTiles, DataGroup.OuterGeo, Anti, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.OuterGeo);
            }

            Geodesic_Flatten.Apply(DataGroup.TempGeo, DataGroup.OuterGeo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);

            SwapTempGeo(Anti);
        }

        void MarkGeoBoundary(bool Anti)
        {
            SwapTempGeo(Anti);

            Geodesic_Boundary.Apply(DataGroup.TempGeo, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TempGeo);

            SwapTempGeo(Anti);
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

            if (Input.LeftMouseDown)
            {
                DataGroup.Building_SelectionSpread();
            }

            if (MapEditor)
            {
                if (Keys.C.Down() || Keys.V.Down() || Keys.N.Down())
                {
                    PaintTiles();
                }

                if (Keys.D5.Pressed())
                {
                    foreach (bool polarity in Vals.Bool)
                    {
                        PropagateFullGeoId(polarity);
                    }

                    CalculatePolarDistance();

                    foreach (bool polarity in Vals.Bool)
                    {
                        SetReducedGeoId(polarity);
                        GrowGeo(polarity);
                        MarkGeoBoundary(polarity);
                    }

                    DirwardExtend(false);
                }

                if (Keys.R.Down() || Keys.T.Down() || Keys.Y.Down() || Keys.U.Down())
                {
                    CreateUnits();
                }

                if (Keys.Delete.Down() || Keys.Back.Down())
                {
                    DeleteUnits();
                }
            }

            if (NotPaused_UnitOrders)
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
