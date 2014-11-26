using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
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

        public Dictionary<int, List<GenericMessage>> QueuedActions = new Dictionary<int, List<GenericMessage>>();

        void DeququeActions(int SimStep)
        {
            if (!QueuedActions.ContainsKey(SimStep)) return;

            var actions = QueuedActions[SimStep];
            foreach (var action in actions)
            {
                action.Innermost.Do();
            }

            QueuedActions[SimStep] = null;
        }

        void ProcessInbox()
        {
            Message message;
            while (Networking.Inbox.TryDequeue(out message))
            {
                Console.WriteLine("  -Processing {0}", message);

                if (Program.Server)
                {
                    if (message.Type == MessageType.PlayerAction)
                    {
                        Networking.ToClients(new MessagePlayerActionAck(ServerSimStep + 1, message));
                    }
                }

                if (message.Type == MessageType.Bookend)
                {
                    message.Innermost.Do();
                }

                if (message.Type == MessageType.StartingStep)
                {
                    message.Innermost.Do();
                }

                if (message.Type == MessageType.PlayerActionAck)
                {
                    message.Inner.Do();
                    //message.Innermost.Do();
                }
            }
        }

        bool SentBookend = false;
        public void Draw()
        {
            ProcessInbox();

            DrawCount++;
            Render.StandardRenderSetup();

            double PreviousSecondsSinceLastUpdate = SecondsSinceLastUpdate;

            if (GameClass.GameActive)
            {
                if (NotPaused_SimulationUpdate)
                {
                    double Elapsed = GameClass.ElapsedSeconds;

                    if (SimStep + SecondsSinceLastUpdate / DelayBetweenUpdates + .25f < ServerSimStep)
                    {
                        Elapsed *= 1.15f;
                        Console.WriteLine("            -- Elasped = {0}", Elapsed);
                    }

                    SecondsSinceLastUpdate += Elapsed;
                    T += (float)Elapsed;
                }
                else
                {
                    DataGroup.PausedSimulationUpdate();
                }

                UpdateAllPlayerUnitCounts();

                if (GameClass.HasFocus)
                switch (CurUserMode)
                {
                    case UserMode.PlaceBuilding:
                        if (UnselectAll)
                        {
                            SelectionUpdate(SelectSize);
                            UnselectAll = false;
                        }

                        PlaceBuilding();
                        break;

                    case UserMode.PlaceUnits:
                        if (UnselectAll)
                        {
                            SelectionUpdate(SelectSize);
                            UnselectAll = false;
                        }

                        PlaceUnits();
                        break;

                    case UserMode.Select:
                        // Count the selected units for the player. Must be done at least before every attack command.
                        var selected = DataGroup.DoUnitCount(PlayerOrNeutral, true);
                        DataGroup.SelectedUnits = selected.Item1;
                        DataGroup.SelectedBarracks = selected.Item2;

                        SelectionUpdate(SelectSize);
                        break;

                    case UserMode.CastSpell:
                        CurSpell.Selecting();

                        if (Input.LeftMousePressed)
                            CurSpell.Execute();

                        break;
                }

                if (Program.Server)
                {
                    if (SecondsSinceLastUpdate > .75f * DelayBetweenUpdates && SimStep == ServerSimStep && !SentBookend)
                    {
                        SentBookend = true;
                        Networking.ToClients(new MessageBookend(ServerSimStep + 1));
                    }
                }

                // Check if we need to do a simulation update
                if (GameClass.UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates || SimStep + 2 < ServerSimStep)
                {
                    if (SimStep < ServerSimStep && !(Program.Server && MinClientSimStep + 2 < ServerSimStep))
                    {
                        SecondsSinceLastUpdate -= DelayBetweenUpdates;
                        if (SecondsSinceLastUpdate < 0) SecondsSinceLastUpdate = 0;

                        DeququeActions(SimStep + 1);
                        SimulationUpdate();

                        SentBookend = false;
                        Networking.ToServer(new MessageStartingStep(SimStep));
                    }
                    else
                    {
                        SecondsSinceLastUpdate = PreviousSecondsSinceLastUpdate;
                        T -= (float)GameClass.ElapsedSeconds;
                    }
                }
            }

            BenchmarkTests.Run(DataGroup.CurrentData, DataGroup.PreviousData);

            DataGroup.UpdateGradient_ToBuildings();

            DrawMinimapTexture();
            DrawGrids();
            
            DrawMinimap();
            DrawTopUi();

            DrawMouseUi();
            DrawCursorInfo();

            Render.StartText();
                DrawUiText();
                MapEditorUiText();
            Render.EndText();
        }

        void DrawMinimapTexture()
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

        void DrawBox(vec2 p1, vec2 p2, float width)
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

        Ui TopUi, TopUi_Player1, TopUi_Player2;
        void MakeTopUi()
        {
            if (TopUi != null) return;

            float a = CameraAspect;

            TopUi = new Ui();
            Ui.Element("Upper Ui");
            Ui.e.SetupPosition(vec(a - 1.87777777777778f, 0.8962962962963f), vec(a - -0.04444444444444f, 1f));
            Ui.e.Texture = Assets.TopUi;


            TopUi_Player1 = new Ui();
            Ui.Element("[Text] Jade");
            Ui.e.SetupPosition(vec(a - 0.21111111111111f, 0.96111111111111f), vec(a - 0.06296296296296f, 0.99259259259259f));

            Ui.Element("[Text] Jade mines");
            Ui.e.SetupPosition(vec(a - 0.4037037037037f, 0.96296296296296f), vec(a - 0.33333333333333f, 0.99444444444444f));

            Ui.Element("[Text] Gold");
            Ui.e.SetupPosition(vec(a - 0.7462962962963f, 0.96111111111111f), vec(a - 0.59814814814815f, 0.99259259259259f));

            Ui.Element("[Text] Gold mines");
            Ui.e.SetupPosition(vec(a - 0.93703703703704f, 0.96296296296296f), vec(a - 0.86666666666667f, 0.99444444444444f));

            Ui.Element("[Text] Units");
            Ui.e.SetupPosition(vec(a - 1.27037037037037f, 0.96111111111111f), vec(a - 1.13888888888889f, 0.99259259259259f));

            Ui.Element("[Text] Barrackses");
            Ui.e.SetupPosition(vec(a - 1.46851851851852f, 0.96296296296296f), vec(a - 1.4f, 0.99444444444444f));


            TopUi_Player2 = new Ui();
            Ui.Element("[Text] Jade");
            Ui.e.SetupPosition(vec(a - 0.21111111111111f, 0.90740740740741f), vec(a - 0.06296296296296f, 0.93888888888889f));

            Ui.Element("[Text] Jade mines");
            Ui.e.SetupPosition(vec(a - 0.4037037037037f, 0.90925925925926f), vec(a - 0.33333333333333f, 0.94074074074074f));

            Ui.Element("[Text] Gold");
            Ui.e.SetupPosition(vec(a - 0.7462962962963f, 0.90740740740741f), vec(a - 0.59814814814815f, 0.93888888888889f));

            Ui.Element("[Text] Gold mines");
            Ui.e.SetupPosition(vec(a - 0.93703703703704f, 0.90925925925926f), vec(a - 0.86666666666667f, 0.94074074074074f));

            Ui.Element("[Text] Units");
            Ui.e.SetupPosition(vec(a - 1.27037037037037f, 0.90740740740741f), vec(a - 1.13888888888889f, 0.93888888888889f));

            Ui.Element("[Text] Barrackses");
            Ui.e.SetupPosition(vec(a - 1.46851851851852f, 0.90925925925926f), vec(a - 1.4f, 0.94074074074074f));
        }

        void DrawTopUi()
        {
            MakeTopUi();
            TopUi.Draw();
            TopUi_Player1.Draw();
            //TopUi_Player2.Draw();
        }

        vec2 ToBatchCoord(vec2 p)
        {
            return vec((p.x + CameraAspect) / (2 * CameraAspect), (1 - (p.y + 1) / 2)) * GameClass.Screen;
        }

        void DrawUiText()
        {
            // Top Ui
            Ui.ActiveUi = TopUi_Player1;
            DrawPlayerInfo(1);
            //Ui.ActiveUi = TopUi_Player2;
            //DrawPlayerInfo(2);

            //// Ui Text
            //DrawUi_TopInfo();
            //DrawUi_PlayerGrid();
            DrawUi_CursorText();

            // Spell
            DrawUi_SpellText();

            // User Messages
            UserMessages.Update();
            UserMessages.Draw();
        }

        void DrawPlayerInfo(int player)
        {
            string s;
            float scale = .5f;
            vec2 offset = vec(.0043f, .012f);

            Ui.Element("[Text] Barrackses");
            s = string.Format("{0:#,##0}", DataGroup.BarracksCount[player]);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);

            Ui.Element("[Text] Units");
            s = string.Format("{0:#,##0}", DataGroup.UnitCount[player]);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);

            Ui.Element("[Text] Gold mines");
            s = string.Format("{0:#,##0}", PlayerInfo[player].GoldMines);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);

            Ui.Element("[Text] Gold");
            s = string.Format("{0:#,##0}", PlayerInfo[player].Gold);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);

            Ui.Element("[Text] Jade mines");
            s = string.Format("{0:#,##0}", 0);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);

            Ui.Element("[Text] Jade");
            s = string.Format("{0:#,##0}", 100000);
            Render.DrawText(s, ToBatchCoord(Ui.e.Tl + offset), scale);
        }

        void DrawUi_SpellText()
        {
            if (CurUserMode == UserMode.CastSpell)
            {
                string s = CurSpell.Name;
                Render.DrawText(s, GameClass.Screen * vec(.95f, .985f), 1, rgba(0xdddddd, .95f), Alignment.Right | Alignment.Bottom);
            }
        }

        void MapEditorUiText()
        {
            if (MapEditor)
            {
                if (MapEditorActive)
                {
                    string s = "Map Editor, Paused\nPlayer " + PlayerNumber;
                    if (CurUserMode == UserMode.PlaceUnits)
                    {
                        s += "\nUnit: " + UnitType.Name(UnitUserIsPlacing) + ", " + UnitDistribution.Name(UnitPlaceStyle);
                    }
                    
                    Render.DrawText(s, vec(0, 0), 1);
                }
                else
                {
                    Render.DrawText("Map Editor, Playing", vec(0, 0), 1);
                }
            }
        }

        void DrawMouseUi()
        {
            CanPlaceItem = false;
            if (GameClass.MouseEnabled)
            {
                switch (CurUserMode)
                { 
                    case UserMode.PlaceBuilding:
                        DrawAvailabilityGrid();
                        DrawPotentialBuilding();
                        DrawArrowCursor();
                        break;

                    case UserMode.PlaceUnits:
                        if (UnitPlaceStyle == UnitDistribution.Single)
                        {
                            UpdateCellAvailability();

                            DrawGridCell();
                            DrawArrowCursor();
                        }
                        else
                        {
                            DrawCircleCursor();
                        }

                        break;

                    case UserMode.Select:
                        DrawCircleCursor();
                        break;

                    case UserMode.CastSpell:
                        CurSpell.DrawCursor();
                        break;
                }
            }
        }

        void DrawGrids()
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
            DrawTiles.Using(camvec, CameraAspect, DataGroup.Tiles, TileSprite, MapEditorActive && DrawGridLines, tiles_solid_blend_flag, tiles_solid_blend);
            GridHelper.DrawGrid();

            //DrawGeoInfo.Using(camvec, CameraAspect, DataGroup.Geo, Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawGeoInfo.Using(camvec, CameraAspect, DataGroup.AntiGeo, Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawDirwardInfo.Using(camvec, CameraAspect, DataGroup.Dirward[Dir.Right], Assets.DebugTexture_Arrows); GridHelper.DrawGrid();
            //DrawPolarInfo.Using(camvec, CameraAspect, DataGroup.Geo, DataGroup.GeoInfo, Assets.DebugTexture_Num); GridHelper.DrawGrid();

            // Territory and corpses
            if (CurUserMode == UserMode.PlaceBuilding && !MapEditorActive)
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

                    DrawCorpses.Using(camvec, CameraAspect, DataGroup.Corpses, UnitsSprite, corpse_blend);
                    GridHelper.DrawGrid();
                }
            }

            // Buildings
            DrawBuildings.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, BuildingsSprite, ExsplosionSprite, PercentSimStepComplete);
            GridHelper.DrawGrid();

            // Markers
            Markers.Update();
            Markers.Draw(DrawOrder.AfterTiles);

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

            // Markers
            Markers.Draw(DrawOrder.AfterUnits);

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
