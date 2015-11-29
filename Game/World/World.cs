using System;

using FragSharpFramework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    public partial class World : SimShader
    {
        public int Guid = 0;

        public World(bool Skeleton = false, GameParameters GameParams = null, bool RemoveComputerDragonLords = false)
        {
            Guid = new Random().Next();

            this.RemoveComputerDragonLords = RemoveComputerDragonLords;

            MyPlayerNumber = Program.StartupPlayerNumber;
            PlayerTeams = Program.Teams;
            PlayerTeamVals = vec(Team.Vals[PlayerTeams[1]], Team.Vals[PlayerTeams[2]], Team.Vals[PlayerTeams[3]], Team.Vals[PlayerTeams[4]]);
            MyTeamNumber = PlayerTeams[MyPlayerNumber];

            CameraAspect = GameClass.ScreenAspect;

            if (GameParams != null)
            {
                Params = GameParams;
            }
            else
            {
                Params = new GameParameters();
            }

            TeamInfo = new TeamInfo[5];
            PlayerInfo = new PlayerInfo[5];
            for (int i = 1; i <= 4; i++)
            {
                TeamInfo[i] = new TeamInfo();
                PlayerInfo[i] = new PlayerInfo(i, Params);
            }

            if (Skeleton) return;

            int GridN = 1024;
            DataGroup = new DataGroup(GridN, GridN);

            Markers = new MarkerList();
            UserMessages = new UserMessageList();

            Minimap = new RenderTarget2D(GameClass.Game.GraphicsDevice, 256, 256);
        }

        //const double DelayBetweenUpdates = .01; // Super fast
        const double DelayBetweenUpdates = .3333; // Normal speed
        //const double DelayBetweenUpdates = 3; // Super slow

        bool RemoveComputerDragonLords;

        bool _MapEditor = false;
        public bool MapEditor
        {
            get
            {
                return _MapEditor;
            }

            set
            {
                _MapEditor = value;
                MapEditorActive = value;
            }
        }
        
        bool _MapEditorActive = false;
        public bool MapEditorActive
        {
            get
            {
                return MapEditor && _MapEditorActive;
            }

            set
            {
                _MapEditorActive = value;
                SimulationPaused = MapEditorActive;
            }
        }
        bool DrawGridLines = false;
        
        public bool SimulationPaused = false;
        public bool WorldPaused = false;
        public bool ServerPaused = false;
        public bool DesyncPause = false;

        bool NotPaused_SimulationUpdate { get { return !SimulationPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused
        bool NotPaused_UnitOrders { get { return !SimulationPaused || MapEditorActive && !WorldPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused

        /// <summary>
        /// If this is a map editor then the current player is "None", so anything can be selected.
        /// Otherwise this returns the player value of this client.
        /// </summary>
        public float PlayerOrNeutral { get { return MapEditorActive ? Player.None : MyPlayerValue; } }

        public vec2 CameraPos = vec2.Zero;
        public float CameraZoom = 30;
        public float CameraAspect = 1;
        public vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

        public DataGroup DataGroup;

        public GameParameters Params;
        public TeamInfo[] TeamInfo;
        public PlayerInfo[] PlayerInfo;

        public MarkerList Markers;
        UserMessageList UserMessages;

        public RenderTarget2D Minimap;
        public RectangleQuad MinimapQuad = new RectangleQuad();

        public float T = 0;
        double SecondsSinceLastUpdate = DelayBetweenUpdates;
        float PercentSimStepComplete = 0;

        int DrawCount = 0;

        public int[]
            PlayerTeams = new int[] { -1, 1, 2, 3, 4 };

        public PlayerTuple PlayerTeamVals;

        public float MyPlayerValue = Player.One;
        public int MyPlayerNumber
        {
            get
            {
                return Int(MyPlayerValue);
            }

            set
            {
                if (value < 0)
                    MyPlayerValue = 0;
                else
                    MyPlayerValue = Player.Vals[value];
            }
        }

        public float MyTeamValue = Team.One;
        public int MyTeamNumber
        {
            get
            {
                return Int(MyTeamValue);
            }

            set
            {
                if (value < 0)
                    MyTeamValue = Team.None;
                else
                    MyTeamValue = Team.Vals[value];
            }
        }

        public PlayerInfo MyPlayerInfo
        {
            get
            {
                if (MyPlayerNumber <= 0) return null;

                return PlayerInfo[MyPlayerNumber];
            }
        }

        public TeamInfo MyTeamInfo
        {
            get
            {
                return TeamInfo[MyTeamNumber];
            }
        }

        enum UserMode { PlaceBuilding, Painting, Select, CastSpell };
        UserMode CurUserMode = UserMode.Select;
        float CurSelectionFilter = SelectionFilter.All;
        float BuildingUserIsPlacing = UnitType.GoldMine;
        float UnitUserIsPlacing = UnitType.Footman;
        float TileUserIsPlacing = TileType.Grass;
        public float UnitPlaceStyle = UnitDistribution.EveryOther;
        bool UnselectAll = false;
        bool SkipNextSelectionUpdate = false;

        Spell CurSpell = null;

        bool CanPlaceItem = false;
        bool[] CanPlace = new bool[3 * 3];

        public vec2 GridSize { get { return DataGroup.GridSize; } }
        public vec2 CellSize { get { return DataGroup.CellSpacing; } }
        public vec2 Scaled { get { return vec2.Ones / CameraZoom; } }

        vec2 SelectSize
        {
            get
            {
                return .2f * Scaled;
            }
        }

        public bool End_PlayedDeathGroan, End_PlayedDeathExplosion;
    }
}
