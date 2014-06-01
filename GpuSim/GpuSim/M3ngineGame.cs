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

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    public static class Texture2DExtension
    {
        public static vec2 UnitSize(this Texture2D Texture)
        {
            return new vec2(1, (float)Texture.Height / (float)Texture.Width);
        }

        public static vec2 Size(this Texture2D Texture)
        {
            return new vec2(Texture.Width, Texture.Height);
        }
    }

    public static class ListExtension
    {
        public static void Swap<T>(this List<T> List, int Index, ref T NewElement)
        {
			T temp = List[Index];
            List[Index] = NewElement;
            NewElement = temp;
        }
    }

    public static class RenderTargetExtension
    {
        public static Color[] GetData(this RenderTarget2D RenderTarget)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            Color[] data = new Color[w * h];
            
            RenderTarget.GetData(data);
            
            return data;
        }

        public static Color[] GetData(this RenderTarget2D RenderTarget, vec2 coord)
        {
            return RenderTarget.GetData(coord, new vec2(1, 1));
        }

        public static Color[] GetData(this RenderTarget2D RenderTarget, vec2 coord, vec2 size)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            
            coord = new vec2((int)Math.Floor(coord.x), (int)Math.Floor(coord.y));
            size = new vec2((int)Math.Floor(size.x), (int)Math.Floor(size.y));
            if (coord.x < 0 || coord.y < 0 || coord.x >= w || coord.y >= h) return null;

            int elements = (int)size.x * (int)size.y;
            Color[] data = new Color[elements];
            Rectangle rect = new Rectangle((int)coord.x, (int)coord.y, (int)size.x, (int)size.y);
            RenderTarget.GetData(0, rect, data, 0, elements);

            return data;
        }
    }

	public static class RndExtension
	{
		public static float Bit(this System.Random rnd)
		{
			return rnd.NextDouble() > .5 ? 1 : 0;
		}

        public static int IntRange(this System.Random rnd, int min, int max)
        {
            return (int)(rnd.NextDouble() * (max - min) + min);
        }
	}

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class M3ngineGame : Game
	{
		const bool UnlimitedSpeed = false;

        const bool MouseEnabled = true;

		vec2 CameraPos = vec2.Zero;
		float CameraZoom = 30;
        float CameraAspect = 1;
        vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

		GraphicsDeviceManager graphics;

        RenderTarget2D
            Temp1, Temp2,
            PreviousUnits, CurrentUnits, PreviousData, CurrentData, Extra, TargetData,
            RandomField,
            Corspes,
            SelectField,
            PreviousDraw, CurrentDraw,
            Paths_Right, Paths_Left, Paths_Up, Paths_Down,
            PathToOtherTeams;
        List<RenderTarget2D>
            Multigrid;

		Texture2D
            BuildingTexture_1,
            UnitTexture_1, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            GroundTexture,
            
            Cursor, SelectCircle, SelectCircle_Data;

        public const int w = 1024, h = 1024;
        public static readonly vec2 GridSize = new vec2(w, h);

		public M3ngineGame()
		{
			graphics = new GraphicsDeviceManager(this);

			Window.Title = "Gpu Sim Test";
            graphics.PreferredBackBufferWidth  = w;
            graphics.PreferredBackBufferHeight = h;
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
            //var effect = Content.Load<Effect>("Shaders/__Test");

            FragSharp.Initialize(Content, GraphicsDevice);

            GridHelper.Initialize(GraphicsDevice);

            BuildingTexture_1 = Content.Load<Texture2D>("Art\\Buildings_1");

			UnitTexture_1  = Content.Load<Texture2D>("Art\\Units_1");
			UnitTexture_2  = Content.Load<Texture2D>("Art\\Units_2");
			UnitTexture_4  = Content.Load<Texture2D>("Art\\Units_4");
			UnitTexture_8  = Content.Load<Texture2D>("Art\\Units_8");
			UnitTexture_16 = Content.Load<Texture2D>("Art\\Units_16");

			GroundTexture = Content.Load<Texture2D>("Art\\Grass");

            Cursor            = Content.Load<Texture2D>("Art\\Cursor");
            SelectCircle      = Content.Load<Texture2D>("Art\\SelectCircle");
            SelectCircle_Data = Content.Load<Texture2D>("Art\\SelectCircle_Data");

            float GroundRepeat = 100;
            Ground = new RectangleQuad(new vec2(-1, -1), new vec2(1, 1), new vec2(0, 0), new vec2(1, 1) * GroundRepeat);

            CurrentUnits        = MakeTarget(w, h);
            PreviousUnits       = MakeTarget(w, h);
            
            CurrentData    = MakeTarget(w, h);
            PreviousData   = MakeTarget(w, h);
            
            Extra          = MakeTarget(w, h);
            TargetData     = MakeTarget(w, h);

            Corspes        = MakeTarget(w, h);
            
            SelectField    = MakeTarget(w, h);

            RandomField    = MakeTarget(w, h);

            CurrentDraw    = MakeTarget(w, h);
            PreviousDraw   = MakeTarget(w, h);

            InitialConditions(w, h);

            Temp1 = MakeTarget(w, h);
            Temp2 = MakeTarget(w, h);
            
            Paths_Right = MakeTarget(w, h);
            Paths_Left  = MakeTarget(w, h);
            Paths_Up    = MakeTarget(w, h);
            Paths_Down  = MakeTarget(w, h);

            PathToOtherTeams = MakeTarget(w, h);

            Multigrid = new List<RenderTarget2D>();
            int n = w;
            while (n >= 1)
            {
                Multigrid.Add(MakeTarget(n, n));
                n /= 2;
            }

			base.Initialize();
		}
        
        void InitialConditions(int w, int h)
        {
            Color[] _unit = new Color[w * h];
            Color[] _data = new Color[w * h];
            Color[] _extra = new Color[w * h];
            Color[] _target = new Color[w * h];
            Color[] _random = new Color[w * h];
            Color[] _corpses = new Color[w * h];

            CurrentData.GetData(_data);

            var rnd = new System.Random();
            for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
            {
                _random[i * h + j] = new Color(rnd.IntRange(0, 256), rnd.IntRange(0, 256), rnd.IntRange(0, 256), rnd.IntRange(0, 256));
                _corpses[i * h + j] = new Color(0, 0, 0, 0);

                //if (true)
                if (false)
                //if (rnd.NextDouble() > 0.85f)
                //if (i == w / 2 && j == h / 2)
                //if (Math.Abs(i - w / 2) < 500)
                //if (j == h / 2)
                //if (i % 9 == 0)
                //if (j % 2 == 0 || i % 2 == 0)
                //if (j % 20 == 0 && i % 20 == 0)
                {
                    //int dir = rnd.Next(1, 5);
                    int dir = rnd.Next(1, 5);

                    int action = (int)(255f * SimShader.UnitAction.Attacking);

                    int g = 0;
                    int b = 0;

                    int player = rnd.Next(1, 2);
                    int team   = player;
                    int type   = rnd.Next(1, 2);

                    _unit[i * h + j] = new Color(type, player, team, 0);
                    _data[i * h + j] = new Color(dir, g, b, action);
                    _extra[i * h + j] = new Color(0, 0, 0, 0);
                    _target[i * h + j] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
                }
                else
                {
                    _unit[i * h + j] = new Color(0, 0, 0, 0);
                    _data[i * h + j] = new Color(0, 0, 0, 0);
                    _extra[i * h + j] = new Color(0, 0, 0, 0);
                    _target[i * h + j] = new Color(0, 0, 0, 0);
                }
            }

            for (int i = 0; i < w; i += 50)
            for (int j = 0; j < h; j += 50)
            {
                for (int _i = i; _i < i+3; _i++)
                for (int _j = j; _j < j+3; _j++)
                {
                    _unit[_i * h + _j] = new Color((int)(255f * SimShader.UnitType.Barracks), 1, 1, 0);
                    _data[_i * h + _j] = new Color((int)(255f * SimShader.Dir.Stationary), _j - j, 0, _i - i);
                    _target[_i * h + _j] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
                }
            }

            CurrentUnits.SetData(_unit);
            PreviousUnits.SetData(_unit);

            CurrentData.SetData(_data);
            PreviousData.SetData(_data);

            Extra.SetData(_extra);

            TargetData.SetData(_target);

            RandomField.SetData(_random);

            Corspes.SetData(_corpses);
        }

        private RenderTarget2D MakeTarget(int w, int h)
        {
            return new RenderTarget2D(graphics.GraphicsDevice, w, h);
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
            if (Buttons.Back.Down())
				this.Exit();

            Input.Update();

            //const float MaxZoomOut = 5.33333f, MaxZoomIn = 200;
            const float MaxZoomOut = 1, MaxZoomIn = 200;

            // Zoom all the way out
            if (Keys.Space.Pressed())
                CameraZoom = MaxZoomOut;

            // Zoom in/out, into the location of the cursor
            var world_mouse_pos = ScreenToWorldCoord(Input.CurMousePos);
            var hold_camvec = camvec;

            if (MouseEnabled)
            {
                float MouseWheelZoomRate = 1.3333f;
                if (Input.DeltaMouseScroll < 0) CameraZoom /= MouseWheelZoomRate;
                else if (Input.DeltaMouseScroll > 0) CameraZoom *= MouseWheelZoomRate;
            }

            float KeyZoomRate = 1.125f;
            if      (Buttons.X.Down() || Keys.X.Pressed() || Keys.E.Pressed()) CameraZoom /= KeyZoomRate;
            else if (Buttons.A.Down() || Keys.Z.Pressed() || Keys.Q.Pressed()) CameraZoom *= KeyZoomRate;

            if (CameraZoom < MaxZoomOut) CameraZoom = MaxZoomOut;
            if (CameraZoom > MaxZoomIn)  CameraZoom = MaxZoomIn;

            if (MouseEnabled && !(Buttons.A.Pressed() || Buttons.X.Pressed()))
            {
                var shifted = GetShiftedCamera(Input.CurMousePos, camvec, world_mouse_pos);
                CameraPos = shifted;
            }

            // Move the camera via: Click And Drag
            //float MoveRate_ClickAndDrag = .00165f;
            //if (Input.LeftMouseDown)
            //    CameraPos += Input.DeltaMousPos / CameraZoom * MoveRate_ClickAndDrag * new vec2(-1, 1);

            // Move the camera via: Push Edge
            if (MouseEnabled)
            {
                float MoveRate_PushEdge = .07f;
                var push_dir = vec2.Zero;
                float EdgeRatio = .1f;
                push_dir.x += -Restrict((EdgeRatio * Screen.x - Input.CurMousePos.x) / (EdgeRatio * Screen.x), 0, 1);
                push_dir.x += Restrict((Input.CurMousePos.x - (1 - EdgeRatio) * Screen.x) / (EdgeRatio * Screen.x), 0, 1);
                push_dir.y -= -Restrict((EdgeRatio * Screen.y - Input.CurMousePos.y) / (EdgeRatio * Screen.y), 0, 1);
                push_dir.y -= Restrict((Input.CurMousePos.y - (1 - EdgeRatio) * Screen.y) / (EdgeRatio * Screen.y), 0, 1);

                CameraPos += push_dir / CameraZoom * MoveRate_PushEdge;
            }

            // Move the camera via: Keyboard or Gamepad
            var dir = Input.Direction();

            float MoveRate_Keyboard = .07f;
            CameraPos += dir / CameraZoom * MoveRate_Keyboard;


            // Make sure the camera doesn't go too far offscreen
            var TR = ScreenToWorldCoord(new vec2(Screen.x, 0));
            if (TR.x > 1)  CameraPos = new vec2(CameraPos.x - (TR.x - 1), CameraPos.y);
            if (TR.y > 1)  CameraPos = new vec2(CameraPos.x, CameraPos.y - (TR.y - 1));
            var BL = ScreenToWorldCoord(new vec2(0, Screen.y));
            if (BL.x < -1) CameraPos = new vec2(CameraPos.x - (BL.x + 1), CameraPos.y);
            if (BL.y < -1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (BL.y + 1));


			base.Update(gameTime);
		}

        const double DelayBetweenUpdates = .3333;
        //const double DelayBetweenUpdates = 5;
		double SecondsSinceLastUpdate = DelayBetweenUpdates;
		public static float PercentSimStepComplete = 0;

        int DrawCount = 0;

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
            DrawCount++;

			//if (CurKeyboard.IsKeyDown(Keys.Enter))
			SecondsSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;

			// Render setup
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            PathUpdate();
            Count();
            Bounds();
            SelectionUpdate();

			// Check if we need to do a simulation update
            if (UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates)
            //if (SecondsSinceLastUpdate > DelayBetweenUpdates)
            {
                SecondsSinceLastUpdate -= DelayBetweenUpdates;

                SimulationUpdate();
            }

            //DrawPrecomputation_Pre.Apply(Current, Previous, Output: DrawPrevious);
            //DrawPrecomputation_Cur.Apply(Current, Previous, Output: DrawCurrent);

            BenchmarkTests.Run(CurrentData, PreviousData);

			// Choose units texture
            Texture2D UnitsSpriteSheet = null, BuildingsSpriteSheet = null;
            float z = 14;
            if (CameraZoom > z)
            {
                BuildingsSpriteSheet = BuildingTexture_1;
                UnitsSpriteSheet = UnitTexture_1;
            }
            else if (CameraZoom > z / 2)
            {
                BuildingsSpriteSheet = BuildingTexture_1;
                UnitsSpriteSheet = UnitTexture_2;
            }
            else if (CameraZoom > z / 4)
            {
                BuildingsSpriteSheet = BuildingTexture_1;
                UnitsSpriteSheet = UnitTexture_4;
            }
            else if (CameraZoom > z / 8)
            {
                BuildingsSpriteSheet = BuildingTexture_1;
                UnitsSpriteSheet = UnitTexture_8;
            }
            else
            {
                BuildingsSpriteSheet = BuildingTexture_1;
                UnitsSpriteSheet = UnitTexture_16;
            }

			// Draw texture to screen
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);

			PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            DrawGrass.Using(camvec, CameraAspect, GroundTexture);
            Ground.Draw(GraphicsDevice);

            DrawCorpses.Using(camvec, CameraAspect, Corspes, UnitsSpriteSheet);
            GridHelper.DrawGrid();

            DrawBuildings.Using(camvec, CameraAspect, CurrentData, CurrentUnits, BuildingsSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();

            if (CameraZoom > z / 8)
                DrawUnits.Using(camvec, CameraAspect, CurrentData, PreviousData, CurrentUnits, PreviousUnits, UnitsSpriteSheet, PercentSimStepComplete);
            else
                DrawUnitsZoomedOut.Using(camvec, CameraAspect, CurrentData, PreviousData, UnitsSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();


            if (MouseEnabled)
            {
                if (true)
                {
                    vec2 GridCord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

                    int _w = 3, _h = 3;
                    var data = CurrentData.GetData(GridCord, new vec2(3, 3));

                    color clr = color.TransparentBlack;
                    if (data != null)
                    {
			            for (int i = 0; i < _w; i++)
			            for (int j = 0; j < _h; j++)
                        {
                            var val = data[i + j * _w];
                            clr = val.R > 0 ? new color(.7f, .2f, .2f, .8f) : new color(.2f, .7f, .2f, .8f);
                            DrawSolid.Using(camvec, CameraAspect, clr);

                            vec2 WorldCord = GridToScreenCoord(new vec2((float)Math.Floor(GridCord.x + i), (float)Math.Floor(GridCord.y + j)));
                            vec2 size = 1 / GridSize;
                            RectangleQuad.Draw(GraphicsDevice, WorldCord + new vec2(size.x, -size.y), size);
                        }
                    }
                }

                if (true)
                {
                    vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
                    DrawMouse.Using(camvec, CameraAspect, Cursor);

                    vec2 size = .02f * Cursor.UnitSize() / CameraZoom;
                    RectangleQuad.Draw(GraphicsDevice, WorldCord + new vec2(size.x, -size.y), size);
                }

                if (true)
                {
                    vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
                    DrawMouse.Using(camvec, CameraAspect, SelectCircle);
                    RectangleQuad.Draw(GraphicsDevice, WorldCord, .2f * vec2.Ones / CameraZoom);
                }
            }

			base.Draw(gameTime);
		}

        vec2 ScreenToGridCoord(vec2 pos)
        {
            var world = ScreenToWorldCoord(pos);
            world.y = -world.y;

            var grid_coord = Screen * (world + vec2.Ones) / 2;

            return grid_coord;
        }

        vec2 GridToScreenCoord(vec2 pos)
        {
            pos = 2 * pos / Screen - vec2.Ones;
            pos.y = -pos.y;
            return pos;
        }

        vec2 ScreenToWorldCoord(vec2 pos)
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

        Color[] CountData = new Color[1];
        int UnitCount = 0, SelectedCount = 0;
        void Count()
        {
            Counting.Apply(CurrentData, Output: Multigrid[1]);

            int n = ((int)Screen.x) / 2;
            int level = 1;
            while (n >= 2)
            {
                _Counting.Apply(Multigrid[level], Output: Multigrid[level + 1]);

                n /= 2;
                level++;
            }
            GraphicsDevice.SetRenderTarget(null);

            Multigrid.Last().GetData(CountData);
            color count = (color)CountData[0];

            var unpacked = SimShader.unpack_vec2(count);
            UnitCount     = (int)Math.Round(unpacked.x);
            SelectedCount = (int)Math.Round(unpacked.y);
            Console.WriteLine("Selected {0} / {1}", SelectedCount, UnitCount);
        }

        vec2 SelectedBound_BL, SelectedBound_TR;
        void Bounds()
        {
            Bounding.Apply(CurrentData, Output: Multigrid[1]);

            int n = ((int)Screen.x) / 2;
            int level = 1;
            while (n >= 2)
            {
                _Bounding.Apply(Multigrid[level], Output: Multigrid[level + 1]);

                n /= 2;
                level++;
            }
            GraphicsDevice.SetRenderTarget(null);

            Multigrid.Last().GetData(CountData);
            color bound = (color)CountData[0];

            SelectedBound_TR = bound.rg;
            SelectedBound_BL = bound.ba;

            Console.WriteLine("Bounds: ({0}), ({1})", SelectedBound_BL, SelectedBound_TR);
        }

        void PathUpdate()
        {
            Pathfinding_ToOtherTeams.Apply(PathToOtherTeams, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref PathToOtherTeams, ref Temp1);

            //Pathfinding_Right.Apply(Paths_Right, Current, Output: Temp1);
            //Swap(ref Paths_Right, ref Temp1);

            //Pathfinding_Left.Apply(Paths_Left, Current, Output: Temp1);
            //Swap(ref Paths_Left, ref Temp1);

            //Pathfinding_Up.Apply(Paths_Up, Current, Output: Temp1);
            //Swap(ref Paths_Up, ref Temp1);

            //Pathfinding_Down.Apply(Paths_Down, Current, Output: Temp1);
            //Swap(ref Paths_Down, ref Temp1);
        }

        void SelectionUpdate()
        {
            vec2 WorldCord     = ScreenToWorldCoord(Input.CurMousePos);
            vec2 WorldCordPrev = ScreenToWorldCoord(Input.PrevMousePos);


            //vec2 GridCord = ScreenToGridCoord(Input.CurMousePos);
            //vec2 size = 2 * (1 / GridSize);
            //vec2 bl = GridCord * size - vec2.Ones;
            //RectangleQuad q = new RectangleQuad(bl, bl + size, 
            //WorldCord = GridToScreenCoord(new vec2((float)Math.Floor(GridCord.x), (float)Math.Floor(GridCord.y)));
            //DrawSolid.Using(camvec, CameraAspect, new color(.2f, .7f, .2f, .8f));

            //vec2 size = 1 / GridSize;
            //RectangleQuad.Draw(GraphicsDevice, WorldCord + new vec2(size.x, -size.y), size);

            
            
            //return;

            bool Deselect  = Input.LeftMousePressed && !Keys.LeftShift.Pressed() && !Keys.RightShift.Pressed();
            bool Selecting = Input.LeftMouseDown;

            DataDrawMouse.Using(SelectCircle_Data, SimShader.Player.One, Output: SelectField, Clear: Color.Transparent);

            if (Selecting)
            {
                vec2 shift = new vec2(1 / Screen.x, -1 / Screen.y);

                for (int i = 0; i <= 10; i++)
                {
                    float t = i / 10.0f;
                    var pos = t * WorldCordPrev + (1-t) * WorldCord;
                    RectangleQuad.Draw(GraphicsDevice, pos - shift, vec2.Ones * .2f / CameraZoom);
                }
            }

            var action = Input.RightMousePressed ? SimShader.UnitAction.Attacking : SimShader.UnitAction.NoChange;
            ActionSelect.Apply(CurrentData, CurrentUnits, SelectField, Deselect, action, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);

            ActionSelect.Apply(PreviousData, CurrentUnits, SelectField, Deselect, SimShader.UnitAction.NoChange, Output: Temp1);
            Swap(ref Temp1, ref PreviousData);

            if (Keys.F.Pressed() || Keys.G.Pressed())
            {
                CreateUnits();
            }

            if (Input.RightMousePressed)
            {
                AttackMove();
            }
        }

        private void CreateUnits()
        {
            float player = Keys.F.Pressed() ? SimShader.Player.One : SimShader.Player.Two;
            float team = Keys.F.Pressed() ? SimShader.Team.One : SimShader.Team.Two;

            ActionSpawn_Unit.Apply(CurrentUnits, SelectField, player, team, Output: Temp1);
            Swap(ref Temp1, ref CurrentUnits);
            ActionSpawn_Data.Apply(CurrentData, SelectField, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);
        }

        private void AttackMove()
        {
            var pos = ScreenToGridCoord(Input.CurMousePos);
            vec2 shift = new vec2(1 / Screen.x, -1 / Screen.y);
            pos -= shift;

            vec2 Selected_BL = SelectedBound_BL * Screen;
            vec2 Selected_Size = (SelectedBound_TR - SelectedBound_BL) * Screen;
            if (Selected_Size.x < 1) Selected_Size.x = 1;
            if (Selected_Size.y < 1) Selected_Size.y = 1;

            float SquareWidth = (float)Math.Sqrt(SelectedCount);
            vec2 Destination_Size = new vec2(SquareWidth, SquareWidth) * 1.25f;
            vec2 Destination_BL = pos - Destination_Size / 2;

            ActionAttackSquare.Apply(CurrentData, TargetData, Destination_BL, Destination_Size, Selected_BL, Selected_Size, Output: Temp1);
            //ActionAttackPoint .Apply(Current, TargetData, pos, Output: Temp1);
            Swap(ref TargetData, ref Temp1);

            ActionAttack2.Apply(CurrentData, Extra, pos, Output: Temp1);
            Swap(ref Extra, ref Temp1);
        }

		void SimulationUpdate()
		{
            PathUpdate();

            Building_SelectCenterIfSelected_SetDirecion.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Target.Apply(CurrentUnits, CurrentData, TargetData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);

            AddCorpses.Apply(CurrentUnits, CurrentData, Corspes, Output: Temp1);
            Swap(ref Corspes, ref Temp1);

            Movement_UpdateDirection_RemoveDead.Apply(TargetData, CurrentUnits, Extra, CurrentData, PathToOtherTeams, Output: Temp1);
            //Movement_UpdateDirection.Apply(TargetData, CurData, Current, Paths_Right, Paths_Left, Paths_Up, Paths_Down, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            Movement_Phase1.Apply(CurrentData, Output: Temp1);
            Movement_Phase2.Apply(CurrentData, Temp1, Output: Temp2);

            Swap(ref CurrentData, ref PreviousData);
            Swap(ref Temp2, ref CurrentData);

            Movement_Convect.Apply(TargetData, CurrentData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            Movement_Convect.Apply(Extra, CurrentData, Output: Temp1);
            Swap(ref Extra, ref Temp1);
            Movement_Convect.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            Swap(ref PreviousUnits, ref Temp1);

            CheckForAttacking.Apply(CurrentUnits, CurrentData, RandomField, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);

            SpawnUnits.Apply(CurrentUnits, CurrentData, PreviousData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            SetSpawn_Unit.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            SetSpawn_Target.Apply(TargetData, CurrentData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            SetSpawn_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            UpdateRandomField.Apply(RandomField, Output: Temp1);
            Swap(ref RandomField, ref Temp1);
		}
	}
}
