using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
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

        public Dictionary<int, int> Hashes = new Dictionary<int, int>();
        public Dictionary<int, string> StringHashes = new Dictionary<int, string>();

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

        public void ProcessInbox()
        {
            Message message;
            while (Networking.Inbox.TryDequeue(out message))
            {
                if (Log.Processing) Console.WriteLine("  -In Game Processing {0}", message);

                if (Program.Server)
                {
                    if (message.Type == MessageType.PlayerAction)
                    {
                        var ack = new MessagePlayerActionAck(AckSimStep, message);

                        var chat = message.Innermost as MessageChat;
                        if (null != chat)
                        {
                            if (chat.Global)
                            {
                                Networking.ToClients(ack);
                            }
                            else
                            {
                                var chat_team = message.Source.Team;
                                if (chat_team < 0) continue;

                                foreach (var client in Server.Clients)
                                {
                                    if (client.Team == chat_team)
                                    {
                                        Networking.ToClient(client, ack);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (message.Source.Index > 4)
                            {
                                Console.WriteLine("Warning! Action message from spectator, possibly malicious.");
                                continue;
                            }

                            Networking.ToClients(ack);
                        }
                    }

                    if (message.Type == MessageType.LeaveGame)
                    {
                        message.Source.HasLeft = true;
                    }

                    if (message.Type == MessageType.RequestPause)
                    {
                        message.Source.RequestingPause = true;
                    }

                    if (message.Type == MessageType.RequestUnpause)
                    {
                        message.Source.RequestingPause = false;

                        // Cancel everyone's pause request.
                        foreach (var client in Server.Clients)
                        {
                            client.RequestingPause = false;
                        }
                    }
                }

                // Messages from players
                if (message != null)
                {
                    if (message.Type == MessageType.StartingStep ||
                        message.Type == MessageType.Hash ||
                        message.Type == MessageType.StringHash)
                    {
                        message.Innermost.Do();
                    }
                }

                // Messages from the server
                if (message.Source.IsServer)
                {
                    if (message.Type == MessageType.Bookend)
                    {
                        message.Innermost.Do();
                    }

                    if (message.Type == MessageType.PlayerActionAck)
                    {
                        message.Inner.Do();
                    }

                    if (message.Type == MessageType.Pause)
                    {
                        if (!ServerPaused)
                        {
                            ServerPaused = true;
                            GameClass.Game.Send("setScreen", "game-paused", new { canUnpause = true });
                        }
                    }

                    if (message.Type == MessageType.Unpause)
                    {
                        if (ServerPaused)
                        {
                            ServerPaused = false;
                            GameClass.Game.Send("back");
                        }
                    }

                    if (message.Type == MessageType.ServerLeft)
                    {
                        if (!Program.Server && !GameOver)
                        {
                            GameClass.Game.Send("setScreen", "disconnected", new { message = "The server has quit. Tell them they suck." });
                        }
                    }

                    if (message.Type == MessageType.NetworkDesync)
                    {
                        DesyncPause = true;
                        if (!Program.Server)
                        {
                            // The server has already put up its desync UI,
                            // so only do this for clients.
                            GameClass.Game.Send("setScreen", "desync");
                        }
                    }

                    if (message.Type == MessageType.GameState)
                    {
                        message.Innermost.Do();
                    }
                }
            }
        }

        public void SynchronizeNetwork()
        {
            if (GameOver) return;

            GameClass.Game.Send("setScreen", "desync");

            SaveCurrentStateInBuffer();

            Networking.ToClients(new Message(MessageType.NetworkDesync));
            Networking.ToClients(new MessageGameState(SimStep, WorldBytes));
        }

        void CheckIfShouldPause()
        {
            if (!Program.Server) return;

            bool ShouldPause = false;
            foreach (var client in Server.Clients)
            {
                if (!client.HasLeft && client.RequestingPause)
                    ShouldPause = true;
            }

            if (ShouldPause && !ServerPaused)
            {
                Networking.ToClients(new Message(MessageType.Pause));
            }
            else if (!ShouldPause && ServerPaused)
            {
                Networking.ToClients(new Message(MessageType.Unpause));
            }
        }

        bool ShowingWaiting = false;
        void CheckIfShouldShowWaiting()
        {
            if (WaitingTime < WaitTimeBeforeShowWaiting)
            {
                if (ShowingWaiting)
                {
                    GameClass.Game.Send("back");
                    ShowingWaiting = false;
                    GameClass.Game.ToggleChatViaFlag(Toggle.Off);
                }
            }
            else
            {
                if (!ShowingWaiting && GameClass.Game.GameInputEnabled && !GameOver)
                {
                    ShowingWaiting = true;
                    GameClass.Game.Send("setScreen", "waiting", new { canLeave = WaitingTime > 8 });
                }
            }
        }

        /// <summary>
        /// How long we have been waiting for the server to respond.
        /// </summary>
        double WaitingTime = 0;

        const double WaitTimeBeforeShowWaiting = 5;

        bool SentBookend = false;
        public void Draw()
        {
            ProcessInbox();

            DrawCount++;
            Render.StandardRenderSetup();

            double PreviousSecondsSinceLastUpdate = SecondsSinceLastUpdate;

            if (!DesyncPause)
            {
                CheckIfShouldPause();
                CheckIfShouldShowWaiting();
            }

            if (GameClass.GameActive && !ServerPaused && !DesyncPause)
            {
                if (NotPaused_SimulationUpdate)
                {
                    double Elapsed = GameClass.DeltaT; //GameClass.ElapsedSeconds;

                    if (SimStep + SecondsSinceLastUpdate / DelayBetweenUpdates < ServerSimStep - .25f)
                    {
                        Elapsed *= 1.15f;
                        if (Log.SpeedMods) Console.WriteLine("            -- Speed up please, Elasped = {3}  # {0}/{1} :{2}", Elapsed, SimStep, ServerSimStep, SecondsSinceLastUpdate / DelayBetweenUpdates);
                    }

                    SecondsSinceLastUpdate += Elapsed;
                    T += (float)Elapsed;
                }
                else
                {
                    DataGroup.PausedSimulationUpdate();

                    if (MapEditorActive)
                    {
                        SecondsSinceLastUpdate += DelayBetweenUpdates;
                        T += (float)DelayBetweenUpdates;
                    }
                }

                if (GameClass.HasFocus)
                switch (CurUserMode)
                {
                    case UserMode.PlaceBuilding:
                        if (UnselectAll)
                        {
                            SelectionUpdate(SelectSize);
                            UnselectAll = false;
                        }

                        Update_BuildingPlacing();
                        break;

                    case UserMode.Painting:
                        if (UnselectAll || MapEditorActive)
                        {
                            SelectionUpdate(SelectSize);
                            UnselectAll = false;
                        }

                        Update_Painting();
                        break;

                    case UserMode.Select:
                        SelectionUpdate(SelectSize, LineSelect: LineSelect);
                        break;

                    case UserMode.CastSpell:
                        if (LeftMousePressed && MyPlayerInfo != null)
                        {
                            if (!MyPlayerInfo.DragonLordAlive)
                            {
                                Message_NoDragonLordMagic();
                            }
                            else if (MyPlayerInfo.CanAffordSpell(CurSpell))
                            {
                                CastSpell(CurSpell);
                            }
                            else
                            {
                                Message_InsufficientJade();
                            }
                        }

                        break;
                }
                
                SkipNextSelectionUpdate = false;

                if (Program.Server)
                {
                    if (SecondsSinceLastUpdate / DelayBetweenUpdates > .75f && SimStep == ServerSimStep && !SentBookend)
                    {
                        if (Log.UpdateSim) Console.WriteLine("Ready for bookend. {0}/{1} : {2}", SimStep, ServerSimStep, SecondsSinceLastUpdate / DelayBetweenUpdates);
                        
                        SentBookend = true;

                        AckSimStep = ServerSimStep + 2;
                        Networking.ToClients(new MessageBookend(ServerSimStep + 1));
                    }
                }

                // Check if post-upate calculation still need to be done
                if (!PostUpdateFinished)
                {
                    PostSimulationUpdate();
                }

                // Check if we need to do a simulation update
                //if (true)
                //Console.WriteLine(ServerSimStep);
                if (GameClass.UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates || SimStep + 2 < ServerSimStep)
                {
                    if (SimStep < ServerSimStep && !(Program.Server && MinClientSimStep + 2 < ServerSimStep))
                    {
                        WaitingTime = 0;

                        if (!PostUpdateFinished)
                        {
                            PostSimulationUpdate(); // If we are behind do another post-sim update to help catchup.
                        }
                        else
                        {
                            SecondsSinceLastUpdate -= DelayBetweenUpdates;
                            if (SecondsSinceLastUpdate < 0) SecondsSinceLastUpdate = 0;

                            HashCheck();

                            DeququeActions(SimStep + 1);

                            HashCheck();

                            SimulationUpdate();

                            if (!Program.Spectate || Program.Spectate && SimStep % 15 == 0)
                            {
                                HashCheck(Send: true);
                            }
                            
                            SentBookend = false;
                            Networking.ToServer(new MessageStartingStep(SimStep));

                            if (Log.UpdateSim) Console.WriteLine("Just updated sim # {0}/{1} : {2}      min={3}", SimStep, ServerSimStep, SecondsSinceLastUpdate / DelayBetweenUpdates, MinClientSimStep);
                        }
                    }
                    else
                    {
                        WaitingTime += GameClass.ElapsedSeconds;

                        if (Log.Delays) Console.WriteLine("-Reverting from # {0}/{1} : {2}", SimStep, ServerSimStep, SecondsSinceLastUpdate / DelayBetweenUpdates);
                        SecondsSinceLastUpdate = DelayBetweenUpdates;
                        T -= (float)GameClass.ElapsedSeconds;
                        if (Log.Delays) Console.WriteLine("-Reverting to # {0}/{1} : {2}", SimStep, ServerSimStep, SecondsSinceLastUpdate / DelayBetweenUpdates);
                    }
                }
                else
                {
                    if (Program.Server)
                    {
                        if (Log.Draws) Console.WriteLine("Draw step {0},  {1}", DrawCount, SecondsSinceLastUpdate / DelayBetweenUpdates);
                    }
                }
            }

            BenchmarkTests.Run(DataGroup.CurrentData, DataGroup.PreviousData);

            if (!Program.Headless)
            {
                try
                {
                    DrawWorld();
                }
                catch (Exception e)
                { 
                    
                }
            }
            else
            {
                GridHelper.GraphicsDevice.SetRenderTarget(null);
            }
        }

        private void DrawWorld()
        {
            if (!MinimapInitialized) UpdateMinimap();
            GridHelper.GraphicsDevice.SetRenderTarget(null);

            DrawGrids();
            
            DrawMouseUi(AfterUi: false);
            Markers.Draw(DrawOrder.AfterMouse);

            if (GameClass.Game.MinimapEnabled)
            {
                DrawMinimap();
            }
        }

        public void DrawUi()
        {
            Render.StandardRenderSetup();

            if (GameClass.Game.UnitDisplayEnabled && MyPlayerValue != 0) DrawSelectedInfo();
            DrawMouseUi(AfterUi: true);

            if (MyPlayerNumber == 0) return;

            Render.StartText();
                DrawUiText();
                //MapEditorUiText();
            Render.EndText();
        }

        bool MinimapInitialized = false;
        public void UpdateMinimap()
        {
            MinimapInitialized = true;

            var hold_CameraAspect = CameraAspect;
            var hold_CameraPos = CameraPos;
            var hold_CameraZoom = CameraZoom;

            CameraPos = vec2.Zero;
            CameraZoom = 1;
            CameraAspect = 1;

            try
            {
                GridHelper.GraphicsDevice.SetRenderTarget(Minimap);
                DrawGrids();
            }
            catch (Exception e)
            {

            }
            finally
            {
                CameraAspect = hold_CameraAspect;
                CameraPos = hold_CameraPos;
                CameraZoom = hold_CameraZoom;   
            }
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

        public void DrawMinimap()
        {
            DrawMinimap(vec(.128f/CameraAspect,.865f), vec(.22f, .22f));
        }

        public void DrawMinimap(vec2 pos, vec2 size, bool ShowCameraBox = true, bool SolidColor = false)
        {
            //vec2 center = pos + vec(-CameraAspect, -1) + new vec2(size.x, size.y) * vec(1.1f, 1.15f);
            vec2 center = pos * vec(2 * CameraAspect, -2) - vec(CameraAspect, -1);
            MinimapQuad.SetupVertices(center - size, center + size, vec(0, 0), vec(1, 1));

            vec2 _size = size * vec(1, 254f / 245f) * 1.12f;
            vec2 _center = center + _size * vec(.03f, -.06f);
            DrawTextureSmooth.Using(vec(0, 0, 1, 1), CameraAspect, Assets.Minimap);
            RectangleQuad.Draw(GameClass.Graphics, _center, _size);

            if (SolidColor)
            {
                DrawSolid.Using(vec(0, 0, 1, 1), CameraAspect, rgb(0x222222));
                MinimapQuad.Draw(GameClass.Graphics);
            }
            else
            {
                DrawTextureSmooth.Using(vec(0, 0, 1, 1), CameraAspect, Minimap);
                MinimapQuad.Draw(GameClass.Graphics);                
            }

            if (ShowCameraBox)
            {
                vec2 cam = CameraPos * size;
                vec2 bl = center + cam - vec(CameraAspect, 1) * size / CameraZoom;
                vec2 tr = center + cam + vec(CameraAspect, 1) * size / CameraZoom;
                bl = max(bl, center - size);
                tr = min(tr, center + size);
                DrawSolid.Using(vec(0, 0, 1, 1), CameraAspect, new color(.6f, .6f, .6f, .5f));
                DrawBox(bl, tr, 2f / GameClass.Screen.x);   
            }
        }

        vec2 ToBatchCoord(vec2 p)
        {
            return vec((p.x + CameraAspect) / (2 * CameraAspect), (1 - (p.y + 1) / 2)) * GameClass.Screen;
        }

        void DrawUiText()
        {
            // User Messages
            UserMessages.Update();
            UserMessages.Draw();
        }

        void MapEditorUiText()
        {
            if (MapEditor)
            {
                if (MapEditorActive)
                {
                    string s = "Map Editor, Paused\nPlayer " + MyPlayerNumber;
                    if (CurUserMode == UserMode.Painting)
                    {
                        if (TileUserIsPlacing != TileType.None)
                            s += "\nTile: " + TileType.Name(TileUserIsPlacing);

                        if (UnitUserIsPlacing != UnitType.None)
                            s += "\nUnit: " + UnitType.Name(UnitUserIsPlacing) + ", " + UnitDistribution.Name(UnitPlaceStyle);
                    }
                    
                    Render.DrawText(s, vec(0, 200), 1);
                }
                else
                {
                    Render.DrawText("Map Editor, Playing", vec(0, 200), 1);
                }
            }
        }

        void DrawMouseUi(bool AfterUi)
        {
            CanPlaceItem = false;

            if (!GameClass.Game.GameInputEnabled || DesyncPause || (MapEditorActive && GameClass.Game.MouseOverHud))
            {
                if (AfterUi) DrawArrowCursor();
                return;
            }

            if (GameClass.MouseEnabled)
            {
                switch (CurUserMode)
                { 
                    case UserMode.PlaceBuilding:
                        if (AfterUi) break;

                        DrawAvailabilityGrid();
                        DrawPotentialBuilding();
                        DrawArrowCursor();

                        break;

                    case UserMode.Painting:
                        if (UnitPlaceStyle == UnitDistribution.Single)
                        {
                            if (AfterUi) break;

                            UpdateCellAvailability();

                            DrawGridCell();
                            DrawArrowCursor();
                        }
                        else
                        {
                            DrawCircleCursor(AfterUi);
                        }

                        break;

                    case UserMode.Select:
                        if (LineSelect)
                            DrawCircleCursor(AfterUi);
                        else
                            DrawBoxSelect(AfterUi);
                        break;

                    case UserMode.CastSpell:
                        if (AfterUi) break;

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

            if (MapEditorActive)
                PercentSimStepComplete = .9f;
            else
                PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            float z = 14;

            // Draw parts of the world outside the playable map
            float tiles_solid_blend = CoreMath.LogLerpRestrict(1f, 0, 5f, 1, CameraZoom);
            bool tiles_solid_blend_flag = tiles_solid_blend < 1;

            if (x_edge > 1 || y_edge > 1)
            {
                DrawOutsideTiles.Using(camvec, CameraAspect, DataGroup.Tiles, TileSprite, tiles_solid_blend_flag, tiles_solid_blend);

                OutsideTiles.SetupVertices(vec(-2.5f, -2f), vec(2.5f, 2), vec(0, 0), vec(-5 / 2, 2 / 1));
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
            if ((CurUserMode == UserMode.PlaceBuilding || CurUserMode == UserMode.CastSpell && CurSpell.Info.TerritoryRange < float.MaxValue)
                && !MapEditorActive)
            {
                float cutoff = _0;
                
                if (CurUserMode == UserMode.PlaceBuilding) cutoff = DrawTerritoryPlayer.TerritoryCutoff;
                else if (CurUserMode == UserMode.CastSpell) cutoff = CurSpell.Info.TerritoryRange;

                DrawTerritoryPlayer.Using(camvec, CameraAspect, DataGroup.DistanceToPlayers, MyPlayerValue, cutoff);
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

            // Dragon Lord marker, before
            DrawDragonLordMarker(After: false);

            // Units
            if (CameraZoom > z / 8)
            {
                float second = (DrawCount % 60) / 60f;
                
                float selection_blend = CoreMath.LogLerpRestrict(60.0f, 1, 1.25f, 0, CameraZoom);
                float selection_size = CoreMath.LogLerpRestrict(6.0f, .6f, z / 4, 0, CameraZoom);

                float solid_blend = CoreMath.LogLerpRestrict(z / 7, 0, z / 2, 1, CameraZoom);
                bool solid_blend_flag = solid_blend < 1;

                DrawUnits.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, DataGroup.CurrentUnits, DataGroup.PreviousUnits,
                    UnitsSprite, Assets.ShadowTexture,
                    MyPlayerValue,
                    PercentSimStepComplete, second,
                    selection_blend, selection_size,
                    solid_blend_flag, solid_blend);
            }
            else
            {
                DrawUnitsZoomedOutBlur.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, MyPlayerValue);
            }
            GridHelper.DrawGrid();

            // Buildings
            DrawBuildings.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, BuildingsSprite, ExsplosionSprite,
                MyPlayerValue,
                PercentSimStepComplete);
            GridHelper.DrawGrid();

            // Markers
            Markers.Update();
            Markers.Draw(DrawOrder.AfterTiles);

            // Antimagic
            if (CurUserMode == UserMode.CastSpell)
            {
                DrawAntiMagic.Using(camvec, CameraAspect, DataGroup.AntiMagic, MyTeamValue);
                GridHelper.DrawGrid();
            }

            // Markers
            Markers.Draw(DrawOrder.AfterUnits);
            
            // Building icons
            if (CameraZoom <= z / 4)
            {
                float blend = CoreMath.LogLerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                float radius = 5.5f / CameraZoom;

                DrawBuildingsIcons.Using(camvec, CameraAspect, DataGroup.DistanceToBuildings, DataGroup.CurrentData, DataGroup.CurrentUnits, blend, radius, MyPlayerValue);
                GridHelper.DrawGrid();
            }

            // Dragon Lord marker, after
            DrawDragonLordMarker(After: true);
        }

        void DrawDragonLordMarker(bool After=false)
        {
            if (!TrackDragonLord) return;

            for (int player = 1; player <= 4; player++)
            {
                DrawDragonLordMarker(player, After);
            }
        }

        void DrawDragonLordMarker(int player, bool After=false)
        {
            if (!TrackDragonLord) return;

            vec2 cur = CurDragonLordPos[player];
            vec2 prev = PrevDragonLordPos[player];

            if (cur.x < 1 && cur.y < 1) return;
            
            var q = new RectangleQuad();
            var p = cur * PercentSimStepComplete + prev * (1 - PercentSimStepComplete);
            p = GridToWorldCood(p);
            var s = vec(.01f, .01f) + .0001f * vec2.Ones * (float)Math.Cos(GameClass.Game.DrawCount * .08f);
            float alpha = 1;

            bool selected = DataGroup.UnitSummary[Int(UnitType.DragonLord) - 1];

            if (!After)
            {
                alpha = selected ? .11f : .05f;
                color clr = selected ? new color(1f, 1f, 1f, alpha) : new color(.8f, .8f, .8f, alpha); 

                q.SetupVertices(p - s * 3, p + s * 3, vec(0, 0), vec(1, 1));
                q.SetColor(clr);

                DrawTexture.Using(camvec, CameraAspect, Assets.DragonLord_Marker);
                q.Draw(GameClass.Game.GraphicsDevice);

                q.SetupVertices(p - s * .5f, p + s * .5f, vec(0, 0), vec(1, 1));
                q.SetColor(clr);

                DrawTexture.Using(camvec, CameraAspect, Assets.DragonLord_Marker);
                q.Draw(GameClass.Game.GraphicsDevice);
            }

            if (After)
            {
                float z = 14;
                if (CameraZoom <= z / 4f)
                {
                    s *= 7;
                    alpha = CoreMath.LerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                }
                else
                {
                    return;
                }

                q.SetupVertices(p - s, p + s, vec(0, 0), vec(1, 1));
                q.SetColor(new color(.8f, .8f, .8f, 1f * alpha));

                var texture = Assets.AoE_DragonLord[player];
                if (player == MyPlayerNumber && selected) texture = Assets.AoE_DragonLord_Selected;

                DrawTexture.Using(camvec, CameraAspect, texture);
                q.Draw(GameClass.Game.GraphicsDevice);
            }
        }

        private void UpdateAllPlayerUnitCounts()
        {
            // Alternate between counting units for each player, to spread out the computational load
            int i = SimStep % 4 + 1;
            float player = Player.Get(i);
            var count = DataGroup.DoUnitCount(player, false);
            
            DataGroup.UnitCount[i] = count.Item1;
            DataGroup.BarracksCount[i] = count.Item2;

            PlayerInfo[i].Units = count.Item1;
            PlayerInfo[i][UnitType.Barracks].Count = count.Item2;
        }
    }
}
