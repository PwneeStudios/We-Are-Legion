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

using CoreEngine;

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
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		const bool UnlimitedSpeed = false;

		Vector2 CameraPos = Vector2.Zero;
		float CameraZoom = 30;

		public KeyboardState CurKeyboard, PrevKeyboard;
		public MouseState CurMouse, PrevMouse;

		const int TOP_LEFT = 0;
		const int TOP_RIGHT = 1;
		const int BOTTOM_RIGHT = 2;
		const int BOTTOM_LEFT = 3;

		GraphicsDeviceManager graphics;

		EzEffect DrawUnitEffect, BasicEffect, compute1, compute2;
		public EffectParameter fx_Texture, fx_CameraAspect, fx_CameraPos;

		RenderTarget2D target1, target2, texture;

		Texture2D UnitTexture, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16, GroundTexture;

		VertexPositionColorTexture[] vertexData, ground_vertexData;
		int[] indexData;

		public Game1()
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

		protected override void Initialize()
		{
			UnitTexture = Content.Load<Texture2D>("Art\\kid");
			UnitTexture_2 = Content.Load<Texture2D>("Art\\kid_2");
			UnitTexture_4 = Content.Load<Texture2D>("Art\\kid_4");
			UnitTexture_8 = Content.Load<Texture2D>("Art\\kid_8");
			UnitTexture_16 = Content.Load<Texture2D>("Art\\kid_16");

			GroundTexture = Content.Load<Texture2D>("Art\\Grass");

			DrawUnitEffect = new EzEffect(Content, "Shaders\\DrawUnit");
			BasicEffect = new EzEffect(Content, "Shaders\\BasicEffect");
			//compute = new EzEffect(Content, "Shaders\\GameOfLife");
			compute1 = new EzEffect(Content, "Shaders\\Unit");
			compute2 = new EzEffect(Content, "Shaders\\Unit2");

			const int w = 1024, h = 1024;
			texture = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			Color[] clr = new Color[w * h];
			texture.GetData(clr);
			var rnd = new System.Random();
			for (int i = 0; i < w; i++)
			for (int j = 0; j < h; j++)
			{
				// Unit Test
				//if (i > w/4 && i < 3*w/4 && j > h/4 && j < 3*h/4 && rnd.NextDouble() > .25)
				if (rnd.NextDouble() > .8)
				{
					int dir = rnd.Next(1, 5);
					int g = rnd.Next(0, 255);
					if (rnd.NextDouble() > .5)
						g = 2;
					else
						g = 0;

					dir = 2;
					clr[i * h + j] = new Color(dir, g, rnd.Next(1, 255), rnd.Next(1, 255));
				}
				else
				{
					clr[i * h + j] = new Color(0, 0, 0, 0);
				}
			}
			texture.SetData(clr);

			target1 = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			target2 = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			target2.SetData(clr);

			SetupVertices(Color.White);
			SetupIndices();

			base.Initialize();
		}

		private void SetupVertices(Color color)
		{
			const float HALF_SIDE = 1.0f;
			const float Z = 0.0f;

			vertexData = new VertexPositionColorTexture[4];
			vertexData[TOP_LEFT] = new VertexPositionColorTexture(new Vector3(-HALF_SIDE, HALF_SIDE, Z), color, new Vector2(0, 0));
			vertexData[TOP_RIGHT] = new VertexPositionColorTexture(new Vector3(HALF_SIDE, HALF_SIDE, Z), color, new Vector2(1, 0));
			vertexData[BOTTOM_RIGHT] = new VertexPositionColorTexture(new Vector3(HALF_SIDE, -HALF_SIDE, Z), color, new Vector2(1, 1));
			vertexData[BOTTOM_LEFT] = new VertexPositionColorTexture(new Vector3(-HALF_SIDE, -HALF_SIDE, Z), color, new Vector2(0, 1));

			ground_vertexData = new VertexPositionColorTexture[4];
			ground_vertexData[TOP_LEFT] = new VertexPositionColorTexture(1 * new Vector3(-HALF_SIDE, HALF_SIDE, Z), color, 100 * new Vector2(0, 0));
			ground_vertexData[TOP_RIGHT] = new VertexPositionColorTexture(1 * new Vector3(HALF_SIDE, HALF_SIDE, Z), color, 100 * new Vector2(1, 0));
			ground_vertexData[BOTTOM_RIGHT] = new VertexPositionColorTexture(1 * new Vector3(HALF_SIDE, -HALF_SIDE, Z), color, 100 * new Vector2(1, 1));
			ground_vertexData[BOTTOM_LEFT] = new VertexPositionColorTexture(1 * new Vector3(-HALF_SIDE, -HALF_SIDE, Z), color, 100 * new Vector2(0, 1));
		}

		private void SetupIndices()
		{
			indexData = new int[6];
			indexData[0] = TOP_LEFT;
			indexData[1] = BOTTOM_RIGHT;
			indexData[2] = BOTTOM_LEFT;

			indexData[3] = TOP_LEFT;
			indexData[4] = TOP_RIGHT;
			indexData[5] = BOTTOM_RIGHT;
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
			Vector2 DeltaMousPos = new Vector2(CurMouse.X - PrevMouse.X, CurMouse.Y - PrevMouse.Y);
			float DeltaMouseScroll = CurMouse.ScrollWheelValue - PrevMouse.ScrollWheelValue;
			PrevMouse = CurMouse;

			//float ZoomRate = 1.0435f;
			float ZoomRate = 1.125f;
			if (DeltaMouseScroll < 0) CameraZoom /= ZoomRate;
			else if (DeltaMouseScroll > 0) CameraZoom *= ZoomRate;

			float MoveRate = .00165f;
			if (CurMouse.LeftButton == ButtonState.Pressed)
				CameraPos += DeltaMousPos / CameraZoom * MoveRate * new Vector2(-1, 1);

			base.Update(gameTime);
		}

		const double DelayBetweenUpdates = .5;
		double SecondsSinceLastUpdate = DelayBetweenUpdates;
		float PercentSimStepComplete = 0;

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

			BasicEffect.Set(CameraPos, CameraZoom, GroundTexture);
			GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, ground_vertexData, 0, 4, indexData, 0, 2);

			DrawUnitEffect.PercentSimStepComplete.SetValue(PercentSimStepComplete);
			DrawUnitEffect.drawTexture.SetValue(draw_texture);
			DrawUnitEffect.Set(CameraPos, CameraZoom, texture, target2);
			GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2);

			base.Draw(gameTime);
		}

		private void SimulationUpdate()
		{
			// Draw texture to target 1
			GraphicsDevice.SetRenderTarget(target1);
			GraphicsDevice.Clear(Color.Transparent);

			compute1.Set(texture);
			GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2);

			// Draw texture to target 2
			if (compute2 != null)
			{
				GraphicsDevice.SetRenderTarget(target2);
				GraphicsDevice.Clear(Color.Transparent);

				compute2.Set(target1, texture);
				GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexData, 0, 4, indexData, 0, 2);
			}

			Swap(ref texture, ref target2);
		}
	}
}
