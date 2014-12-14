using System;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        public void UpdateCellAvailability()
        {
            CanPlaceItem = false;

            if (!GameClass.HasFocus) return;

            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos);

            Render.UnsetDevice();

            CanPlaceItem = false;

            var _data = DataGroup.CurrentData.GetData<data>(GridCoord, new vec2(1, 1));

            color clr = color.TransparentBlack;
            if (_data != null)
            {
                CanPlaceItem = true;

                var here = _data[0];

                bool occupied = here.direction > 0;

                CanPlaceItem = !occupied;
            }
        }

        public void PlaceUnit(float unit_tpe)
        {
            Render.UnsetDevice();

            if (!CanPlaceItem)
            {
                Message_CanNotPlaceHere();
            }
            else
            {
                try
                {
                    vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos);
                    Create.PlaceUnit(DataGroup, GridCoord, unit_tpe, PlayerValue, TeamValue, SetPrevious: MapEditorActive);

                    CanPlaceItem = false;
                }
                catch
                {
                }
            }
        }
 
        void PlaceBuilding()
        {
            CanPlaceItem = false;

            if (!GameClass.HasFocus) return;

            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            int _w = 3, _h = 3;

            Render.UnsetDevice();

            CanPlaceItem = false;
            for (int i = 0; i < _w; i++)
            for (int j = 0; j < _h; j++)
            {
                CanPlace[i + j * _w] = false;
            }

            if (BuildingUserIsPlacing == UnitType.Barracks && PlayerNumber > 0)
            {
                var _data = DataGroup.CurrentData.GetData<building>(GridCoord, new vec2(_w, _h));
                var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                color clr = color.TransparentBlack;
                if (_data != null)
                {
                    CanPlaceItem = true;
                    for (int i = 0; i < _w; i++)
                    for (int j = 0; j < _h; j++)
                    {
                        var building_here = _data[i + j * _w];
                        var distance_to = _dist[i + j * _w];

                        var distance = Get(distance_to, PlayerNumber);

                        bool occupied = building_here.direction > 0;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = !occupied && (in_territory || MapEditorActive);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceItem = false;
                    }
                }
            }

            if (BuildingUserIsPlacing == UnitType.GoldMine || BuildingUserIsPlacing == UnitType.JadeMine)
            {
                var _data = DataGroup.CurrentUnits.GetData<unit>(GridCoord, new vec2(_w, _h));
                var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                color clr = color.TransparentBlack;
                if (_data != null)
                {
                    CanPlaceItem = true;
                    for (int i = 0; i < _w; i++)
                    for (int j = 0; j < _h; j++)
                    {
                        var unit_here = _data[i + j * _w];
                        var distance_to = _dist[i + j * _w];

                        var distance = Get(distance_to, PlayerNumber);

                        bool occupied = unit_here.type > 0;
                        bool is_valid_source = unit_here.team == Team.None && unit_here.type == BuildingUserIsPlacing;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = (is_valid_source || MapEditorActive && !occupied) && (in_territory || MapEditorActive);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceItem = false;
                    }
                }
            }

            if (Input.LeftMousePressed)
            {
                if (!CanPlaceItem)
                {
                    Message_CanNotPlaceHere();
                }
                else if (!CanAffordBuilding(BuildingUserIsPlacing, PlayerNumber))
                {
                    Message_InsufficientGold();
                }
                else try
                {
                    Networking.ToServer(new MessagePlaceBuilding(GridCoord, BuildingUserIsPlacing));
                }
                catch
                {
                }
            }
        }

        public void PlaceBuildingApply(int PlayerNum, int TeamNum, vec2 Pos, float Building)
        {
            while (true)
            {
                try
                {
                    Render.UnsetDevice();
                    Create.PlaceBuilding(DataGroup, Pos, Building, Player.Vals[PlayerNum], Team.Vals[TeamNum]);

                    SubtractGold(Params.BuildingCost(Building), PlayerNum);
                    CanPlaceItem = false;

                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        void PlaceUnits()
        {
            SelectionUpdate(SelectSize, EffectSelection: false, LineSelect: true);

            if (Input.LeftMouseDown)
            {
                SpawnUnits(PlayerValue, TeamValue, UnitUserIsPlacing, UnitPlaceStyle);
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

        public void Fireball()
        {
            
        }

        public void FireballApply(int PlayerNumber, vec2 GridCoord)
        {
            vec2 Pos = GridToWorldCood(GridCoord);
            vec2 Size = vec(30, 30) * CellSize;

            AddExplosion(Pos);

            GameClass.Data.SelectInArea(Pos, Size, false, true, Player.Vals[PlayerNumber], false);

            Kill.Apply(DataGroup.SelectField, DataGroup.Magic, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Magic);
        }

        public void RaiseSkeletons(vec2 area)
        {
            //CurUserMode = UserMode.Select;
            //SkipDeselect = true;
        }

        public void RaiseSkeletonsApply(int PlayerNumber, vec2 Pos, vec2 area)
        {
            AddSummonAreaEffect(area);

            SpawnUnits(PlayerValue, TeamValue, UnitType.Skeleton, UnitDistribution.OnCorpses);
        }

        public void SummonTerracotta(vec2 area)
        {
            //CurUserMode = UserMode.Select;
            //SkipDeselect = true;
        }

        public void SummonTerracottaApply(int PlayerNumber, vec2 Pos, vec2 area)
        {
            AddSummonAreaEffect(area);

            SpawnUnits(PlayerValue, TeamValue, UnitType.ClaySoldier, UnitDistribution.EveryOther);
        }

        public void SummonNecromancer()
        {
            CurUserMode = UserMode.Select;
            SkipDeselect = true;
            SkipSelect = true;
        }

        public void SummonNecromancerApply(int PlayerNumber, vec2 Pos)
        {
            AddSummonUnitEffect();

            PlaceUnit(UnitType.Necromancer);
        }

        public void SpawnUnits(float player, float team, float type, float distribution, bool raising = true)
        {
            if (MapEditorActive) raising = false;

            if (distribution == UnitDistribution.Single)
            {
                PlaceUnit(type);
                return;
            }

            ActionSpawn_Filter.Apply(DataGroup.SelectField, DataGroup.CurrentData, DataGroup.CurrentUnits, DataGroup.Corpses, distribution, Output: DataGroup.Temp2);
            var Filter = DataGroup.Temp2;

            ActionSpawn_Unit.Apply(Filter, DataGroup.CurrentUnits, player, team, type, raising, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            if (SimulationPaused)
            {
                ActionSpawn_Unit.Apply(Filter, DataGroup.PreviousUnits, player, team, type, raising, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);
            }

            ActionSpawn_Target.Apply(Filter, DataGroup.TargetData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TargetData);
            
            ActionSpawn_Data.Apply(Filter, DataGroup.CurrentData, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);

            if (SimulationPaused)
            {
                ActionSpawn_Data.Apply(Filter, DataGroup.PreviousData, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);
            }

            if (distribution == UnitDistribution.OnCorpses)
            {
                ActionSpawn_Corpse.Apply(Filter, DataGroup.Corpses, Output: DataGroup.Temp1);
                CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Corpses);
            }
        }

        bool SkipDeselect = false;
        bool SkipSelect = false;
        public void SelectionUpdate(vec2 Size, bool EffectSelection = true, bool LineSelect = true)
        {
            if (!GameClass.HasFocus) return;

            vec2 WorldCoord = ScreenToWorldCoord(Input.CurMousePos);
            vec2 WorldCoordPrev = ScreenToWorldCoord(Input.PrevMousePos);

            bool Deselect = Input.LeftMousePressed && !Keys.LeftShift.Down() && !Keys.RightShift.Down()
                || CurUserMode != UserMode.Select
                || Keys.Back.Down() || Keys.Escape.Down();
            bool Selecting = Input.LeftMouseDown && (CurUserMode == UserMode.Select || CurUserMode == UserMode.CastSpell || CurUserMode == UserMode.PlaceUnits);

            if (SkipDeselect)
            {
                Deselect = false;
                SkipDeselect = false;
            }

            if (SkipSelect)
            {
                Selecting = false;
                if (CurUserMode != UserMode.Select || !Input.LeftMouseDown) SkipSelect = false;
            }

            if (LineSelect)
            {
                if (Input.LeftMousePressed && Selecting && EffectSelection) Networking.ToServer(new MessageSelect(Size, Deselect, WorldCoord, WorldCoordPrev));
            }
            else
            {
                DataGroup.SelectInArea(WorldCoord, Size, Deselect, Selecting, PlayerOrNeutral, EffectSelection);
            }

            if (CurUserMode != UserMode.Select) return;

            if (Input.LeftMouseDown)
            {
                DataGroup.Building_SelectionSpread();
            }

            if (MapEditorActive)
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

            vec2 Pos = ScreenToGridCoord(Input.CurMousePos);

            vec2 Selected_BL = DataGroup.SelectedBound_BL;
            vec2 Selected_Size = DataGroup.SelectedBound_TR - DataGroup.SelectedBound_BL;
            if (Selected_Size.x < 1) Selected_Size.x = 1;
            if (Selected_Size.y < 1) Selected_Size.y = 1;

            float SquareWidth = (float)Math.Sqrt(DataGroup.SelectedUnits);
            if (SquareWidth < 2) SquareWidth = 0;
            Pos = floor(Pos);

            vec2 Destination_Size = new vec2(SquareWidth, SquareWidth) * .8f;
            vec2 Destination_BL = Pos - Destination_Size / 2;

            Destination_Size = floor(Destination_Size);
            Destination_BL = floor(Destination_BL);
            Destination_BL = max(Destination_BL, vec2.Zero);

            Networking.ToServer(new MessageAttackMove(Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size));

            AddAttackMarker();
        }

        void CastSpell(Spell spell)
        {
            CurSpell.Execute();

            vec2 Pos = ScreenToGridCoord(Input.CurMousePos);
            Networking.ToServer(new MessageCastSpell(Spells.SpellList.IndexOf(spell), Pos));
        }
    }
}
