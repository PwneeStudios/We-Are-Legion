using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Diagnostics;

using FragSharpFramework;

namespace GpuSim
{
	public static class RndExtension
	{
		public static float RndBit(this System.Random rnd)
		{
			return rnd.NextDouble() > .5 ? 1 : 0;
		}
	}

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Pinnacle : Game
	{
		const bool UnlimitedSpeed = false;

		vec2 CameraPos = vec2.Zero;
		float CameraZoom = 30;
        float CameraAspect = 1;
        vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

		public KeyboardState CurKeyboard, PrevKeyboard;
		public MouseState CurMouse, PrevMouse;

		GraphicsDeviceManager graphics;

		RenderTarget2D target1, target2, texture;

		Texture2D UnitTexture, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16, GroundTexture;

		public Pinnacle()
		{
			graphics = new GraphicsDeviceManager(this);

			Window.Title = "Gpu Sim Test";
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 1024;
			//graphics.IsFullScreen = rez.Mode == WindowMode.Fullscreen;
			graphics.SynchronizeWithVerticalRetrace = !UnlimitedSpeed;
			IsFixedTimeStep = !UnlimitedSpeed;

			Content.RootDirectory = "Content";
		}

		void Swap<T>(ref T a, ref T b)
		{
			T temp = a;
			a = b;
			b = temp;
		}

		void UpdateGrid(Color[] clr, int w, int h)
		{
			for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				int index = i * h + j;
				Color lookup = clr[index];
				int dir = (int)lookup.R;

				int index2;
				switch (dir)
				{
					case 1: index2 = (i + 1 + w) % w * h + (j + 0 + h) % h; break;
					case 2: index2 = (i + 0 + w) % w * h + (j + 1 + h) % h; break;
					case 3: index2 = (i - 1 + w) % w * h + (j + 0 + h) % h; break;
					case 4: index2 = (i + 0 + w) % w * h + (j - 1 + h) % h; break;
					default: continue;
				}

				if (clr[index2].R == 0)
				{
					clr[index2] = lookup;
					clr[index] = Color.Transparent;
				}
				else
				{
					dir++;
					if (dir > 4) dir = 1;
					clr[index].R = (byte)dir;
				}
			}}
		}

        Quad Ground;

		protected override void Initialize()
		{
            FragSharp.Initialize(Content, GraphicsDevice);

            GridHelper.Initialize(GraphicsDevice);

			UnitTexture = Content.Load<Texture2D>("Art\\kid");
			UnitTexture_2 = Content.Load<Texture2D>("Art\\kid_2");
			UnitTexture_4 = Content.Load<Texture2D>("Art\\kid_4");
			UnitTexture_8 = Content.Load<Texture2D>("Art\\kid_8");
			UnitTexture_16 = Content.Load<Texture2D>("Art\\kid_16");

			GroundTexture = Content.Load<Texture2D>("Art\\Grass");

            float GroundRepeat = 100;
            Ground = new Quad(new vec2(-1, -1), new vec2(1, 1), new vec2(0, 0), new vec2(1, 1) * GroundRepeat);

			const int w = 1024, h = 1024;
			texture = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			Color[] clr = new Color[w * h];
			texture.GetData(clr);
			var rnd = new System.Random();
			for (int i = 0; i < w; i++)
			for (int j = 0; j < h; j++)
			{
                if (rnd.NextDouble() > 0.8)
                //if (i == w / 2 && j == h / 2)
                //if (Math.Abs(i - w / 2) < 500)
                //if (j == h / 2)
                //if (i % 9 == 0)
                //if (j % 9 == 0 || i % 9 == 0)
                if (true)
                {
                    int dir = rnd.Next(1, 5);
                    //dir = 2;

                    int g = 0;
                    //int g = rnd.Next(0, 255);
                    //if (rnd.NextDouble() > .5)
                    //    g = 2;
                    //else
                    //    g = 0;

                    int b = 0;
                    //int b = rnd.Next(0, 255);

                    clr[i * h + j] = new Color(dir, g, b, rnd.Next(1, 255));
                }

                //// Unit Test
                ////if (i > w/4 && i < 3*w/4 && j > h/4 && j < 3*h/4 && rnd.NextDouble() > .25)
                //if (rnd.NextDouble() > .8)
                //{
                //    int dir = rnd.Next(1, 5);
                //    int g = rnd.Next(0, 255);
                //    if (rnd.NextDouble() > .5)
                //        g = 2;
                //    else
                //        g = 0;

                //    //dir = 2;
                //    int b = rnd.Next(0, 255);
                //    clr[i * h + j] = new Color(dir, g, b, rnd.Next(1, 255));
                //}
				else
				{
					clr[i * h + j] = new Color(0, 0, 0, 0);
				}
			}

            //Stopwatch t = new Stopwatch();
            //t.Start();
            //int iterations = 10;
            //for (int i = 0; i < iterations; i++)
            //{
            //    UpdateGrid(clr, w, h);
            //}
            //t.Stop();
            //double avg = t.Elapsed.TotalSeconds / iterations;
            //Console.WriteLine("Average cpu time: {0}", avg);

			texture.SetData(clr);

			target1 = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			target2 = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			target2.SetData(clr);

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
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			CurKeyboard = Keyboard.GetState();

			CurMouse = Mouse.GetState();
			vec2 DeltaMousPos = new vec2(CurMouse.X - PrevMouse.X, CurMouse.Y - PrevMouse.Y);
			float DeltaMouseScroll = CurMouse.ScrollWheelValue - PrevMouse.ScrollWheelValue;
			PrevMouse = CurMouse;

			//float ZoomRate = 1.0435f;
			float ZoomRate = 1.125f;
			if (DeltaMouseScroll < 0) CameraZoom /= ZoomRate;
			else if (DeltaMouseScroll > 0) CameraZoom *= ZoomRate;

			float MoveRate = .00165f;
			if (CurMouse.LeftButton == ButtonState.Pressed)
				CameraPos += DeltaMousPos / CameraZoom * MoveRate * new vec2(-1, 1);

			base.Update(gameTime);
		}

		const double DelayBetweenUpdates = .5;
		double SecondsSinceLastUpdate = DelayBetweenUpdates;
		public static float PercentSimStepComplete = 0;

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			//if (CurKeyboard.IsKeyDown(Keys.Enter))
			SecondsSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;

			// Render setup
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

			// Check if we need to do a simulation update
            if (UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates)
            //if (SecondsSinceLastUpdate > DelayBetweenUpdates)
            {
                SecondsSinceLastUpdate -= DelayBetweenUpdates;

                SimulationUpdate();
            }

			// Choose texture
			Texture2D draw_texture = null;
			float z = 12;
			if (CameraZoom > z)
				draw_texture = UnitTexture;
			else if (CameraZoom > z/2)
				draw_texture = UnitTexture_2;
			else if (CameraZoom > z/4)
				draw_texture = UnitTexture_4;
			else if (CameraZoom > z/8)
				draw_texture = UnitTexture_8;
			else
				draw_texture = UnitTexture_16;

			// Draw texture to screen
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);

			PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            DrawGrass.Use(camvec, CameraAspect, GroundTexture);
            Ground.Draw(GraphicsDevice);

            DrawUnit.Use(camvec, CameraAspect, texture, target2, draw_texture, PercentSimStepComplete);
            GridHelper.DrawGrid();

			base.Draw(gameTime);
		}

		private void SimulationUpdate()
		{
			// Draw texture to target 1
            Movement_Phase1.Use(texture, Output: target1);

			// Draw texture to target 2
            Movement_Phase2.Use(target1, texture, Output: target2);

			Swap(ref texture, ref target2);
		}
	}
}
