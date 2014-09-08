using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

#if DEBUG
using System.IO;
using System.Security.Permissions;
#endif

namespace GpuSim
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

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 1024;

            //graphics.IsFullScreen = false;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            //graphics.IsFullScreen = false;
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

            World = new World();
            World.Load("Beset.m3n");

            //World.Migrate();
            //Render.UnsetDevice();
            //World.Save("Beset.m3n");

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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GameClass.Time = gameTime;
            DrawCount++;

            Input.Update();

            if (Buttons.Back.Down())
                this.Exit();

            if (World.MapEditor)
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
            World.Draw();

            base.Draw(gameTime);
        }
    }
}
