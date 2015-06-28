using System;
using System.IO;
using System.Linq;

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
        GameState State = GameState.ToEditor;
        //GameState State = GameState.ToMap;
        //GameState State = GameState.TitleScreen;
#else
        GameState State = GameState.TitleScreen;
#endif

        double TimeSinceLoad = 0;
        string ScenarioToLoad = null;

        bool MouseMovedSome = false;

        void SetScenarioToLoad(string name)
        {
            Program.WorldLoaded = false;
            ScenarioToLoad = name;
            State = GameState.Loading;
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
            switch (State)
            {
                case GameState.ToEditor:
                    World = new World();

                    //World.Load("TestSave.m3n");
                    //World.Load(Path.Combine("Content", Path.Combine("Maps", "Beset.m3n")));
                    //World.Load(Path.Combine("Content", Path.Combine("Maps", "Gilgamesh.m3n")));
                    //World.Load(Path.Combine("Content", Path.Combine("Maps", "Nice.m3n")));

                    World.MapEditor = true;

                    Send("setMode", "map-editor");
                    Send("setScreen", "none");
                    State = GameState.Game;

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

                    // No mouse input to Awesomium
                    awesomium.AllowMouseEvents = false;

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
                        awesomium.AllowMouseEvents = true;
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

                        World = new World();
                        World.Load(Path.Combine("Content", Path.Combine("Maps", ScenarioToLoad)));

                        Program.WorldLoaded = true;
                        Networking.ToServer(new Message(MessageType.DoneLoading));

                        ScenarioToLoad = null;
                        TimeSinceLoad = 0;
                        DrawFullScreen(Assets.ScreenLoading);

                        GetNames();
                    }

                    if (!Program.GameStarted)
                    {
                        TimeSinceLoad = 0;
                        break;
                    }

#if DEBUG
                    const float LoadTime = .3f;
#else
                    const float LoadTime = 2.7f;
#endif
                    const float FadeLength = .7f;

                    if (TimeSinceLoad > LoadTime)
                    {
                        BlackOverlay((float)(TimeSinceLoad - LoadTime) / FadeLength);
                    }

                    if (TimeSinceLoad > LoadTime + FadeLength)
                    {
                        State = GameState.Game;
                    }

                    break;

                case GameState.Game:
                    StartGameMusicIfNeeded();

                    if (awesomium.WebViewTexture != null)
                    {
                        CalculateMouseDownOverUi();
                    }

                    DrawGame(gameTime);

                    DrawWebView();

                    World.DrawUi();

                    if (TimeSinceLoad < 1.5f)
                    {
                        BlackOverlay(1f - (float)(TimeSinceLoad - 1.3f) / .2f);
                    }

                    break;
            }
        }

        void DrawGame(GameTime gameTime)
        {
            if (GameClass.GameActive && GameClass.HasFocus)
            {
                if (World.MapEditorActive)
                {
                    if (Keys.S.Pressed() && Keys.LeftControl.Down())
                    {
                        World.Save("Choke Points.m3n");
                        //World.Save("SavedMap.m3n");
                    }

                    if (Keys.L.Pressed() && Keys.LeftControl.Down())
                    {
                        //World.Load("SavedMap.m3n");
                        //World.Load("Content/Maps/Beset.m3n");
                        World.Load("Choke Points.m3n");
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
            Message message;
            while (Networking.Inbox.TryDequeue(out message))
            {
                if (Log.Processing) Console.WriteLine("  -Processing {0}", message);

                if (message.Type == MessageType.Start)
                {
                    Program.GameStarted = true;
                }

                if (Program.Server)
                {
                    if (message.Type == MessageType.DoneLoading)
                    {
                        message.Source.HasLoaded = true;

                        if (!Program.GameStarted && Server.Clients.Count(client => client.HasLoaded) == Program.NumPlayers)
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
                UInt64 user = Program.SteamUsers[player];
                string name = new SteamPlayer(user).Name();

                if (name != null && name.Length > 0)
                {
                    World.PlayerInfo[player + 1].Name = name;
                }
            }
        }

        public void Defeat(int winning_team)
        {
            GetNames();
            Sounds.GameOver.MaybePlay();
            Send("setScreen", "game-over", new { victory = false, winningTeam = winning_team, info = World.PlayerInfo });
        }

        public void Victory(int winning_team)
        {
            GetNames();
            Sounds.GameOver.MaybePlay();
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
                Sounds.DyingDragonLord.MaybePlay();
            }
            else
            {
                World.Markers.Hide = false;
                Sounds.ExplodingDragonLord.MaybePlay();
            }

            GameInputEnabled = false;

            float s = (float)Math.Min(1, (T - World.GameOverTime) / 1.25);
            World.CameraPos = s * World.DragonLordDeathPos + (1 - s) * World.GameOverPos;
            World.CameraZoom = CoreMath.LogLerpRestrict((float)World.GameOverTime, World.GameOverZoom, (float)World.GameOverTime + 1.25f, 100, (float)T);
        }
    }
}
