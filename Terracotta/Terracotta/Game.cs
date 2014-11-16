using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public class GameClass : Game
    {
        public static GameClass Game;
        public static GameTime Time;
        public static double ElapsedSeconds { get { return Time.ElapsedGameTime.TotalSeconds; } }

        public const bool UnlimitedSpeed = false;
        public const bool MouseEnabled = true;

        public static GraphicsDeviceManager GraphicsManager { get { return Game.graphics; } }
        public static GraphicsDevice Graphics { get { return Game.GraphicsDevice; } }
        public static ContentManager ContentManager { get { return Game.Content; } }
        public static vec2 Screen { get { return new vec2(GraphicsManager.PreferredBackBufferWidth, GraphicsManager.PreferredBackBufferHeight); } }
        public static float ScreenAspect { get { return Screen.x / Screen.y; } }
        public static bool HasFocus { get { return Game.IsActive; } }

        GraphicsDeviceManager graphics;

        public static World World;

        public GameClass()
        {
            Game = this;

            graphics = new GraphicsDeviceManager(this);

            Window.Title = "Terracotta";

            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //graphics.IsFullScreen = false;
            //graphics.PreferredBackBufferWidth = 1024;
            //graphics.PreferredBackBufferHeight = 1024;

            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 1080;

            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;


            graphics.SynchronizeWithVerticalRetrace = !UnlimitedSpeed;
            IsFixedTimeStep = !UnlimitedSpeed;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
#if DEBUG
            SetupHotswap();
#endif

            FragSharp.Initialize(Content, GraphicsDevice);
            GridHelper.Initialize(GraphicsDevice);

            Assets.Initialize();
            Render.Initialize();

            Spells.Initialize();

            base.Initialize();
        }

#if DEBUG
        public string HotSwapDir = "Content\\HotSwap\\";
        private void SetupHotswap()
        {
            string ProjDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/Terracotta/Terracotta/";
            string ArtSrcDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/Terracotta/Terracotta/Terracotta/TerracottaContent/Art/";

            var cwd = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Replace('\\', '/');

            // On the dev machine copy actual art folder to hot swap, and then work from art folder directly
            if (cwd.Contains(ProjDir))
            {
                HotSwapDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/Terracotta/Terracotta/Terracotta/Terracotta/bin/x86/Debug/Content/HotSwap/";

                Directory.Delete(HotSwapDir, true);

                var source_art = Directory.EnumerateFiles(ArtSrcDir, "*", SearchOption.AllDirectories);

                foreach (var file in source_art)
                {
                    string dest = file.Replace("TerracottaContent/Art", "Terracotta/bin/x86/Debug/Content/HotSwap");
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    File.Copy(file, dest);
                }

                HotSwapDir = ArtSrcDir;
            }

            // Setup watcher
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = HotSwapDir;
            watcher.Filter = "*.png";

            // Watcher event handles
            watcher.Changed += new FileSystemEventHandler(OnHotSwapChanged);
            watcher.Changed += new FileSystemEventHandler(OnHotSwapChanged);
            watcher.Created += new FileSystemEventHandler(OnHotSwapChanged);
            watcher.Deleted += new FileSystemEventHandler(OnHotSwapChanged);
            
            // Start watching
            watcher.EnableRaisingEvents = true;
        }

        private static void OnHotSwapChanged(object source, FileSystemEventArgs e)
        {
            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            Assets.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }
#endif

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        int DrawCount = 0;

        enum GameState { TitleScreen, Menu, Loading, Game,    ToEditor }
        //GameState State = GameState.TitleScreen;
        GameState State = GameState.ToEditor;

        double TimeSinceLoad = 0;
        string ScenarioToLoad = null;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GameClass.Time = gameTime;
            DrawCount++;
            TimeSinceLoad += gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive)
            {
                Input.Update();
            }

            if (Buttons.Back.Down())
                this.Exit();

            switch (State)
            {
                case GameState.ToEditor:
                    World = new World();
                    //World.Load("TestSave.m3n");
                    World.MapEditor = true;

                    State = GameState.Game;

                    break;

                case GameState.TitleScreen:
                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.DemoScreen1);

                    if (gameTime.TotalGameTime.Seconds > .4f)
                        if (Input.CurKeyboard.GetPressedKeys().Length > 0 && Input.PrevKeyboard.GetPressedKeys().Length == 0 ||
                            Keys.Enter.Pressed() || Keys.Space.Pressed() || Keys.Escape.Pressed())
                        {
                            State = GameState.Menu;
                        }

                    break;

                case GameState.Menu:
                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.DemoScreen2);

                    if (Keys.D1.Down() || Keys.NumPad1.Down())
                    {
                        World.StaticMaxZoomOut = 7.5f;
                        State = GameState.Loading;
                        ScenarioToLoad = "Beset.m3n";
                        DrawFullScreen(Assets.DemoScreen3);
                    }
                    else if (Keys.D2.Down() || Keys.NumPad2.Down())
                    {
                        World.StaticMaxZoomOut = 5.61781263f;
                        State = GameState.Loading;
                        ScenarioToLoad = "Gilgamesh.m3n";
                        DrawFullScreen(Assets.DemoScreen3);
                    }
                    else if (Keys.D3.Down() || Keys.NumPad3.Down())
                    {
                        World.StaticMaxZoomOut = 1f;
                        State = GameState.Loading;
                        ScenarioToLoad = "Nice.m3n";
                        DrawFullScreen(Assets.DemoScreen3);
                    }
                    else if (Keys.Back.Down())
                    {
                        State = GameState.TitleScreen;
                    }
                    else if (Input.CurKeyboard.GetPressedKeys().Length > 0 && Input.PrevKeyboard.GetPressedKeys().Length == 0 ||
                        Keys.Enter.Pressed() || Keys.Space.Pressed() || Keys.Escape.Pressed())
                    {
                        if (World != null)
                        {
                            State = GameState.Game;
                        }
                    }

                    break;

                case GameState.Loading:
                    Render.StandardRenderSetup();
                    DrawFullScreen(Assets.DemoScreen3);

                    if (ScenarioToLoad != null)
                    {
                        World = new World();
                        World.Load(Path.Combine("Content", Path.Combine("Maps", ScenarioToLoad)));
                        ScenarioToLoad = null;
                        TimeSinceLoad = 0;
                        DrawFullScreen(Assets.DemoScreen3);
                    }

                    if (TimeSinceLoad > .3f)
                    {
                        BlackOverlay((float)(TimeSinceLoad - .3f) / .7f);
                    }

                    if (TimeSinceLoad > 1f)
                    {
                        State = GameState.Game;
                    }

                    break;

                case GameState.Game:
                    if (Keys.Escape.Pressed())
                    {
                        State = GameState.Menu;
                    }

                    DrawGame(gameTime);

                    if (TimeSinceLoad < 1.5f)
                    {
                        BlackOverlay(1f - (float)(TimeSinceLoad - 1.3f) / .2f);
                    }

                    break;
            }

            base.Draw(gameTime);
        }

        private void BlackOverlay(float s)
        {
            float a = 1920f / 1080f;
            var q = new RectangleQuad();

            q.SetupVertices(new vec2(-a, -1), new vec2(a, 1), vec2.Zero, vec2.Ones);
            q.SetColor(new color(1f, 1f, 1f, 1f));
            DrawSolid.Using(new vec4(0, 0, 1, 1), ScreenAspect, new color(0f, 0f, 0f, s));
            q.Draw(GameClass.Graphics);
            Render.UnsetDevice();
        }

        static void DrawFullScreen(Texture2D texture)
        {
            float a = 1920f / 1080f;
            var q = new RectangleQuad();

            q.SetupVertices(new vec2(-a, -1), new vec2(a, 1), vec2.Zero, vec2.Ones);
            q.SetColor(new color(1f, 1f, 1f, 1f));
            DrawTextureSmooth.Using(new vec4(0, 0, 1, 1), ScreenAspect, texture);
            q.Draw(GameClass.Graphics);
        }

        void DrawGame(GameTime gameTime)
        {
            if (IsActive)
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
            }

            World.Draw();
        }
    }
}
