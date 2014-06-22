using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
        public World()
        {
            CameraAspect = GameClass.ScreenAspect;

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
            UserMessages = new UserMessageList();
        }

        const double DelayBetweenUpdates = .3333;

        public bool MapEditor = true;
        public bool SimulationPaused = false;
        public bool WorldPaused = false;

        /// <summary>
        /// If this is a map editor then the current player is "None", so anything can be selected.
        /// Otherwise this returns the player value of this client.
        /// </summary>
        public float PlayerOrNeutral { get { return MapEditor ? Player.None : PlayerValue; } }

        vec2 CameraPos = vec2.Zero;
        float CameraZoom = 30;
        public float CameraAspect = 1;
        public vec4 camvec { get { return new vec4(CameraPos.x, CameraPos.y, CameraZoom, CameraZoom); } }

        DataGroup DataGroup;

        GameParameters Params;
        PlayerInfo[] PlayerInfo;

        MarkerList Markers;
        UserMessageList UserMessages;

        RectangleQuad Ground;

        double SecondsSinceLastUpdate = DelayBetweenUpdates;
        float PercentSimStepComplete = 0;

        int DrawCount = 0;

        float PlayerValue = Player.One;
        int PlayerNumber { get { return Int(PlayerValue); } }

        float TeamValue = Team.One;
        int TeamNumber { get { return Int(TeamValue); } }

        enum UserMode { PlaceBuilding, Select };
        UserMode CurUserMode = UserMode.Select;
        float BuildingType = UnitType.GoldMine;
        bool UnselectAll = false;

        bool CanPlaceBuilding = false;
        bool[] CanPlace = new bool[3 * 3];
    }
}
