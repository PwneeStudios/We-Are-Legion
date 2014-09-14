using System.Collections.Generic;

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

            DrawMinimapTexture();
            DrawGrids();
            
            DrawMinimap();
            DrawTopUi();

            DrawMouseUi();
            DrawUiText();
        }

        private void DrawMinimapTexture()
        {
            var hold_CameraAspect = CameraAspect;
            var hold_CameraPos = CameraPos;
            var hold_CameraZoom = CameraZoom;

            CameraPos = vec2.Zero;
            CameraZoom = 1;
            CameraAspect = 1;

            GridHelper.GraphicsDevice.SetRenderTarget(Minimap);
            DrawGrids();
            GridHelper.GraphicsDevice.SetRenderTarget(null);

            CameraAspect = hold_CameraAspect;
            CameraPos = hold_CameraPos;
            CameraZoom = hold_CameraZoom;
        }

        private void DrawBox(vec2 p1, vec2 p2, float width)
        {
            DrawLine(vec(p1.x, p1.y), vec(p2.x, p1.y), width);
            DrawLine(vec(p2.x, p1.y), vec(p2.x, p2.y), width);
            DrawLine(vec(p2.x, p2.y), vec(p1.x, p2.y), width);
            DrawLine(vec(p1.x, p2.y), vec(p1.x, p1.y), width);
        }

        private void DrawLine(vec2 p1, vec2 p2, float width)
        {
            var q = new RectangleQuad();
            vec2 thick = vec(width, width);
            q.SetupVertices(min(p1 - thick, p2 - thick), max(p1 + thick, p2 + thick), vec2.Zero, vec2.Ones);
            q.Draw(GameClass.Graphics);
        }

        private void DrawMinimap()
        {
            vec2 size = vec(.2f, .2f);
            vec2 center = vec(-CameraAspect, -1) + new vec2(size.x, size.y);
            DrawTextureSmooth.Using(vec(0, 0, 1, 1), CameraAspect, Minimap);
            RectangleQuad.Draw(GameClass.Graphics, center, size);

            vec2 cam = CameraPos * size;
            vec2 bl = center + cam - vec(CameraAspect, 1) * size / CameraZoom;
            vec2 tr = center + cam + vec(CameraAspect, 1) * size / CameraZoom;
            bl = max(bl, center - size);
            tr = min(tr, center + size);
            DrawSolid.Using(vec(0, 0, 1, 1), CameraAspect, new color(.6f, .6f, .6f, .5f));
            DrawBox(bl, tr, .001f);
        }

        class Ui
        {
            public Ui()
            {
                ActiveUi = this;
            }

            public Dictionary<string, RectangleQuad> Elements = new Dictionary<string, RectangleQuad>();
            public List<RectangleQuad> Order = new List<RectangleQuad>();

            public void Add(string name, RectangleQuad e)
            {
                e.Texture = Assets.White;
                Elements.Add(name, e);

                //if (!name.Contains("[Text]"))
                {
                    Order.Add(e);
                }
            }

            public void Draw()
            {
                foreach (var e in Order)
                {
                    DrawElement(e);
                }
            }

            // Static

            public static Ui ActiveUi;
            public static RectangleQuad e;

            public static void Element(string name)
            {
                if (Ui.ActiveUi.Elements.ContainsKey(name))
                {
                    Ui.e = Ui.ActiveUi.Elements[name];
                    return;
                }

                var e = new RectangleQuad(vec2.Zero, vec2.Zero, vec2.Zero, vec2.Ones);

                Ui.ActiveUi.Add(name, e);
                Ui.e = e;
            }

            static void DrawElement(RectangleQuad e)
            {
                DrawTextureSmooth.Using(vec(0, 0, 1, 1), GameClass.ScreenAspect, e.Texture);
                e.Draw(GameClass.Graphics);
            }
        }

        Ui TopUi;
        void MakeTopUi()
        {
            if (TopUi != null) return;

            TopUi = new Ui();

            float a = CameraAspect;

            Ui.Element("Bar");
            Ui.e.SetupPosition(vec(a - 1.65f, 0.99814814814815f), vec(a - -0.00185185185185f, 0.94814814814815f));

            Ui.Element("Tab");
            Ui.e.SetupPosition(vec(a - 1.92037037037037f, 0.99814814814815f), vec(a - 1.65f, 0.94814814814815f));

            Ui.Element("[Text] Jade");
            Ui.e.SetupPosition(vec(a - 0.2537037037037f, 0.99074074074074f), vec(a - 0.10555555555556f, 0.95925925925926f));

            Ui.Element("Jade");
            Ui.e.SetupPosition(vec(a - 0.30185185185185f, 0.99444444444444f), vec(a - 0.26481481481481f, 0.9537037037037f));

            Ui.Element("[Text] Jade mines");
            Ui.e.SetupPosition(vec(a - 0.45555555555556f, 0.99259259259259f), vec(a - 0.38518518518518f, 0.96111111111111f));

            Ui.Element("Jade mine");
            Ui.e.SetupPosition(vec(a - 0.53703703703704f, 1f), vec(a - 0.46481481481481f, 0.95f));

            Ui.Element("[Text] Gold");
            Ui.e.SetupPosition(vec(a - 0.80740740740741f, 0.98888888888889f), vec(a - 0.65925925925926f, 0.95740740740741f));

            Ui.Element("Gold");
            Ui.e.SetupPosition(vec(a - 0.85555555555556f, 0.99444444444444f), vec(a - 0.81851851851852f, 0.9537037037037f));

            Ui.Element("[Text] Gold mines");
            Ui.e.SetupPosition(vec(a - 1.00925925925926f, 0.99074074074074f), vec(a - 0.93888888888889f, 0.95925925925926f));

            Ui.Element("Gold mine");
            Ui.e.SetupPosition(vec(a - 1.09074074074074f, 0.9962962962963f), vec(a - 1.01666666666667f, 0.94814814814815f));

            Ui.Element("[Text] Units");
            Ui.e.SetupPosition(vec(a - 1.32222222222222f, 0.98888888888889f), vec(a - 1.19074074074074f, 0.95740740740741f));

            Ui.Element("Unit");
            Ui.e.SetupPosition(vec(a - 1.36851851851852f, 0.99444444444444f), vec(a - 1.32777777777778f, 0.9537037037037f));

            Ui.Element("[Text] Barrackses");
            Ui.e.SetupPosition(vec(a - 1.51481481481481f, 0.99074074074074f), vec(a - 1.4462962962963f, 0.95925925925926f));

            Ui.Element("Barracks");
            Ui.e.SetupPosition(vec(a - 1.6f, 0.99444444444444f), vec(a - 1.52777777777778f, 0.94814814814815f));


            Ui.Element("Bar");
            Ui.e.SetColor(rgba(0x6c839c, .69f));
            
            Ui.Element("Tab");
            Ui.e.SetColor(rgba(0x6c839c, .90f));

            Ui.Element("Barracks");
            SetBuildingQuad(Ui.e.Bl, Ui.e.size, UnitType.Barracks, 1, Ui.e);

            Ui.Element("Gold mine");
            SetBuildingQuad(Ui.e.Bl, Ui.e.size, UnitType.GoldMine, 1, Ui.e);

            Ui.Element("Jade mine");
            SetBuildingQuad(Ui.e.Bl, Ui.e.size, UnitType.JadeMine, 1, Ui.e);

            Ui.Element("Unit");
            SetUnitQuad(Ui.e.Bl, Ui.e.size, 1, 0, Dir.Right, Ui.e);

            Ui.Element("Gold");
            Ui.e.Texture = Assets.Gold;

            Ui.Element("Jade");
            Ui.e.Texture = Assets.Jade;
        }

        private void DrawTopUi()
        {
            MakeTopUi();
            TopUi.Draw();
        }

        vec2 ToBatchCoord(vec2 p)
        {
            return vec((p.x + CameraAspect) / (2 * CameraAspect), (1 - (p.y + 1) / 2)) * GameClass.Screen;
        }

        private void DrawUiText()
        {
            Render.StartText();

            // Top Ui
            string s;
            float scale = .942f;

            Ui.Element("[Text] Barrackses");
            s = string.Format("{0:#,##0}", DataGroup.BarracksCount[1]);
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale, rgb(0xff1010));

            Ui.Element("[Text] Units");
            s = string.Format("{0:#,##0}", DataGroup.UnitCount[1]);
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale);

            Ui.Element("[Text] Gold mines");
            s = string.Format("{0:#,##0}", PlayerInfo[1].GoldMines);
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale);

            Ui.Element("[Text] Gold");
            s = string.Format("{0:#,##0}", PlayerInfo[1].GoldMines);
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale);

            Ui.Element("[Text] Jade mines");
            s = "0";
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale);

            Ui.Element("[Text] Jade");
            s = "0";
            Render.DrawText(s, ToBatchCoord(Ui.e.Bl), scale);

            //// Ui Text
            //DrawUi_TopInfo();
            //DrawUi_PlayerGrid();
            //DrawUi_CursorText();

            //// User Messages
            //UserMessages.Update();
            //UserMessages.Draw();

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
            //GameClass.Graphics.SetRenderTarget(null);
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
