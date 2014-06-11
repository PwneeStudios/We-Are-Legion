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
    public class GameParameters : SimShader
    {
        public int
            BarracksCost = 2500,
            GoldMineCost = 5000,

            StartGold = 7500;

        public int BuildingCost(float type)
        {
            if (type == UnitType.Barracks) return BarracksCost;
            if (type == UnitType.GoldMine) return GoldMineCost;
            return int.MaxValue;
        }
    }

    public class PlayerInfo
    {
        public int
            Gold = 0,
            GoldMines = 0;

        public PlayerInfo(GameParameters Params)
        {
            Gold = Params.StartGold;
        }
    }

    public class MarkerList
    {
        List<Marker> Markers = new List<Marker>();

        public void Add(Marker marker)
        {
            Markers.Add(marker);
        }

        public void Draw()
        {
            foreach (var marker in Markers) marker.Draw();
        }

        public void Update()
        {
            foreach (var marker in Markers) marker.Update();
            Markers.RemoveAll(marker => marker.alpha <= 0);
        }
    }

    public class Marker
    {
        public float alpha;
        float alpha_fade;

        RectangleQuad quad;
        Texture2D texture;

        public Marker(vec2 pos, vec2 size, Texture2D texture, float alpha_fade)
        {
            alpha = 1;
            this.alpha_fade = alpha_fade;

            quad = new RectangleQuad(pos - size / 2, pos + size / 2, vec2.Zero, vec2.Ones);
            this.texture = texture;
        }

        public void Draw()
        {
            DrawTexture.Using(GameClass.Game.camvec, GameClass.Game.CameraAspect, texture);
            quad.SetColor(new color(1, 1, 1, alpha));
            quad.Draw(GameClass.Game.GraphicsDevice);
        }

        public void Update()
        {
            alpha += (float)GameClass.Time.ElapsedGameTime.TotalSeconds * alpha_fade;
        }
    }

	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GameClass : Game
	{
        public static GameClass Game;
        public static GameTime Time;

		const bool UnlimitedSpeed = false;

        const bool MouseEnabled = true;

		public vec2 CameraPos = vec2.Zero;
		public float CameraZoom = 30;
        public float CameraAspect = 1;
        public vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

		GraphicsDeviceManager graphics;
        
        SpriteBatch MySpriteBatch;
        SpriteFont DefaultFont;

        DataGroup DataGroup;
        GameParameters Params;
        PlayerInfo[] PlayerInfo;

        MarkerList Markers;

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

            CameraAspect = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;
            
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
        RectangleQuad Ground;

        Effect test;
		protected override void Initialize()
		{
            //test = Content.Load<Effect>("Shaders/__Test");
            //test = Content.Load<Effect>("Shaders/Counting");
            //return;

            MySpriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultFont = Content.Load<SpriteFont>("Default");

            FragSharp.Initialize(Content, GraphicsDevice);
            GridHelper.Initialize(GraphicsDevice);

            Assets.Initialize();

            float GroundRepeat = 100;
            Ground = new RectangleQuad(new vec2(-1, -1), new vec2(1, 1), new vec2(0, 0), new vec2(1, 1) * GroundRepeat);

            DataGroup = new DataGroup(1024, 1024);
            
            Params = new GameParameters();
            PlayerInfo = new PlayerInfo[5];
            for (int i = 1; i <= 4; i++)
            {
                PlayerInfo[i] = new PlayerInfo(Params);
            }

            Markers = new MarkerList();

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
            //Update();

			base.Update(gameTime);
		}

        void Update()
        {
            float FpsRateModifier = 1;

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
                float MouseWheelZoomRate = 1.3333f * FpsRateModifier;
                if (Input.DeltaMouseScroll < 0) CameraZoom /= MouseWheelZoomRate;
                else if (Input.DeltaMouseScroll > 0) CameraZoom *= MouseWheelZoomRate;
            }

            float KeyZoomRate = 1.125f * FpsRateModifier;
            if (Buttons.X.Down() || Keys.X.Pressed() || Keys.E.Pressed()) CameraZoom /= KeyZoomRate;
            else if (Buttons.A.Down() || Keys.Z.Pressed() || Keys.Q.Pressed()) CameraZoom *= KeyZoomRate;

            if (CameraZoom < MaxZoomOut) CameraZoom = MaxZoomOut;
            if (CameraZoom > MaxZoomIn) CameraZoom = MaxZoomIn;

            if (MouseEnabled && !(Buttons.A.Pressed() || Buttons.X.Pressed()))
            {
                var shifted = GetShiftedCamera(Input.CurMousePos, camvec, world_mouse_pos);
                CameraPos = shifted;
            }

            // Move the camera via: Click And Drag
            //float MoveRate_ClickAndDrag = .00165f * FpsRateModifier;
            //if (Input.LeftMouseDown)
            //    CameraPos += Input.DeltaMousPos / CameraZoom * MoveRate_ClickAndDrag * new vec2(-1, 1);

            // Move the camera via: Push Edge
            if (MouseEnabled)
            {
                float MoveRate_PushEdge = .07f * FpsRateModifier;
                var push_dir = vec2.Zero;
                float EdgeRatio = .1f;
                push_dir.x += -CoreMath.Restrict((EdgeRatio * Screen.x - Input.CurMousePos.x) / (EdgeRatio * Screen.x), 0, 1);
                push_dir.x += CoreMath.Restrict((Input.CurMousePos.x - (1 - EdgeRatio) * Screen.x) / (EdgeRatio * Screen.x), 0, 1);
                push_dir.y -= -CoreMath.Restrict((EdgeRatio * Screen.y - Input.CurMousePos.y) / (EdgeRatio * Screen.y), 0, 1);
                push_dir.y -= CoreMath.Restrict((Input.CurMousePos.y - (1 - EdgeRatio) * Screen.y) / (EdgeRatio * Screen.y), 0, 1);

                CameraPos += push_dir / CameraZoom * MoveRate_PushEdge;
            }

            // Move the camera via: Keyboard or Gamepad
            var dir = Input.Direction();

            float MoveRate_Keyboard = .07f * FpsRateModifier;
            CameraPos += dir / CameraZoom * MoveRate_Keyboard;


            // Make sure the camera doesn't go too far offscreen
            var TR = ScreenToWorldCoord(new vec2(Screen.x, 0));
            if (TR.x > 1) CameraPos = new vec2(CameraPos.x - (TR.x - 1), CameraPos.y);
            if (TR.y > 1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (TR.y - 1));
            var BL = ScreenToWorldCoord(new vec2(0, Screen.y));
            if (BL.x < -1) CameraPos = new vec2(CameraPos.x - (BL.x + 1), CameraPos.y);
            if (BL.y < -1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (BL.y + 1));


            // Switch modes
            if (Keys.B.Pressed())
            {
                CurUserMode = UserMode.PlaceBuilding;
                UnselectAll = true;
                BuildingType = SimShader.UnitType.Barracks;
            }

            if (Keys.G.Pressed())
            {
                CurUserMode = UserMode.PlaceBuilding;
                UnselectAll = true;
                BuildingType = SimShader.UnitType.GoldMine;
            }

            if (Keys.Escape.Pressed() || Keys.Back.Pressed() || CurUserMode == UserMode.PlaceBuilding && Input.RightMousePressed)
            {
                CurUserMode = UserMode.Select;
            }
        }

        const double DelayBetweenUpdates = .3333;
        //const double DelayBetweenUpdates = 5;
		double SecondsSinceLastUpdate = DelayBetweenUpdates;
		public static float PercentSimStepComplete = 0;

        int DrawCount = 0;

        float PlayerValue = SimShader.Player.Two;
        int PlayerNumber { get { return SimShader.Int(PlayerValue); } }

        float TeamValue = SimShader.Team.Two;
        int TeamNumber { get { return SimShader.Int(TeamValue); } }

        public enum UserMode { PlaceBuilding, Select };
        public UserMode CurUserMode = UserMode.PlaceBuilding;
        public float BuildingType = SimShader.UnitType.GoldMine;
        bool UnselectAll = false;

        bool CanPlaceBuilding = false;
        bool[] CanPlace = new bool[3 * 3];

        void SubtractGold(int amount, int player)
        {
            PlayerInfo[player].Gold -= amount;
        }

        bool CanAffordBuilding(float building_type, int player)
        {
            var cost = Params.BuildingCost(building_type);

            return cost <= PlayerInfo[player].Gold;
        }

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
            GameClass.Time = gameTime;

            Update();

            DrawCount++;

			//if (CurKeyboard.IsKeyDown(Keys.Enter))
			SecondsSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;

			// Render setup
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
			GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;


            switch (CurUserMode)
            {
                case UserMode.PlaceBuilding:
                    if (UnselectAll)
                    {
                        SelectionUpdate();
                        UnselectAll = false;
                    }

                    PlaceBuilding();
                    break;

                case UserMode.Select:
                    for (int i = 1; i <= 2; i++)
                    {
                        float player = SimShader.Player.Get(i);
                        DataGroup.UnitCount[i] = DataGroup.DoUnitCount(player, false);
                    }

                    DataGroup.SelectedCount = DataGroup.DoUnitCount(PlayerValue, true);
                    SelectionUpdate();
                    break;
            }

			// Check if we need to do a simulation update
            if (UnlimitedSpeed || SecondsSinceLastUpdate > DelayBetweenUpdates)
            //if (SecondsSinceLastUpdate > DelayBetweenUpdates)
            {
                SecondsSinceLastUpdate -= DelayBetweenUpdates;

                DataGroup.DoGoldMineCount(PlayerInfo);
                DoGoldUpdate();

                DataGroup.SimulationUpdate();
            }

            BenchmarkTests.Run(DataGroup.CurrentData, DataGroup.PreviousData);

			// Choose units texture
            Texture2D UnitsSpriteSheet = null, BuildingsSpriteSheet = null, ExplosionSpriteSheet = null;
            float z = 14;
            if (CameraZoom > z)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_1;
            }
            else if (CameraZoom > z / 2)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_2;
            }
            else if (CameraZoom > z / 4)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_4;
            }
            else if (CameraZoom > z / 8)
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_8;
            }
            else
            {
                BuildingsSpriteSheet = Assets.BuildingTexture_1;
                ExplosionSpriteSheet = Assets.ExplosionTexture_1;
                UnitsSpriteSheet = Assets.UnitTexture_16;
            }

			// Draw texture to screen
			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);

			PercentSimStepComplete = (float)(SecondsSinceLastUpdate / DelayBetweenUpdates);

            DrawGrass.Using(camvec, CameraAspect, Assets.GroundTexture);
            Ground.Draw(GraphicsDevice);

            if (CurUserMode == UserMode.PlaceBuilding)
            {
                DrawTerritoryPlayer.Using(camvec, CameraAspect, DataGroup.DistanceToPlayers, PlayerValue);
            }
            else if (CameraZoom <= z / 4)
            {
                float blend = CoreMath.Lerp(z / 4, 0, z / 8, 1, CameraZoom);
                DrawTerritoryColors.Using(camvec, CameraAspect, DataGroup.DistanceToPlayers, blend);
            }
            else
            {
                DrawCorpses.Using(camvec, CameraAspect, DataGroup.Corspes, UnitsSpriteSheet);
            }
            GridHelper.DrawGrid();

            if (CameraZoom <= z / 4)
            {
                float blend = CoreMath.LerpRestrict(z / 4, 0, z / 8, 1, CameraZoom);
                DrawBuildingsIcons.Using(camvec, CameraAspect, DataGroup.DistanceToBuildings, blend);
                GridHelper.DrawGrid();
            }
            DrawBuildings.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.CurrentUnits, BuildingsSpriteSheet, ExplosionSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();

            Markers.Update();
            Markers.Draw();

            if (CameraZoom > z / 8)
                DrawUnits.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, DataGroup.CurrentUnits, DataGroup.PreviousUnits, UnitsSpriteSheet, PercentSimStepComplete);
            else
                DrawUnitsZoomedOut.Using(camvec, CameraAspect, DataGroup.CurrentData, DataGroup.PreviousData, UnitsSpriteSheet, PercentSimStepComplete);
            GridHelper.DrawGrid();


            CanPlaceBuilding = false;
            if (MouseEnabled)
            {
                if (CurUserMode == UserMode.PlaceBuilding)
                {
                    DrawAvailabilityGrid();
                    DrawPotentialBuilding();
                    DrawArrowCursor();
                }

                if (CurUserMode == UserMode.Select)
                {
                    //DrawGridCell();
                    DrawCircleCursor();
                    DrawArrowCursor();
                }
            }

            var units_1 = string.Format("Player 1 {0:#,##0}", DataGroup.UnitCount[1]);
            var units_2 = string.Format("Player 2 {0:#,##0}", DataGroup.UnitCount[2]);
            var selected = string.Format("[{0:#,##0}]", DataGroup.SelectedCount);
            var gold = string.Format("Gold {0:#,##0}", PlayerInfo[PlayerNumber].Gold);
            var gold_mines = string.Format("Gold Mines {0:#,##0}", PlayerInfo[PlayerNumber].GoldMines);
            MySpriteBatch.Begin();
            
            MySpriteBatch.DrawString(DefaultFont, units_1, new Vector2(0, 0), Color.White);
            MySpriteBatch.DrawString(DefaultFont, units_2, new Vector2(0, 20), Color.White);
            MySpriteBatch.DrawString(DefaultFont, gold, new Vector2(0, 40), Color.White);
            MySpriteBatch.DrawString(DefaultFont, gold_mines, new Vector2(0, 60), Color.White);
            
            if (CurUserMode == UserMode.Select)
                MySpriteBatch.DrawString(DefaultFont, selected, Input.CurMousePos + new vec2(30, -130), Color.White);

            MySpriteBatch.End();

			base.Draw(gameTime);
		}

        void DrawGridCell()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos);

            DrawSolid.Using(camvec, CameraAspect, DrawTerritoryPlayer.Unavailable);

            vec2 gWorldCord = GridToScreenCoord(SimShader.floor(GridCoord));
            vec2 size = 1 / DataGroup.GridSize;
            RectangleQuad.Draw(GraphicsDevice, gWorldCord + new vec2(size.x, -size.y), size);
        }

        void DrawAvailabilityGrid()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            int _w = 3, _h = 3;

            color clr = color.TransparentBlack;

            CanPlaceBuilding = true;
            for (int i = 0; i < _w; i++)
            for (int j = 0; j < _h; j++)
            {
                clr = CanPlace[i + j * _h] ? DrawTerritoryPlayer.Available : DrawTerritoryPlayer.Unavailable;
                DrawSolid.Using(camvec, CameraAspect, clr);

                vec2 gWorldCord = GridToScreenCoord(new vec2((float)Math.Floor(GridCoord.x + i), (float)Math.Floor(GridCoord.y + j)));
                vec2 size = 1 / DataGroup.GridSize;
                RectangleQuad.Draw(GraphicsDevice, gWorldCord + new vec2(size.x, -size.y), size);
            }
        }

        void DrawPotentialBuilding()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            DrawTexture.Using(camvec, CameraAspect, Assets.BuildingTexture_1);

            vec2 WorldCord = GridToScreenCoord(new vec2((float)Math.Floor(GridCoord.x), (float)Math.Floor(GridCoord.y)));
            vec2 size = 3 * 1 / DataGroup.GridSize;

            vec2 uv_size = SimShader.BuildingSpriteSheet.BuildingSize;
            vec2 uv_sheet_size = SimShader.BuildingSpriteSheet.SubsheetSize;
            vec2 uv_offset = new vec2(0, uv_sheet_size.y * ((255)*SimShader.UnitType.BuildingIndex(BuildingType)));

            var building = new RectangleQuad(WorldCord, WorldCord + 2 * new vec2(size.x, -size.y), new vec2(0, uv_size.y) + uv_offset, new vec2(uv_size.x, 0) + uv_offset);
            building.SetColor(new color(1, 1, 1, .7f));
            building.Draw(GraphicsDevice);
        }

        void DrawCircleCursor()
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTexture.Using(camvec, CameraAspect, Assets.SelectCircle);
            RectangleQuad.Draw(GraphicsDevice, WorldCord, .2f * vec2.Ones / CameraZoom);
        }

        void DrawArrowCursor()
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTexture.Using(camvec, CameraAspect, Assets.Cursor);

            vec2 size = .02f * Assets.Cursor.UnitSize() / CameraZoom;
            RectangleQuad.Draw(GraphicsDevice, WorldCord + new vec2(size.x, -size.y), size);
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

        void DoGoldUpdate()
        {
            for (int player = 1; player <= 4; player++)
            {
                PlayerInfo[player].Gold += PlayerInfo[player].GoldMines;
            }
        }

        void PlaceBuilding()
        {
            CanPlaceBuilding = false;

            if (true)
            {
                vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

                int _w = 3, _h = 3;

                UnsetDevice();

                CanPlaceBuilding = false;

                if (BuildingType == SimShader.UnitType.Barracks)
                {
                    var _data = DataGroup.CurrentData.GetData<building>(GridCoord, new vec2(_w, _h));
                    var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                    color clr = color.TransparentBlack;
                    if (_data != null)
                    {
                        CanPlaceBuilding = true;
                        for (int i = 0; i < _w; i++)
                        for (int j = 0; j < _h; j++)
                        {
                            var building_here = _data[i + j * _w];
                            var distance_to = _dist[i + j * _w];

                            var distance = SimShader.Get(distance_to, PlayerNumber);

                            bool occupied = building_here.direction > 0;
                            bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                            bool can_place = !occupied && in_territory;
                            CanPlace[i + j * _w] = can_place;

                            if (!can_place) CanPlaceBuilding = false;
                        }
                    }
                }

                if (BuildingType == SimShader.UnitType.GoldMine)
                {
                    var _data = DataGroup.CurrentUnits.GetData<unit>(GridCoord, new vec2(_w, _h));
                    var _dist = DataGroup.DistanceToPlayers.GetData<PlayerTuple>(GridCoord, new vec2(_w, _h));

                    color clr = color.TransparentBlack;
                    if (_data != null)
                    {
                        CanPlaceBuilding = true;
                        for (int i = 0; i < _w; i++)
                        for (int j = 0; j < _h; j++)
                        {
                            var unit_here = _data[i + j * _w];
                            var distance_to = _dist[i + j * _w];

                            var distance = SimShader.Get(distance_to, PlayerNumber);

                            bool is_gold_source = unit_here.team == SimShader.Team.None && unit_here.type == SimShader.UnitType.GoldSource;
                            bool in_territory = distance < DrawTerritoryPlayer.TerritoryCutoff;

                            bool can_place = is_gold_source && in_territory;
                            CanPlace[i + j * _w] = can_place;

                            if (!can_place) CanPlaceBuilding = false;
                        }
                    }
                }

                if (CanPlaceBuilding && Input.LeftMousePressed && CanAffordBuilding(BuildingType, PlayerNumber))
                {
                    try
                    {
                        Create.PlaceBuilding(DataGroup, GridCoord, BuildingType, PlayerValue, TeamValue);

                        SubtractGold(Params.BuildingCost(BuildingType), PlayerNumber);
                        CanPlaceBuilding = false;
                    }
                    catch
                    {
                    }
                }
            }
        }

        void CreateUnits()
        {
            float player = 0, team = 0;

            if (Keys.R.Pressed()) { player = SimShader.Player.One; team = SimShader.Team.One; }
            if (Keys.T.Pressed()) { player = SimShader.Player.Two; team = SimShader.Team.Two; }
            if (Keys.Y.Pressed()) { player = SimShader.Player.Three; team = SimShader.Team.Three; }
            if (Keys.U.Pressed()) { player = SimShader.Player.Four; team = SimShader.Team.Four; }

            ActionSpawn_Unit.Apply(DataGroup.CurrentUnits, DataGroup.SelectField, player, team, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);
            ActionSpawn_Data.Apply(DataGroup.CurrentData, DataGroup.SelectField, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);
        }
        
        void AddAttackMarker()
        {
            vec2 pos = ScreenToWorldCoord(Input.CurMousePos);
            vec2 cell_size = 2 * (1 / DataGroup.GridSize);
            
            float bigger = 1 / (CameraZoom / 30);
            if (bigger < 1) bigger = 1;
            vec2 size = cell_size * bigger;

            Markers.Add(new Marker(pos, size, Assets.AttackMarker, -1f));
        }

        void SelectionUpdate()
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            vec2 WorldCordPrev = ScreenToWorldCoord(Input.PrevMousePos);

            bool Deselect = Input.LeftMousePressed && !Keys.LeftShift.Pressed() && !Keys.RightShift.Pressed()
                || CurUserMode != UserMode.Select
                || Keys.Back.Pressed() || Keys.Escape.Pressed();
            bool Selecting = Input.LeftMouseDown && CurUserMode == UserMode.Select;

            vec2 size = vec2.Ones * .2f / CameraZoom;
            DataGroup.SelectAlongLine(WorldCord, WorldCordPrev, size, Deselect, Selecting, PlayerValue);

            if (CurUserMode != UserMode.Select) return;

            if (Keys.R.Pressed() || Keys.T.Pressed() || Keys.Y.Pressed() || Keys.U.Pressed())
            {
                CreateUnits();
            }

            if (Input.RightMousePressed)
            {
                AttackMove();
            }
        }

        void AttackMove()
        {
            DataGroup.SelectedUnitsBounds();

            if (DataGroup.SelectedCount == 0) return;

            vec2 pos = ScreenToGridCoord(Input.CurMousePos);

            vec2 Selected_BL = DataGroup.SelectedBound_BL;
            vec2 Selected_Size = DataGroup.SelectedBound_TR - DataGroup.SelectedBound_BL;
            if (Selected_Size.x < 1) Selected_Size.x = 1;
            if (Selected_Size.y < 1) Selected_Size.y = 1;

            float SquareWidth = (float)Math.Sqrt(DataGroup.SelectedCount);
            if (SquareWidth < 2) SquareWidth = 0;
            pos = SimShader.floor(pos);

            vec2 Destination_Size = new vec2(SquareWidth, SquareWidth) * 1.25f;
            vec2 Destination_BL = pos - Destination_Size / 2;

            Destination_Size = SimShader.floor(Destination_Size);
            Destination_BL = SimShader.floor(Destination_BL);

            DataGroup.AttackMoveApply(pos, Selected_BL, Selected_Size, Destination_Size, Destination_BL);

            AddAttackMarker();
        }



        private void UnsetDevice()
        {
            GraphicsDevice.Textures[0] = null;
            GraphicsDevice.Textures[1] = null;
            GraphicsDevice.Textures[2] = null;
            GraphicsDevice.Textures[3] = null;
            GraphicsDevice.SetRenderTarget(null);
        }
	}
}
