using System;
using System.IO;

using Windows = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Core.Dynamic;
using AwesomiumXNA;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;

using Newtonsoft.Json;

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
        //GameState State = GameState.ToMap;
        GameState State = GameState.TitleScreen;
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

                    State = GameState.Game;

                    break;

                case GameState.ToMap:
                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.ScreenLoading);

                    SetScenarioToLoad(Program.StartupMap);

                    break;

                case GameState.TitleScreen:
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
                        //MapPreviewPos = new vec2(2.66f, 0.554f);
                        //MapPreviewSize = new vec2(.22f, .22f);
                        World.DrawMinimap(MapPreviewPos, MapPreviewSize, ShowCameraBox: false, SolidColor: MapLoading);
                    }

                    World.DrawArrowCursor();

                    break;

                case GameState.Loading:
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
                    }

                    if (!Program.GameStarted)
                    {
                        TimeSinceLoad = 0;
                        break;
                    }

#if DEBUG
                    const float LoadTime = .3f;
#else
                    const float LoadTime = 1.3f;
#endif

                    if (TimeSinceLoad > LoadTime)
                    {
                        BlackOverlay((float)(TimeSinceLoad - LoadTime) / .7f);
                    }

                    if (TimeSinceLoad > LoadTime + .7f)
                    {
                        State = GameState.Game;
                    }

                    break;

                case GameState.Game:
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
                    if (Keys.S.Pressed())
                    {
                        World.Save("TestSave.m3n");
                    }

                    if (Keys.L.Pressed())
                    {
                        World.Load("TestSave.m3n");
                    }
                }

                World.Update();
                UpdateJsData();
                UpdateParams();
            }

            World.Draw();
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

        public void Defeat(int winning_team)
        {
            Send("setScreen", "gameOver", new { victory = false, winningTeam = winning_team });
        }

        public void Victory(int winning_team)
        {
            Send("setScreen", "gameOver", new { victory = true, winningTeam = winning_team });
        }
    }
}
