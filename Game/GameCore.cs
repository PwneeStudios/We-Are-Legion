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
        public static bool GameActive { get { return GameClass.Game.IsActive || Program.AlwaysActive; } }

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
        public static DataGroup Data { get { return World.DataGroup; } }
        public static PlayerInfo[] PlayerInfo { get { return World.PlayerInfo; } }

        bool FullScreen = false;
        bool AutoSaveOnTab = false;

        public GameClass()
        {
            Game = this;

            graphics = new GraphicsDeviceManager(this);

            Window.Title = "We Are Legion";

            if (Program.Width < 0 || Program.Height < 0)
            {
                FullScreen = true;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = Program.Width;
                graphics.PreferredBackBufferHeight = Program.Height;
            }

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

            if (FullScreen)
            {
                FakeFullscreen();
            }

            if (Program.StartupMap != null)
            {
                //State = GameState.ToMap;
                State = GameState.ToEditor;
            }
        }

        protected override void Initialize()
        {
#if DEBUG
            //if (Assets.HotSwap && !Program.Server && !Program.Client)
            if (Assets.HotSwap)
                SetupHotswap();
#endif

            if (Program.PosX >= 0 && Program.PosY >= 0)
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);

                form.Location = new System.Drawing.Point(Program.PosX - 14, Program.PosY);
            }

            FragSharp.Initialize(Content, GraphicsDevice);
            GridHelper.Initialize(GraphicsDevice);

            Assets.Initialize();
            Render.Initialize();

            Spells.Initialize();
            Networking.Start();

            AwesomiumInitialize();

            base.Initialize();
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
            base.Update(gameTime);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            Render.UnsetDevice();

            if (AutoSaveOnTab && !FocusSaved && World != null)
            {
                World.SaveInBuffer();
                FocusSaved = true;
            }

            FakeMinimize();

            base.OnDeactivated(sender, args);
        }

        bool ActivateFakeFullScreen = false;
        protected override void OnActivated(object sender, EventArgs args)
        {
            if (AutoSaveOnTab && FocusSaved && World != null)
            {
                World.LoadFromBuffer();
                FocusSaved = false;
            }

            ActivateFakeFullScreen = true;

            base.OnActivated(sender, args);
        }

        void FakeFullscreen()
        {
            ActivateFakeFullScreen = false;

            IntPtr hWnd = Window.Handle;
            var control = Windows.Control.FromHandle(hWnd);
            var form = control.FindForm();

            if (FullScreen)
            {
                //control.Show();
                control.Location = new System.Drawing.Point(0, 0);

                form.TopMost = true;

                form.FormBorderStyle = Windows.FormBorderStyle.None;
                //form.WindowState = Windows.FormWindowState.Maximized;
            }
            else
            {
                form.TopMost = false;

                form.FormBorderStyle = Windows.FormBorderStyle.FixedSingle;
                //form.WindowState = Windows.FormWindowState.Normal;
            }
        }

        void FakeMinimize()
        {
            IntPtr hWnd = Window.Handle;
            var control = Windows.Control.FromHandle(hWnd);
            var form = control.FindForm();

            if (FullScreen)
            {
                form.TopMost = false;

                form.FormBorderStyle = Windows.FormBorderStyle.None;
                //form.WindowState = Windows.FormWindowState.Minimized;

                control.Location = new System.Drawing.Point(-10000, -10000);
            }
        }


        int DrawCount = 0;
        bool FocusSaved = false;

        protected override void OnExiting(object sender, EventArgs args)
        {
            Environment.Exit(0);
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GameClass.Time = gameTime;
            DrawCount++;
            TimeSinceLoad += gameTime.ElapsedGameTime.TotalSeconds;

            if (GameClass.GameActive)
            {
                if (ActivateFakeFullScreen) FakeFullscreen();

                Input.Update();
            }
            else
            {
                Graphics.SetRenderTarget(null);
                Graphics.Clear(Color.Black);

                base.Draw(gameTime);
                return;
            }

            if (Buttons.Back.Down())
                this.Exit();

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
    }
}
