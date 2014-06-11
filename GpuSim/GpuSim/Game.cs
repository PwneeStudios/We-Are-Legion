using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

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

        GraphicsDeviceManager graphics;

        public static World World;

		public GameClass()
		{
            Game = this;

			graphics = new GraphicsDeviceManager(this);

			Window.Title = "Gpu Sim Test";

            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth  = 1024;
            graphics.PreferredBackBufferHeight = 1024;
            
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            
            graphics.SynchronizeWithVerticalRetrace = !UnlimitedSpeed;
            IsFixedTimeStep = !UnlimitedSpeed;

			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
            FragSharp.Initialize(Content, GraphicsDevice);
            GridHelper.Initialize(GraphicsDevice);

            Assets.Initialize();
            Render.Initialize();

            World = new World();

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
		}

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

            World.Update();
            World.Draw();

			base.Draw(gameTime);
		}
	}
}
