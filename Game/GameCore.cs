using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FragSharpHelper;
using FragSharpFramework;
using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        // Steam Integration
        // Set this to true to turn on Steam Integrtaion code
        public const bool UsingSteam = true;
        public static bool SteamInitialized = false;
        public static bool SteamAvailable
        {
            get
            {
                return UsingSteam && SteamInitialized;
            }
        }

        public static bool GameActive { get { return GameClass.Game.IsActive || Program.AlwaysActive; } }

        public static GameClass Game;
        public static GameTime Time;
        public static double ElapsedSeconds { get { return Time.ElapsedGameTime.TotalSeconds; } }
        public static double DeltaT = 0, T = 0;

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
        public static DataGroup Data { get { return World.DataGroup; } }
        public static PlayerInfo[] PlayerInfo { get { return World.PlayerInfo; } }
        public static LobbyInfo LobbyInfo;

        bool AutoSaveOnTab = false;

        public GameClass()
        {
            Game = this;

            graphics = new GraphicsDeviceManager(this);

            Window.Title = "We Are Legion";

            if (Program.Width < 0 || Program.Height < 0)
            {
                LoadConfig();
            }
            else
            {
                var config = new Config();
                config.Width = Program.Width;
                config.Height = Program.Height;
                config.Fullscreen = false;

                CurrentConfig = config;
            }

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = CurrentConfig.Width;
            graphics.PreferredBackBufferHeight = CurrentConfig.Height;
            graphics.ApplyChanges();
            ApplyConfig(Activate:false);

            if (Program.MaxFps)
            {
                graphics.SynchronizeWithVerticalRetrace = false;
                IsFixedTimeStep = false;
            }
            else
            {
                graphics.SynchronizeWithVerticalRetrace = !UnlimitedSpeed;
                IsFixedTimeStep = !UnlimitedSpeed;
            }

            if (Program.Headless)
            {
                graphics.PreferredBackBufferWidth = 1;
                graphics.PreferredBackBufferHeight = 1;
            }

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            if (UsingSteam)
            {
                Console.WriteLine("Using Steam, checking if restart is needed.");

                if (SteamCore.RestartViaSteamIfNecessary(354560))
                {
                    Console.WriteLine("Restart is needed.");

                    Exit();
                    return;
                }

                Console.WriteLine("Initializing Steam.");
                SteamInitialized = SteamCore.Initialize();
                Console.WriteLine("Steam initialization: {0}", SteamInitialized ? "Success" : "Failed");

                //

                //Test_P2P();
                //Test_CreateLobby();
            }

#if DEBUG
            if (Assets.HotSwap)
                SetupHotswap();
#endif

            EnsureFormPosition();

            FragSharp.Initialize(Content, GraphicsDevice);
            GridHelper.Initialize(GraphicsDevice);

            Assets.Initialize();

            //Test_SaveLoad();

            Sounds.Initialize();
            Render.Initialize();
            Spells.Initialize();
            Networking.Start();

            SteamWrapper.SteamHtml.Initialize((uint)CurrentConfig.Width, (uint)CurrentConfig.Height);
            //SteamWrapper.SteamHtml.Initialize(2880, 1800);
            //SteamWrapper.SteamHtml.Initialize(1280, 720);

            Activated += ActivatedEvent;
            Deactivated += DeactivatedEvent;

            SetFormOptions();

            BlankWorld = new World();

            base.Initialize();
        }

        private const bool DoWinFormSetting = false;
        private void SetFormOptions()
        {
            if (!DoWinFormSetting) return;

            //Form.MinimizeBox = false;
            //Form.MaximizeBox = false;
        }

        private void EnsureFormPosition()
        {
            if (!DoWinFormSetting) return;

            if (Program.PosX >= 0 && Program.PosY >= 0)
            {
                //Form.Location = new System.Drawing.Point(Program.PosX - 14, Program.PosY);
            }
        }

        private void SetFormTopMost()
        {
            if (!DoWinFormSetting) return;

            //Form.TopMost = true;
        }

        private void SetFormWindowed()
        {
            if (!DoWinFormSetting) return;

            //Form.TopMost = true;
            //Form.FormBorderStyle = Windows.FormBorderStyle.FixedSingle;
        }

        private void SetFormFullscreen()
        {
            if (!DoWinFormSetting) return;

            //Control.Location = new System.Drawing.Point(0, 0);
            //Form.TopMost = true;
            //Form.FormBorderStyle = Windows.FormBorderStyle.None;
        }

        private void CycleFormTopMost()
        {
            if (!DoWinFormSetting) return;

            //Form.TopMost = true;
            //Form.TopMost = false;
        }

        private void EnsureFormPos()
        {
            if (!DoWinFormSetting) return;

            //if (DrawCount % 100 == 0 && CurrentConfig.Fullscreen && (Form.Location.X < 0 || Form.Location.Y < 0))
            //{
            //    Console.WriteLine("Form is outside bounds of monitor, moving form now. Draw count {0}", DrawCount);
            //    Form.Location = new System.Drawing.Point(0, 0);
            //}
        }

#if DEBUG
        public string HotSwapDir = "Content\\HotSwap\\";
        private void SetupHotswap()
        {
            string ProjDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/WAL/Source/";
            string ArtSrcDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/WAL/Source/Content/Art/";

            var cwd = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Replace('\\', '/');

            // On the dev machine copy actual art folder to hot swap, and then work from art folder directly
            if (cwd.Contains(ProjDir))
            {
                HotSwapDir = "C:/Users/Jordan/Desktop/Dir/Pwnee/Games/WAL/Source/Game/bin/x86/Debug/Content/HotSwap/";
                try
                {
                    Directory.Delete(HotSwapDir, true);
                }
                catch
                {

                }

                var source_art = Directory.EnumerateFiles(ArtSrcDir, "*", SearchOption.AllDirectories);

                foreach (var file in source_art)
                {
                    string dest = file.Replace("Content/Art", "Game/bin/x86/Debug/Content/HotSwap");
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
            try
            {
                base.Update(gameTime);
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            try
            {
                // The following seems to prevent context loss from WinKey + D.
                // If you want to be even more aggressive, set TopMost = true and return.
                CycleFormTopMost();

                // Don't do the default behavior.
                //base.OnDeactivated(sender, args);
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }
        }

        void DeactivatedEvent(object sender, EventArgs args)
        {
            try
            {
                // The following code saves the game state to a buffer
                // so it can be retrieved if context is lost.
                // There doesn't seem to be a way to call this soon enough
                // before the context is lost, however.
                // Currently this is not called, because we do not call
                // base.OnDeactivated in our override OnDeactivated method.

                Render.UnsetDevice();

                if (AutoSaveOnTab && !FocusSaved && World != null)
                {
                    World.SaveInBuffer();
                    FocusSaved = true;
                }

                FakeMinimize();
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }
        }

        bool ActivateFakeFullScreen = false;
        void ActivatedEvent(object sender, EventArgs args)
        {
            if (AutoSaveOnTab && FocusSaved && World != null)
            {
                World.LoadFromBuffer();
                FocusSaved = false;
            }

            ActivateFakeFullScreen = true;
        }

        // Use these to grab the Control and Form for the XNA window.
        // This is needed on vanilla XNA on Windows in order to
        // manipulate the window to allow it to be always on top.
        // This is unfortunately necessary because when the app
        // loses top level focus the GPU can lose context and the
        // entire game state is lost. This doesn't seem to be a 
        // problem with FNA.
        //Windows.Control Control
        //{
        //    get
        //    {
        //        IntPtr hWnd = Window.Handle;
        //        var control = Windows.Control.FromHandle(hWnd);
        //        return control;
        //    }
        //}

        //Windows.Form Form
        //{
        //    get
        //    {
        //        return Control.FindForm();
        //    }
        //}

        void ApplyConfigToForm()
        {
            ActivateFakeFullScreen = false;

            try
            {
                if (CurrentConfig.Fullscreen)
                {
                    SetFormFullscreen();
                }
                else
                {
                    SetFormWindowed();
                }
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }
        }

        void FakeMinimize()
        {
            try
            {
                SetFormTopMost();
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }

            /* If we actually want to minimize, do the following.
            if (FullScreen)
            {
                Form.TopMost = false;
                Form.FormBorderStyle = Windows.FormBorderStyle.None;
                //Form.WindowState = Windows.FormWindowState.Minimized;
                Control.Location = new System.Drawing.Point(-10000, -10000);
            }
            */
        }

        public int DrawCount = 0;
        bool FocusSaved = false;

        protected override void OnExiting(object sender, EventArgs args)
        {
            Networking.Cleanup();

            SteamCore.Shutdown();

            Environment.Exit(0);
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Sometimes the fullscreen is misconfigured at initial load.
            // Check if window is off center and recenter if necessary.
            EnsureFormPos();

            try
            {
                _Draw(gameTime);
            }
            catch (Exception e)
            {
                Program.LogDump(e);
            }
        }

        void _Draw(GameTime gameTime)
        {
            DeltaT = gameTime.ElapsedGameTime.TotalSeconds;
            T = gameTime.TotalGameTime.TotalSeconds;

            GameOverLogic();

            if (NeedsApplication)
            {
                DoActivation();
            }

            if (SteamAvailable)
            {
                SteamCore.Update();
                SteamHtml.Update();
            }

            GameClass.Time = gameTime;
            DrawCount++;
            TimeSinceLoad += gameTime.ElapsedGameTime.TotalSeconds;

            // Update songs
            if (SongWad.Wad != null)
            {
                SongWad.Wad.PhsxStep();
            }

            if (GameClass.GameActive)
            {
                if (ActivateFakeFullScreen)
                {
                    ApplyConfigToForm();
                }

                Input.Update();
            }
            else
            {
                // Normally we would pause the game if we don't have focus.
                // Since this is multiplayer, and since we don't want to give up
                // our GPU resources, we keep rendering as usual.
                // Hopefully nothing awful happens.
                //Graphics.SetRenderTarget(null);
                //Graphics.Clear(Color.Black);

                //base.Draw(gameTime);
                //return;

                Input.Update();
            }

            if (World == null) World = new World(Skeleton: true);
            GameLogic(gameTime);

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

        public static void DrawFullScreen(Texture2D texture)
        {
            float a = 1920f / 1080f;
            var q = new RectangleQuad();

            q.SetupVertices(new vec2(-a, -1), new vec2(a, 1), vec2.Zero, vec2.Ones);
            q.SetColor(new color(1f, 1f, 1f, 1f));
            DrawTextureSmooth.Using(new vec4(0, 0, 1, 1), ScreenAspect, texture);
            q.Draw(GameClass.Graphics);
        }

        public string PlayerName()
        {
            if (SteamAvailable)
            {
                return SteamCore.PlayerName();
            }
            else
            {
                return "Me";
            }
        }
    }
}
