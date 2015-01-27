using FragSharpFramework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terracotta
{
    public partial class World : SimShader
    {
        public World()
        {
            MyPlayerNumber = Program.StartupPlayerNumber;
            PlayerTeams = Program.Teams;
            PlayerTeamVals = vec(Team.Vals[PlayerTeams[1]], Team.Vals[PlayerTeams[2]], Team.Vals[PlayerTeams[3]], Team.Vals[PlayerTeams[4]]);
            MyTeamNumber = PlayerTeams[MyPlayerNumber];

            CameraAspect = GameClass.ScreenAspect;

            float GroundRepeat = 100;
            Ground = new RectangleQuad(new vec2(-1, -1), new vec2(1, 1), new vec2(0, 0), new vec2(1, 1) * GroundRepeat);

            //DataGroup = new DataGroup(512, 512);
            DataGroup = new DataGroup(1024, 1024);
            //DataGroup = new DataGroup(2048, 2048);
            //DataGroup = new DataGroup(4096, 4096);

            Params = new GameParameters();
            PlayerInfo = new PlayerInfo[5];
            for (int i = 1; i <= 4; i++)
            {
                PlayerInfo[i] = new PlayerInfo(Params);
            }

            Markers = new MarkerList();
            UserMessages = new UserMessageList();

            Minimap = new RenderTarget2D(GameClass.Game.GraphicsDevice, 256, 256);
        }

        //const double DelayBetweenUpdates = .01; // Super fast
        const double DelayBetweenUpdates = .3333; // Normal speed
        //const double DelayBetweenUpdates = 3; // Super slow

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

        bool NotPaused_SimulationUpdate { get { return !SimulationPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused
        bool NotPaused_UnitOrders { get { return !SimulationPaused || MapEditorActive && !WorldPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused

        /// <summary>
        /// If this is a map editor then the current player is "None", so anything can be selected.
        /// Otherwise this returns the player value of this client.
        /// </summary>
        public float PlayerOrNeutral { get { return MapEditorActive ? Player.None : MyPlayerValue; } }

        vec2 CameraPos = vec2.Zero;
        float CameraZoom = 30;
        public float CameraAspect = 1;
        public vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

        public DataGroup DataGroup;

        GameParameters Params;
        public PlayerInfo[] PlayerInfo;

        MarkerList Markers;
        UserMessageList UserMessages;

        RectangleQuad Ground;

        public RenderTarget2D Minimap;

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
                MyTeamValue = Team.Vals[value];
            }
        }

        PlayerInfo MyPlayerInfo
        {
            get
            {
                return PlayerInfo[MyPlayerNumber];
            }
        }

        enum UserMode { PlaceBuilding, PlaceUnits, Select, CastSpell, };
        UserMode CurUserMode = UserMode.Select;
        enum SelectionFilter { All, Units, Buildings, Soldiers, Special, Count };
        SelectionFilter CurSelectionFilter = SelectionFilter.All;
        float BuildingUserIsPlacing = UnitType.GoldMine;
        float UnitUserIsPlacing = UnitType.Footman;
        float UnitPlaceStyle = UnitDistribution.EveryOther;
        bool UnselectAll = false;

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
    }
}
