using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            TitleScreen, MainMenu, Loading, Game,
            ToEditor, ToMap
        }

#if DEBUG
        //GameState State = GameState.ToEditor;
        //GameState State = GameState.ToMap;
        GameState State = GameState.TitleScreen;
#else
        GameState State = GameState.TitleScreen;
        //GameState State = GameState.ToMap;
#endif

        double TimeSinceLoad = 0, TimeLoading = 0;
        string ScenarioToLoad = null;

        bool MouseMovedSome = false;

        void SetScenarioToLoad(string name)
        {
            Program.WorldLoaded = false;
            ScenarioToLoad = name;
            State = GameState.Loading;
            TimeLoading = 0;
        }

        bool ShouldStartMenuMusic = true;
        void StartMenuMusicIfNeeded()
        {
            if (ShouldStartMenuMusic)
            {
                PlayMenuMusic();
            }

            ShouldStartMenuMusic = false;
            ShouldFadeOutMenuMusic = true;
            ShouldStartGameMusic = true;
        }

        bool ShouldFadeOutMenuMusic = true;
        void FadeOutMenuMusicIfNeeded()
        {
            if (ShouldFadeOutMenuMusic)
            {
                FadeOutMenuMusic();
            }

            ShouldStartMenuMusic = false;
            ShouldFadeOutMenuMusic = false;
            ShouldStartGameMusic = true;
        }

        bool ShouldStartGameMusic = true;
        void StartGameMusicIfNeeded()
        {
            if (ShouldStartGameMusic)
            {
                PlayGameMusic();
            }

            ShouldStartGameMusic = false;
            ShouldStartMenuMusic = true;
        }

        void GameLogic(GameTime gameTime)
        {
            //Send("setMode", "in-game");
            //Send("setScreen", "in-game-ui");

            switch (State)
            {
                case GameState.ToEditor:
                    NewWorldEditor();
                    State = GameState.Game;

                    Send("setMode", "in-game");
                    Send("setScreen", "editor-ui");

                    break;

                case GameState.ToMap:
                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.ScreenLoading);

                    if (Program.StartupMap == null) Program.StartupMap = "Beset.m3n";
                    SetScenarioToLoad(Program.StartupMap);

                    break;

                case GameState.TitleScreen:
                    StartMenuMusicIfNeeded();
                    AmbientSounds.EndAll();

                    // No mouse input to web browser.
                    SteamWrapper.SteamHtml.AllowMouseEvents = false;

                    Render.StandardRenderSetup();

                    DrawFullScreen(Assets.ScreenTitle);

                    if (gameTime.TotalGameTime.Seconds < .005f)
                        break;

                    if (MouseMovedSome)
                    {
                        World.DrawArrowCursor();
                    }
                    else
                    {
                        if (Input.DeltaMousPos.Length() > 40)
                        {
                            MouseMovedSome = true;
                        }
                    }

                    if (InputHelper.SomethingPressed())
                    {
                        State = GameState.MainMenu;
                        Send("setMode", "main-menu");
                        Send("setScreen", "game-menu");
                    }

                    break;

                case GameState.MainMenu:
                    StartMenuMusicIfNeeded();
                    AmbientSounds.EndAll();

                    if (!InputHelper.SomethingDown())
                    {
                        SteamWrapper.SteamHtml.AllowMouseEvents = true;
                    }

                    if (_MapLoading != MapLoading)
                    {
                        _MapLoading = MapLoading;
                        SetMapLoading();
                    }

                    if (MapLoading && NewMap != null)
                    {
                        World = NewMap;
                        MapLoading = false;

                        SetMapLoading();
                    }

                    Render.StandardRenderSetup();
                    if (DrawMapPreview && World != null && World.DataGroup != null)
                    {
                        Render.UnsetDevice();
                        try
                        {
                            World.DataGroup.UpdateGradient_ToPlayers();
                            World.DataGroup.UpdateGradient_ToBuildings();
                            World.DataGroup.UpdateGradient_ToPlayers();
                            World.DataGroup.UpdateGradient_ToBuildings();
                        }
                        catch
                        {
                        }

                        World.UpdateMinimap();

                        GridHelper.GraphicsDevice.SetRenderTarget(null);
                    }

                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.ScreenDark);
                    DrawWebView();

                    if (DrawMapPreview && World != null && World.DataGroup != null)
                    {
                        MapPreviewPos = new vec2(0.76f, 0.32f);
                        MapPreviewSize = new vec2(.4f, .4f);

                        bool UseSolidColor = MapLoading || World == BlankWorld;
                        World.DrawMinimap(MapPreviewPos, MapPreviewSize, ShowCameraBox: false, SolidColor: UseSolidColor);
                    }

                    World.DrawArrowCursor();

                    break;

                case GameState.Loading:
                    FadeOutMenuMusicIfNeeded();

                    PreGame();

                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.ScreenLoading);

                    if (ScenarioToLoad != null)
                    {
                        Send("setMode", "in-game");
                        Send("setScreen", "in-game-ui");

                        World = new World(
                            GameParams: Program.StartupGameParams,
                            RemoveComputerDragonLords: Program.RemoveComputerDragonLords);

                        TimeLoading = 0;
                        World.LoadPlayerInfo = false;
                        World.Load(Path.Combine(MapDirectory, ScenarioToLoad));

                        Program.WorldLoaded = true;
                        Networking.ToServer(new Message(MessageType.DoneLoading));

                        ScenarioToLoad = null;
                        TimeSinceLoad = 0;
                        DrawFullScreen(Assets.ScreenLoading);

                        GetNames();


                        //World.SaveCurrentStateInBuffer();
                        ////var m = new MessageGameState(World.SimStep, World.WorldBytes);
                        //var m = new Message(MessageType.DoneLoading);
                        //var s = m.Encode();
                        
                        //Networking.SendString(new SteamPlayer(SteamCore.PlayerId()), s);
                        //var t = Networking.ReceiveString();
                        //var _s = t.Item2;

                        //Console.WriteLine("!");
                    }

                    if (Program.GameStarted)
                    {
                        if (Program.Spectate)
                        {
                            State = GameState.Game;
                        }
                    }
                    else
                    {
                        TimeLoading += DeltaT;

                        if (TimeLoading > 25)
                        {
                            OnFailedToJoinGame();
                        }

                        TimeSinceLoad = 0;
                        break;
                    }

                    FadeOutLoading();

                    break;

                case GameState.Game:
                    StartGameMusicIfNeeded();

                    CalculateMouseDownOverUi();

                    DrawGame(gameTime);

                    if (Program.Spectate && ShouldDrawFading())
                    {
                        Render.StandardRenderSetup();
                        DrawFullScreen(Assets.ScreenLoading);

                        FadeOutLoading();
                    }
                    else
                    {
                        DrawWebView();

                        World.DrawUi();

                        if (TimeSinceLoad < 1.5f)
                        {
                            BlackOverlay(1f - (float)(TimeSinceLoad - 1.3f) / .2f);
                        }
                    }

                    break;
            }
        }

#if DEBUG
        const float MinLoadTime = .3f;
#else
        const float MinLoadTime = 2.7f;
#endif
        const float LoadingFadeLength = .7f;

        private bool ShouldDrawFading()
        {
            return TimeSinceLoad <= MinLoadTime + LoadingFadeLength;
        }

        private void FadeOutLoading()
        {
            if (TimeSinceLoad > MinLoadTime)
            {
                BlackOverlay((float)(TimeSinceLoad - MinLoadTime) / LoadingFadeLength);
            }

            if (TimeSinceLoad > MinLoadTime + LoadingFadeLength)
            {
                State = GameState.Game;
            }
        }

        private void NewWorldEditor(string path = null)
        {
            InTrainingLobby = false;

            if (Networking._Server != null)
            {
                Console.WriteLine("Ending previous server.");
                Networking._Server.TemporaryJoin();
                Networking.FinalSend();
            }
            
            Program.Server = true;
            Program.Client = false;
            Program.SteamNetworking = true;
            Program.SteamUsers = new ulong[] { SteamCore.PlayerId(), 0, 0, 0 };
            Program.SteamServer = SteamCore.PlayerId();
            Program.SteamSpectators = new List<ulong>();
            Program.Spectate = false;
            Networking.Start();

            World = new World();

            if (path != null)
            {
                World.Load(path, Retries:5);
            }

            World.MapEditor = true;

            Console.WriteLine("Making new map editor level.");

            Send("setMode", "in-game");
            Send("setScreen", "editor-ui");

            UpdateEditorJsData();
        }

        void DrawGame(GameTime gameTime)
        {
            if (GameClass.GameActive && GameClass.HasFocus)
            {
////#if DEBUG
//                if (Keys.S.Pressed() && Keys.LeftControl.Down())
//                {
//                    World.SaveCurrentStateInBuffer();
//                    File.WriteAllBytes("TestDump", World.WorldBytes);
//                }

//                if (Keys.L.Pressed() && Keys.LeftControl.Down())
//                {
//                    //World.LoadStateFromBuffer();
//                    World.Reload(World.SimStep, World.WorldBytes);
//                }
////#endif
//                if (Keys.D.Pressed() && Keys.LeftControl.Down())
//                {
//                    World.SynchronizeNetwork();
//                }

                if (World.MapEditorActive)
                {
                    if (Keys.S.Pressed() && InputHelper.CtrlDown() && InputHelper.ShiftDown())
                    {
                        SendCommand("save-as");
                    }
                    else if (Keys.S.Pressed() && InputHelper.CtrlDown())
                    {
                        SendCommand("save");
                    }
                    else if (Keys.L.Pressed() && InputHelper.CtrlDown())
                    {
                        SendCommand("load");
                    }
                }

                World.Update();
                UpdateJsData();
                UpdateParams();
            }

            World.Draw();
        }

        public void PlayMenuMusic()
        {
            SongWad.Wad.SuppressNextInfoDisplay = true;
            SongWad.Wad.SetPlayList(Assets.Song_MenuMusic);
            SongWad.Wad.Restart(true);
        }

        public void FadeOutMenuMusic()
        {
            SongWad.Wad.FadeOut();
        }

        public void PlayGameMusic()
        {
            SongWad.Wad.SetPlayList(Assets.SongList_Standard);
            SongWad.Wad.Restart(true);
        }

        void PreGame()
        {
            if (Program.GameStarted) return;

            Message message;
            while (Networking.Inbox.TryDequeue(out message))
            {
                if (Log.Processing) Console.WriteLine("  -Pre Game Processing {0}", message);

                if (message.Type == MessageType.Start || message.Type == MessageType.NetworkDesync)
                {
                    Program.GameStarted = true;
                    break;
                }

                if (Program.Server)
                {
                    if (message.Type == MessageType.DoneLoading)
                    {
                        message.Source.HasLoaded = true;

                        if (!Program.GameStarted && Server.Clients.Count(client => client.HasLoaded && !client.Spectator) == Program.NumPlayers)
                        {
                            Networking.ToClients(new Message(MessageType.Start));
                        }
                    }
                }
            }
        }

        public void GetNames()
        {
            if (World != null && World.PlayerInfo != null && Program.SteamUsers == null || Program.SteamUsers.Length == 0) return;

            for (int player = 0; player < 4; player++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Program.Kingdoms[i] == player + 1)
                    {
                        UInt64 user = Program.SteamUsers[i];
                        if (user == 0) continue;

                        string name = new SteamPlayer(user).Name();

                        if (name != null && name.Length > 0)
                        {
                            World.PlayerInfo[player + 1].Name = name;
                        }
                    }
                }
            }
        }

        void GameOverInitialize()
        {
            ResetLobby();
            GetNames();
            Sounds.GameOver.MaybePlay();
        }

        public void GameOver(int winning_team)
        {
            GameOverInitialize();
            Send("setScreen", "game-over", new { spectator = true, victory = false, winningTeam = winning_team, info = World.PlayerInfo });
        }

        public void Defeat(int winning_team)
        {
            GameOverInitialize();
            Send("setScreen", "game-over", new { victory = false, winningTeam = winning_team, info = World.PlayerInfo });
        }

        public void Victory(int winning_team)
        {
            GameOverInitialize();
            Send("setScreen", "game-over", new { victory = true, winningTeam = winning_team, info = World.PlayerInfo });
        }

        void GameOverLogic()
        {
            if (World == null || !World.GameOver || World.MapEditor) return;

            float dist = (World.DragonLordDeathPos - World.GameOverPos).Length();
            float PanTime = CoreMath.LerpRestrict(0, .5f, 1.35f, 2, dist);

            if (T - World.GameOverTime < PanTime)
            {
                DeltaT *= .03f;
                World.Markers.Hide = true;
            }
            else if (T - World.GameOverTime < PanTime + 1.25)
            {
                DeltaT *= .35f;

                if (!World.End_PlayedDeathGroan && T - World.GameOverTime > PanTime + 0.0)
                {
                    Sounds.DyingDragonLord.MaybePlay();
                    World.End_PlayedDeathGroan = true;
                }
            }
            else
            {
                World.Markers.Hide = false;

                if (!World.End_PlayedDeathExplosion)
                {
                    Sounds.EndOfGameDyingDragonLord.MaybePlay();
                    World.End_PlayedDeathExplosion = true;
                }
            }

            GameInputEnabled = false;

            float s = (float)Math.Min(1, (T - World.GameOverTime) / PanTime);
            World.CameraPos = s * World.DragonLordDeathPos + (1 - s) * World.GameOverPos;
            World.CameraZoom = CoreMath.LogLerpRestrict((float)World.GameOverTime, World.GameOverZoom, (float)World.GameOverTime + 1.25f, 100, (float)T);
        }
    }
}
