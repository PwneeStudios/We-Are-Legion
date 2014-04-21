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

    public static class InputInfo
    {
        public static KeyboardState CurKeyboard, PrevKeyboard;

        public static MouseState CurMouse, PrevMouse;
        public static vec2 MousePos;

        public static vec2 DeltaMousPos;
        public static float DeltaMouseScroll;

        public static void Update()
        {
            PrevMouse = CurMouse;

            CurKeyboard = Keyboard.GetState();

            CurMouse = Mouse.GetState();
            MousePos = new vec2(CurMouse.X, CurMouse.Y);

            DeltaMousPos = new vec2(CurMouse.X - PrevMouse.X, CurMouse.Y - PrevMouse.Y);
            DeltaMouseScroll = CurMouse.ScrollWheelValue - PrevMouse.ScrollWheelValue;
        }

        public static bool LeftMousePressed
        {
            get
            {
                return CurMouse.LeftButton  == ButtonState.Pressed &&
                       PrevMouse.LeftButton == ButtonState.Released;
            }
        }

        public static bool LeftMouseDown
        {
            get
            {
                return CurMouse.LeftButton  == ButtonState.Pressed;
            }
        }
    }

    public static class KeyExtension
    {
        public static bool Pressed(this Keys key)
        {
            return InputInfo.CurKeyboard.IsKeyDown(key);
        }
    }

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class M3ngineGame : Game
	{
		const bool UnlimitedSpeed = false;

		vec2 CameraPos = vec2.Zero;
		float CameraZoom = 30;
        float CameraAspect = 1;
        vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

		GraphicsDeviceManager graphics;

        RenderTarget2D
            Temp1, Temp2,
            Previous, Current,
            Paths;

		Texture2D
            UnitTexture, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            GroundTexture, MouseTexture;

		public M3ngineGame()
		{
			graphics = new GraphicsDeviceManager(this);

			Window.Title = "Gpu Sim Test";
            graphics.PreferredBackBufferWidth  = 1024;
            graphics.PreferredBackBufferHeight = 1024;
			//graphics.IsFullScreen = rez.Mode == WindowMode.Fullscreen;
			graphics.SynchronizeWithVerticalRetrace = !UnlimitedSpeed;
			IsFixedTimeStep = !UnlimitedSpeed;

			Content.RootDirectory = "Content";
		}

        public vec2 Screen
        {
            get
            {
                return new vec2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }
        }

        public float Restrict(float val, float a, float b)
        {
            if (val < a) return a;
            if (val > b) return b;
            return val;
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

        RectangleQuad Ground;

		protected override void Initialize()
		{
            FragSharp.Initialize(Content, GraphicsDevice);

            GridHelper.Initialize(GraphicsDevice);

			UnitTexture    = Content.Load<Texture2D>("Art\\kid");
			UnitTexture_2  = Content.Load<Texture2D>("Art\\kid_2");
			UnitTexture_4  = Content.Load<Texture2D>("Art\\kid_4");
			UnitTexture_8  = Content.Load<Texture2D>("Art\\kid_8");
			UnitTexture_16 = Content.Load<Texture2D>("Art\\kid_16");

			GroundTexture = Content.Load<Texture2D>("Art\\Grass");

            MouseTexture = Content.Load<Texture2D>("Art\\MouseSelect");

            float GroundRepeat = 100;
            Ground = new RectangleQuad(new vec2(-1, -1), new vec2(1, 1), new vec2(0, 0), new vec2(1, 1) * GroundRepeat);

			const int w = 1024, h = 1024;
			Current = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			Color[] clr = new Color[w * h];
			Current.GetData(clr);
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
                {
                    int dir = rnd.Next(1, 5);
                    int type = rnd.Next(0, 0);

                    int g = 0;
                    int b = 0;

                    clr[i * h + j] = new Color(dir, g, b, type);
                }
				else
				{
					clr[i * h + j] = new Color(0, 0, 0, 0);
				}
			}

			Current.SetData(clr);

			Temp1    = new RenderTarget2D(graphics.GraphicsDevice, w, h);
            Temp2    = new RenderTarget2D(graphics.GraphicsDevice, w, h);
			Previous = new RenderTarget2D(graphics.GraphicsDevice, w, h); Previous.SetData(clr);
            Paths    = new RenderTarget2D(graphics.GraphicsDevice, w, h);

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

            InputInfo.Update();

            // Zoom in/out, into the location of the cursor
            var world_mouse_pos = GetWorldCoordinate(InputInfo.MousePos);
            var hold_camvec = camvec;
            
			float ZoomRate = 1.3333f;
            if      (InputInfo.DeltaMouseScroll < 0) CameraZoom /= ZoomRate;
            else if (InputInfo.DeltaMouseScroll > 0) CameraZoom *= ZoomRate;

            float KeyZoomRate = 1.125f;
            if      (Keys.X.Pressed() || Keys.E.Pressed()) CameraZoom /= KeyZoomRate;
            else if (Keys.Z.Pressed() || Keys.Q.Pressed()) CameraZoom *= KeyZoomRate;

            if (CameraZoom < 1) CameraZoom = 1;
            if (CameraZoom > 1000) CameraZoom = 1000;

            var shifted = GetShiftedCamera(InputInfo.MousePos, camvec, world_mouse_pos);
            CameraPos = shifted;


            // Move the camera via: Click And Drag
            //float MoveRate_ClickAndDrag = .00165f;
            //if (InputInfo.LeftMouseDown)
            //    CameraPos += InputInfo.DeltaMousPos / CameraZoom * MoveRate_ClickAndDrag * new vec2(-1, 1);

            // Move the camera via: Push Edge
            float MoveRate_PushEdge = .065f;
            var push_dir = vec2.Zero;
            float EdgeRatio = .1f;
            push_dir.x += -Restrict((EdgeRatio * Screen.x -     InputInfo.MousePos.x) / (EdgeRatio * Screen.x), 0, 1);
            push_dir.x +=  Restrict((InputInfo.MousePos.x - (1-EdgeRatio) * Screen.x) / (EdgeRatio * Screen.x), 0, 1);
            push_dir.y -= -Restrict((EdgeRatio * Screen.y - InputInfo.MousePos.y) / (EdgeRatio * Screen.y), 0, 1);
            push_dir.y -=  Restrict((InputInfo.MousePos.y - (1 - EdgeRatio) * Screen.y) / (EdgeRatio * Screen.y), 0, 1);

            CameraPos += push_dir / CameraZoom * MoveRate_PushEdge;

            // Move the camera via: Keyboard
            var dir = vec2.Zero;
            if (Keys.Up   .Pressed() || Keys.W.Pressed()) dir.y =  1;
            if (Keys.Down .Pressed() || Keys.S.Pressed()) dir.y = -1;
            if (Keys.Right.Pressed() || Keys.D.Pressed()) dir.x =  1;
            if (Keys.Left .Pressed() || Keys.A.Pressed()) dir.x = -1;

            float MoveRate_Keyboard = .05f;
            CameraPos += dir / CameraZoom * MoveRate_Keyboard;


            // Make sure the camera doesn't go too far offscreen
            var TR = GetWorldCoordinate(new vec2(Screen.x, 0));
            if (TR.x > 1)  CameraPos = new vec2(CameraPos.x - (TR.x - 1), CameraPos.y);
            if (TR.y > 1)  CameraPos = new vec2(CameraPos.x, CameraPos.y - (TR.y - 1));
            var BL = GetWorldCoordinate(new vec2(0, Screen.y));
            if (BL.x < -1) CameraPos = new vec2(CameraPos.x - (BL.x + 1), CameraPos.y);
            if (BL.y < -1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (BL.y + 1));


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

            PathUpdate();

            SelectionUpdate();

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

            DrawGrass.Using(camvec, CameraAspect, GroundTexture);
            Ground.Draw(GraphicsDevice);

            DrawUnit.Using(camvec, CameraAspect, Current, Previous, Paths, draw_texture, PercentSimStepComplete);
            GridHelper.DrawGrid();


            vec2 WorldCord = GetWorldCoordinate(InputInfo.MousePos);

            DrawMouse.Using(camvec, CameraAspect, MouseTexture);
            RectangleQuad.Draw(GraphicsDevice, WorldCord, vec2.Ones * .2f / CameraZoom);

			base.Draw(gameTime);
		}

        vec2 GetWorldCoordinate(vec2 pos)
        {
            var screen = new vec2(Screen.x, Screen.y);
            var ScreenCord = (2 * pos - screen) / screen;
            vec2 WorldCord;
            WorldCord.x = CameraAspect * ScreenCord.x / camvec.z + camvec.x;
            WorldCord.y = -ScreenCord.y / camvec.w + camvec.y;
            return WorldCord;
        }

        vec2 GetShiftedCamera(vec2 pos, vec4 prev_camvec, vec2 prev_worldcoord)
        {
            var screen = new vec2(Screen.x, Screen.y);
            var ScreenCord = (2 * pos - screen) / screen;

            vec2 shifted_cam;
            shifted_cam.x = prev_worldcoord.x - CameraAspect * ScreenCord.x / prev_camvec.z;
            shifted_cam.y = prev_worldcoord.y + ScreenCord.y / prev_camvec.w;

            return shifted_cam;
        }

        void PathUpdate()
        {
            Pathfinding.Apply(Paths, Current, Output: Temp1);
            Pathfinding.Apply(Temp1, Current, Output: Temp2);
            Swap(ref Paths, ref Temp2);
        }

        void SelectionUpdate()
        {
            vec2 WorldCord = GetWorldCoordinate(InputInfo.MousePos);

            bool Deselect  = InputInfo.LeftMousePressed;
            bool Selecting = InputInfo.LeftMouseDown;

            DataDrawMouse.Using(Output: Temp1, Clear: Color.Transparent);
            if (Selecting)
                RectangleQuad.Draw(GraphicsDevice, WorldCord, vec2.Ones * .2f / CameraZoom);

            ActionSelect.Apply(Current, Temp1, Deselect, Output: Temp2);
            Swap(ref Temp2, ref Current);

            ActionSelect.Apply(Previous, Temp1, Deselect, Output: Temp2);
            Swap(ref Temp2, ref Previous);
        }

		void SimulationUpdate()
		{
            Movement_Phase1.Apply(Current,               Output: Temp1);
            Movement_Phase2.Apply(Current, Temp1, Paths, Output: Temp2);
            
            Swap(ref Current, ref Previous);
            Swap(ref Temp2, ref Current);
		}
	}
}
