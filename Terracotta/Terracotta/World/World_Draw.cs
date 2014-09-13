using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
        RectangleQuad OutsideTiles = new RectangleQuad();

        Texture2D UnitsSprite 
        {
            get
            {
                float z = 14;
                if (CameraZoom > z)
                {
                    return Assets.UnitTexture_1;
                }
                else if (CameraZoom > z / 2)
                {
                    return Assets.UnitTexture_2;
                }
                else if (CameraZoom > z / 4)
                {
                    return Assets.UnitTexture_4;
                }
                else if (CameraZoom > z / 8)
                {
                    return Assets.UnitTexture_4;
                }
                else
                {
                    return Assets.UnitTexture_4;
                }                    
            }
        }

        Texture2D BuildingsSprite
        {
            get
            {
                float z = 14;
                if (CameraZoom > z)
                {
                    return Assets.BuildingTexture_1;
                }
                else if (CameraZoom > z / 2)
                {
                    return Assets.BuildingTexture_1;
                }
                else if (CameraZoom > z / 4)
                {
                    return Assets.BuildingTexture_1;
                }
                else if (CameraZoom > z / 8)
                {
                    return Assets.BuildingTexture_1;
                }
                else
                {
                    return Assets.BuildingTexture_1;
                }
            }
        }

        Texture2D ExsplosionSprite
        {
            get
            {
                float z = 14;
                if (CameraZoom > z)
                {
                    return Assets.ExplosionTexture_1;
                }
                else if (CameraZoom > z / 2)
                {
                    return Assets.ExplosionTexture_1;
                }
                else if (CameraZoom > z / 4)
                {
                    return Assets.ExplosionTexture_1;
                }
                else if (CameraZoom > z / 8)
                {
                    return Assets.ExplosionTexture_1;
                }
                else
                {
                    return Assets.ExplosionTexture_1;
                }
            }
        }

        Texture2D TileSprite
        {
            get
            {
                //return Assets.TileSpriteSheet_1;
                float z = 14;

                if (CameraZoom > z)
                {
                    return Assets.TileSpriteSheet_1;
                }
                else if (CameraZoom > z / 2)
                {
                    return Assets.TileSpriteSheet_2;
                }
                else if (CameraZoom > z / 4)
                {
                    return Assets.TileSpriteSheet_4;
                }
                else if (CameraZoom > z / 8)
                {
                    return Assets.TileSpriteSheet_8;
                }
                else
                {
                    return Assets.TileSpriteSheet_8;
                }
            }
        }

        public void Draw()
        {
            DrawCount++;
            Render.StandardRenderSetup();

            if (NotPaused_SimulationUpdate)
                SecondsSinceLastUpdate += GameClass.ElapsedSeconds;

            UpdateAllPlayerUnitCounts();

            switch (CurUserMode)
            {
                case UserMode.PlaceBuilding:
                    if (UnselectAll)
                    {
                        SelectionUpdate();
                        UnselectAll = false;
                    }

                    PlaceBuilding();
                    break;

                case UserMode.Select:
                    // Count the selected units for the player. Must be done at least before every attack command.
                    var selected = DataGroup.DoUnitCount(PlayerOrNeutral, true);
                    DataGroup.SelectedUnits = selected.Item1;
                    DataGroup.SelectedBarracks = selected.Item2;

                    SelectionUpdate();
                    break;
            }

            // Check if we need to do a simulation update
            if (GameClass.UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates)
            {
                SecondsSinceLastUpdate -= DelayBetweenUpdates;

                SimulationUpdate();
            }

            BenchmarkTests.Run(DataGroup.CurrentData, DataGroup.PreviousData);

            DrawGrids();
            DrawMouseUi();
            DrawUiText();
        }

        private void DrawUiText()
        {
            Render.StartText();

            // Ui Text
            DrawUi_TopInfo();
            DrawUi_PlayerGrid();
            DrawUi_CursorText();

            // User Messages
            UserMessages.Update();
            UserMessages.Draw();

            Render.EndText();
        }

        private void DrawMouseUi()
        {
            CanPlaceBuilding = false;
            if (GameClass.MouseEnabled)
            {
                if (CurUserMode == UserMode.PlaceBuilding)
                {
                    DrawAvailabilityGrid();
                    DrawPotentialBuilding();
                    DrawArrowCursor();
                }

                if (CurUserMode == UserMode.Select)
                {
                    //DrawGridCell();
                    DrawCircleCursor();
                    //DrawArrowCursor();
                }
            }
        }

        private void DrawGrids()
        {
            // Draw texture to screen
            GameClass.Graphics.SetRenderTarget(null);
            GameClass.Graphics.Clear(Color.Black);

            PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            float z = 14;

            // Draw parts of the world outside the playable map
            float tiles_solid_blend = CoreMath.LogLerpRestrict(1f, 0, 5f, 1, CameraZoom);
            bool tiles_solid_blend_flag = tiles_solid_blend < 1;

            if (x_edge > 1)
            {
                DrawOutsideTiles.Using(camvec, CameraAspect, DataGroup.Tiles, TileSprite, tiles_solid_blend_flag, tiles_solid_blend);

                OutsideTiles.SetupVertices(vec(-x_edge, -1), vec(0, 1), vec(0, 0), vec(-x_edge / 2, 1));
                OutsideTiles.Draw(GameClass.Graphics);

                OutsideTiles.SetupVertices(vec(x_edge, -1), vec(0, 1), vec(0, 0), vec(x_edge / 2, 1));
                OutsideTiles.Draw(GameClass.Graphics);
            }

            // The the map tiles
            DrawTiles.Using(camvec, CameraAspect, DataGroup.Tiles, TileSprite, MapEditor && DrawGridLines, tiles_solid_blend_flag, tiles_solid_blend);
            GridHelper.DrawGrid();

            //DrawGeoInfo.Using(camvec, CameraAspect, DataGroup.Geo, Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawGeoInfo.Using(camvec, CameraAspect, DataGroup.AntiGeo, Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawDirwardInfo.Using(camvec, CameraAspect, DataGroup.Dirward[Dir.Right], Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawPolarInfo.Using(camvec, CameraAspect, DataGroup.Geo, DataGroup.GeoInfo, Assets.DebugTexture_Num); GridHelper.DrawGrid();

            // Territory and corpses
            if (CurUserMode == UserMode.PlaceBuilding && !MapEditor)
            {
                DrawTerritoryPlayer.Using(camvec, CameraAspect, DataGroup.DistanceToPlayers, PlayerValue);
                GridHelper.DrawGrid();
            }
            else
            {
                if (CameraZoom <= z / 4)
                {
                    float territory_blend = CoreMath.LerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                    DrawTerritoryColors.Using(camvec, CameraAspect, DataGroup.DistanceToPlayers, territory_blend);
                    GridHelper.DrawGrid();
                }

                if (CameraZoom >= z / 8)
                {
                    float corpse_blend = .35f * CoreMath.LerpRestrict(z / 2, 1, z / 16, 0, CameraZoom);

                    DrawCorpses.Using(camvec, CameraAspect, DataGroup.Corspes, UnitsSprite, corpse_blend);
                    GridHelper.DrawGrid();
                }
            }

            // Buildings
            DrawBuildings.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, BuildingsSprite, ExsplosionSprite, PercentSimStepComplete);
            GridHelper.DrawGrid();

            // Markers
            Markers.Update();
            Markers.Draw();

            // Units
            if (CameraZoom > z / 8)
            {
                float second = (DrawCount % 60) / 60f;

                float selection_blend = CoreMath.LogLerpRestrict(60.0f, 1, 1.25f, 0, CameraZoom);
                float selection_size = CoreMath.LogLerpRestrict(6.0f, .6f, z / 4, 0, CameraZoom);

                float solid_blend = CoreMath.LogLerpRestrict(z / 7, 0, z / 2, 1, CameraZoom);
                bool solid_blend_flag = solid_blend < 1;

                DrawUnits.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, DataGroup.CurrentUnits, DataGroup.PreviousUnits, UnitsSprite,
                    PercentSimStepComplete, second,
                    selection_blend, selection_size,
                    solid_blend_flag, solid_blend);
            }
            else
            {
                DrawUnitsZoomedOutBlur.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, DataGroup.CurrentUnits, DataGroup.PreviousUnits, UnitsSprite, PercentSimStepComplete);
            }
            GridHelper.DrawGrid();

            // Building icons
            if (CameraZoom <= z / 4)
            {
                float blend = CoreMath.LogLerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                float radius = 5.5f / CameraZoom;

                DrawBuildingsIcons.Using(camvec, CameraAspect, DataGroup.DistanceToBuildings, blend, radius);
                GridHelper.DrawGrid();
            }
        }

        private void UpdateAllPlayerUnitCounts()
        {
            // Alternate between counting units for each player, to spread out the computational load
            int i = DrawCount % 4 + 1;
            float player = Player.Get(i);
            var count = DataGroup.DoUnitCount(player, false);
            DataGroup.UnitCount[i] = count.Item1;
            DataGroup.BarracksCount[i] = count.Item2;
        }
    }
}
