using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        public static void TryTillSuccess(Action a)
        {
            while (true)
            {
                try
                {
                    a();
                    return;
                }
                catch
                { 
                    
                }
            }
        }

        public void UpdateCellAvailability(float TerritoryRange = float.MaxValue)
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos);

            try
            {
                CanPlaceItem = CellAvailable_1x1(GridCoord, TerritoryRange);
            }
            catch
            {
                CanPlaceItem = false;
            }
        }

        public bool CellAvailable_1x1(vec2 GridCoord, float TerritoryRange = float.MaxValue)
        {
            Render.UnsetDevice();

            bool Available = false;

            var _data = DataGroup.CurrentData.GetData<data>(GridCoord, new vec2(1, 1));

            color clr = color.TransparentBlack;
            if (_data != null)
            {
                Available = true;

                var here = _data[0];

                bool occupied = here.direction > 0;

                Available = !occupied;
            }

            if (Available && TerritoryRange < float.MaxValue)
            {
                var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(1, 1));
                var distance_to = _dist[0];
                var distance = GetPlayerVal(distance_to, MyPlayerNumber);
                bool in_territory = distance < TerritoryRange;

                Available = in_territory;
            }

            return Available;
        }

        public void PlaceUnit(ref bool placed, float unit_tpe, vec2 GridCoord, float PlayerValue, float TeamValue, float TerritoryRange = float.MaxValue)
        {
            placed = false;

            Render.UnsetDevice();

            if (CellAvailable_1x1(GridCoord, TerritoryRange))
            {
                try
                {
                    Create.PlaceUnit(DataGroup, GridCoord, unit_tpe, PlayerValue, TeamValue, SetPrevious: MapEditorActive);

                    placed = true;
                    CanPlaceItem = false;
                }
                catch
                {
                }                
            }
        }
 
        void Update_BuildingPlacing()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            try
            {
                CanPlaceItem = CheckBuildingAvailability(GridCoord, MyPlayerNumber, MyTeamNumber, BuildingUserIsPlacing, CanPlace);
            }
            catch
            {
                CanPlaceItem = false;
            }

            if (LeftMousePressed && MyPlayerInfo != null)
            {
                if (!MyPlayerInfo.DragonLordAlive && !MapEditorActive)
                {
                    Message_NoDragonLordConstruction();
                }
                else if (!CanPlaceItem)
                {
                    Message_CanNotPlaceHere();
                }
                else if (!MapEditorActive && !MyPlayerInfo.CanAffordBuilding(BuildingUserIsPlacing))
                {
                    Message_InsufficientGold();
                }
                else
                {
                    try
                    {
                        Networking.ToServer(new MessagePlaceBuilding(GridCoord, Int(BuildingUserIsPlacing)));
                        
                        if (!MapEditorActive)
                        {
                            EndPlaceMode();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        bool[] TempCanPlace = new bool[3 * 3];
        bool CheckBuildingAvailability(vec2 GridCoord, int PlayerNum, int TeamNum, float BuildingToPlace, bool[] CanPlace)
        {
            int _w = 3, _h = 3;

            Render.UnsetDevice();

            bool CanPlaceItem = false;
            for (int i = 0; i < _w; i++)
            for (int j = 0; j < _h; j++)
            {
                CanPlace[i + j * _w] = false;
            }

            if (BuildingToPlace == UnitType.Barracks && PlayerNum > 0)
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

                        var distance = GetPlayerVal(distance_to, PlayerNum);

                        bool occupied = building_here.direction > 0;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = !occupied && (in_territory || MapEditorActive);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceItem = false;
                    }
                }
            }

            if (BuildingToPlace == UnitType.GoldMine || BuildingToPlace == UnitType.JadeMine)
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

                        var distance = GetPlayerVal(distance_to, PlayerNum);

                        bool occupied = unit_here.type > 0;
                        bool is_valid_source = unit_here.team == Team.None && unit_here.type == BuildingToPlace;
                        bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                        bool can_place = (is_valid_source || MapEditorActive && !occupied) && (in_territory || MapEditorActive);
                        CanPlace[i + j * _w] = can_place;

                        if (!can_place) CanPlaceItem = false;
                    }
                }
            }

            return CanPlaceItem;
        }

        public void PlaceBuildingApply(int PlayerNum, int TeamNum, vec2 Pos, int Building)
        {
            bool placed = false;
            TryTillSuccess(() => PlaceBuilding(PlayerNum, TeamNum, Pos, _[Building], ref placed));

            if (!MapEditorActive && placed) PlayerInfo[PlayerNum].BuyBuilding(Building);
            CanPlaceItem = false;
        }

        public void PlaceBuilding(int PlayerNum, int TeamNum, vec2 GridCoord, float Building, ref bool placed)
        {
            placed = false;

            if (!CheckBuildingAvailability(GridCoord, PlayerNum, TeamNum, Building, TempCanPlace))
                return;

            Render.UnsetDevice();
            Create.PlaceBuilding(DataGroup, GridCoord, Building, Player.Vals[PlayerNum], Team.Vals[TeamNum]);
            placed = true;

            AddBuildBuildingEffect(GridCoord + vec(1, 1));
        }

        void Update_Painting()
        {
            SelectionUpdate(SelectSize, EffectSelection: false, LineSelect: true);

            if (LeftMouseDown)
            {
                if (UnitUserIsPlacing != UnitType.None)
                    SpawnUnits(GridMousePos, MyPlayerValue, MyTeamValue, UnitUserIsPlacing, UnitPlaceStyle);

                if (MapEditorActive && TileUserIsPlacing != TileType.None)
                    PaintTiles();
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
            float tile = TileUserIsPlacing;

            Action_PaintTiles.Apply(DataGroup.Tiles, DataGroup.SelectField, DataGroup.RandomField, tile, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Tiles);

            PostPaintUpdate();
        }

        void PostPaintUpdate()
        {
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

        void EndSpellMode()
        {
            CurUserMode = UserMode.Select;
            SkipDeselect = true;
            SkipSelect = true;
        }

        void EndPlaceMode()
        {
            CurUserMode = UserMode.Select;
            SkipDeselect = true;
            SkipSelect = true;
        }

        public bool Fireball()
        {
            EndSpellMode();

            return true;
        }

        void SetEffectArea(vec2 Pos, float Radius, int PlayerNumber)
        {
            Pos.y -= .5f;

            vec2 rounded_pos = floor(Pos + vec(.5f, .5f));

            DataGroup.SelectInArea(rounded_pos, Radius, Player.Vals[PlayerNumber]);
        }

        public bool FireballApply(int PlayerNumber, int TeamNumber, vec2 GridCoord)
        {
            AddExplosion(GridToWorldCood(GridCoord), 2 * vec(Spells.FlameRadius, Spells.FlameRadius));

            SetEffectArea(GridCoord, Spells.FlameRadius, PlayerNumber);

            Kill.Apply(DataGroup.SelectField, DataGroup.Magic, DataGroup.AntiMagic, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Magic);

            return true;
        }

        public bool RaiseSkeletons()
        {
            EndSpellMode();

            return true;
        }

        public bool RaiseSkeletonsApply(int PlayerNumber, int TeamNumber, vec2 GridCoord, float Radius)
        {
            AddSummonAreaEffect(GridToWorldCood(GridCoord), 2 * vec(Radius, Radius));

            SetEffectArea(GridCoord, Radius, PlayerNumber);

            SpawnUnits(GridCoord, Player.Vals[PlayerNumber], Team.Vals[TeamNumber], UnitType.Skeleton, UnitDistribution.OnCorpses);

            return true;
        }

        public bool SummonTerracotta()
        {
            EndSpellMode();

            return true;
        }

        public bool SummonTerracottaApply(int PlayerNumber, int TeamNumber, vec2 GridCoord, float Radius)
        {
            AddSummonAreaEffect(GridToWorldCood(GridCoord), 2 * vec(Radius, Radius));

            SetEffectArea(GridCoord, Radius, PlayerNumber);

            SpawnUnits(GridCoord, Player.Vals[PlayerNumber], Team.Vals[TeamNumber], UnitType.ClaySoldier, UnitDistribution.EveryOther);

            return true;
        }

        public bool SummonNecromancer(float TerritoryRange)
        {
            UpdateCellAvailability(TerritoryRange);

            if (!CanPlaceItem)
            {
                Message_CanNotPlaceHere();
                return false;
            }

            EndSpellMode();

            return true;
        }

        public bool SummonNecromancerApply(int PlayerNumber, int TeamNumber, vec2 GridCoord)
        {
            AddSummonUnitEffect(GridCoord);

            bool placed = false;
            TryTillSuccess(() => PlaceUnit(ref placed, UnitType.Necromancer, GridCoord, Player.Vals[PlayerNumber], Team.Vals[TeamNumber]));

            return placed;
        }

        public void SpawnUnits(vec2 grid_coord, float player, float team, float type, float distribution, bool raising = true)
        {
            if (MapEditorActive) raising = false;

            if (distribution == UnitDistribution.Single)
            {
                bool placed = false;
                PlaceUnit(ref placed, type, grid_coord, player, team);
                return;
            }

            ActionSpawn_Filter.Apply(DataGroup.SelectField, DataGroup.CurrentData, DataGroup.CurrentUnits, DataGroup.Corpses, DataGroup.AntiMagic, distribution, team, Output: DataGroup.Temp2);
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
            if (SkipNextSelectionUpdate) return;

            vec2 WorldCoord = ScreenToWorldCoord(Input.CurMousePos);
            vec2 WorldCoordPrev = ScreenToWorldCoord(Input.PrevMousePos);

            bool Deselect =
                (LineSelect && LeftMouseDown || !LineSelect && Input.LeftMouseReleased)
                && !Keys.LeftShift.Down() && !Keys.RightShift.Down()
                || CurUserMode != UserMode.Select
                || Keys.Back.Down() || Keys.Escape.Down();
            bool Selecting =
                (LineSelect && LeftMouseDown || !LineSelect && Input.LeftMouseReleased)
                && (CurUserMode == UserMode.Select || CurUserMode == UserMode.CastSpell || CurUserMode == UserMode.Painting);

            if (SkipDeselect)
            {
                Deselect = false;
                SkipDeselect = false;
            }

            if (SkipSelect)
            {
                Selecting = false;
                if (CurUserMode != UserMode.Select || !LeftMouseDown) SkipSelect = false;
            }

            if (LineSelect)
            {
                bool DoSelect = false;
                if (MapEditorActive && CurUserMode == UserMode.Painting)
                {
                    // Continuous selection
                    DoSelect = LeftMouseDown && Selecting && EffectSelection;
                }
                else
                {
                    // Discrete selection
                    DoSelect = LeftMousePressed && Selecting && EffectSelection;
                }

                if (DoSelect) Networking.ToServer(new MessageSelectAlongLine(Size, Deselect, WorldCoord, WorldCoordPrev));
            }
            else
            {
                if (Selecting && EffectSelection && BoxSelecting)
                {
                    BoxSelecting = false;
                    vec2 bl = floor(min(BoxSelectGridStart, BoxSelectGridEnd) - vec(1f, 1f));
                    vec2 tr = ceiling(max(BoxSelectGridStart, BoxSelectGridEnd));
                    
                    Networking.ToServer(new MessageSelectInBox(Deselect, bl, tr));
                }
            }

            if (MapEditorActive && !GameClass.Game.ShowChat)
            {
                if (Keys.D5.Pressed())
                {
                    FinalizeGeodesics();
                }

                if (Keys.OemOpenBrackets.Pressed())
                {
                    MakeMapSymmetric(SymmetryType.Octo, true);
                }

                if (Keys.O.Pressed())
                {
                    MakeMapSymmetric(SymmetryType.Quad, false);
                }

                if (Keys.Delete.Down() || Keys.Back.Down())
                {
                    DeleteUnits();
                }
            }

            if (CurUserMode != UserMode.Select) return;

            if (NotPaused_UnitOrders)
            {
                if (Input.RightMousePressed)
                {
                    AttackMove();
                }

                if (Deselect)
                {
                    CurSelectionFilter = SelectionFilter.All;
                }

                if (Keys.Tab.Pressed())
                {
                    // Cycle through and find the next selection filter that results in something different than the current filter.
                    // Skip filters that result in nothing selected.
                    // Keep the same filter if no other valid filter can be found.
                    float PrevSelectionFilter = CurSelectionFilter;
                    bool[] PreviousFilteredSummary = FilteredSummary(PrevSelectionFilter);
                    
                    do
                        ChangeSelectionFilter();
                    while ((PreviousFilteredSummary.SequenceEqual(FilteredSummary(CurSelectionFilter)) || !FilteredSummary(CurSelectionFilter).Contains(true)) &&
                           CurSelectionFilter != PrevSelectionFilter);
                }
            }
        }

        private void MakeMapSymmetric(float type, bool convert_dragonlords)
        {
            MakeSymmetric.Apply(DataGroup.Tiles, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Tiles);

            MakeSymmetric.Apply(DataGroup.CurrentData, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);

            MakeSymmetric.Apply(DataGroup.PreviousData, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);

            MakeUnitsSymmetric.Apply(DataGroup.CurrentUnits, type, convert_dragonlords, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            MakeUnitsSymmetric.Apply(DataGroup.PreviousUnits, type, convert_dragonlords, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);

            MakeSymmetric.Apply(DataGroup.Extra, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Extra);

            MakeTargetSymmetric.Apply(DataGroup.TargetData, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.TargetData);

            MakeSymmetric.Apply(DataGroup.Corpses, type, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.Corpses);

            FixBuildings_1.Apply(DataGroup.CurrentData, DataGroup.CurrentUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);

            FixBuildings_1.Apply(DataGroup.PreviousData, DataGroup.PreviousUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);

            FixBuildings_2.Apply(DataGroup.CurrentData, DataGroup.CurrentUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);

            FixBuildings_2.Apply(DataGroup.PreviousData, DataGroup.PreviousUnits, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);

            PostPaintUpdate();
        }

        public void FinalizeGeodesics()
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

        bool[] FilteredSummary(float filter)
        {
            return DataGroup.UnitSummary.Select((b, i) => b && SelectionFilter.FilterHasUnit(filter, _[i+1])).ToArray();
        }

        void ChangeSelectionFilter()
        {
            CurSelectionFilter++;
            if (CurSelectionFilter >= SelectionFilter.Count - eps) CurSelectionFilter = SelectionFilter.First;
        }

        void AttackMove()
        {
            DataGroup.SelectedUnitsBounds();

            if (DataGroup.SelectedUnits == 0) return;

            vec2 Pos = ScreenToGridCoord(Input.CurMousePos);

            if (MouseOverMinimap)
            {
                Pos = MinimapGridPos();
            }
            
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

            Networking.ToServer(new MessageAttackMove(Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size, CurSelectionFilter));

            AddAttackMarker();
        }

        void CastSpell(Spell spell)
        {
            if (CurSpell.Execute())
            {
                vec2 Pos = ScreenToGridCoord(Input.CurMousePos);
                Networking.ToServer(new MessageCastSpell(Spells.SpellList.IndexOf(spell), Pos));
            }
        }
    }
}
