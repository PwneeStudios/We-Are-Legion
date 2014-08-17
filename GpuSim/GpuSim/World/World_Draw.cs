using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
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

            // Choose units texture
            Texture2D UnitsSpriteSheet = null, BuildingsSpriteSheet = null, ExplosionSpriteSheet = null;
            float z = 14;
            if (CameraZoom > z)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_1;
            }
            else if (CameraZoom > z / 2)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_2;
            }
            else if (CameraZoom > z / 4)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_4;
            }
            else if (CameraZoom > z / 8)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_8;
            }
            else
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_16;
            }

            // Draw texture to screen
            GameClass.Graphics.SetRenderTarget(null);
            GameClass.Graphics.Clear(Color.Black);

            PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            DrawTiles.Using(camvec, CameraAspect, DataGroup.Tiles, Assets.TileSpriteSheet, MapEditor && DrawGridLines);
            GridHelper.DrawGrid();

            //DrawGeoInfo.Using(camvec, CameraAspect, DataGroup.Geo, Assets.DebugInfoTexture);
            DrawDirwardInfo.Using(camvec, CameraAspect, DataGroup.Dirward[Dir.Up], Assets.DebugInfoTexture);
            GridHelper.DrawGrid();

            //DrawGrass.Using(camvec, CameraAspect, Assets.GroundTexture);
            //Ground.Draw(GameClass.Graphics);

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
                    float corpse_blend = 1f * CoreMath.LerpRestrict(z / 2, 1, z / 16, 0, CameraZoom);

                    DrawCorpses.Using(camvec, CameraAspect, DataGroup.Corspes, UnitsSpriteSheet, corpse_blend);
                    GridHelper.DrawGrid();
                }
            }

            // Buildings
            DrawBuildings.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, BuildingsSpriteSheet, ExplosionSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();

            // Markers
            Markers.Update();
            Markers.Draw();

            // Units
            if (CameraZoom > z / 8)
                DrawUnits.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, DataGroup.CurrentUnits, DataGroup.PreviousUnits, UnitsSpriteSheet, PercentSimStepComplete);
            else
                DrawUnitsZoomedOut.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, UnitsSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();

            // Building icons
            if (CameraZoom <= z / 4)
            {
                float blend = CoreMath.LerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                DrawBuildingsIcons.Using(camvec, CameraAspect, DataGroup.DistanceToBuildings, blend);
                GridHelper.DrawGrid();
            }

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
                    DrawArrowCursor();
                }
            }
            
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
