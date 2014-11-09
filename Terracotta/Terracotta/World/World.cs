using FragSharpFramework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terracotta
{
    public partial class World : SimShader
    {
        //Migrate.Apply(World.DataGroup.CurrentUnits, Output: World.DataGroup.Temp1);
        //CoreMath.Swap(ref World.DataGroup.Temp1, ref World.DataGroup.CurrentUnits);

        //Migrate.Apply(World.DataGroup.PreviousUnits, Output: World.DataGroup.Temp1);
        //CoreMath.Swap(ref World.DataGroup.Temp1, ref World.DataGroup.PreviousUnits);


        public World()
        {
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

        //const double DelayBetweenUpdates = .01;
        const double DelayBetweenUpdates = .3333;
        //const double DelayBetweenUpdates = .4;

        public bool MapEditor = false;
        bool DrawGridLines = false;
        
        public bool SimulationPaused = false;
        public bool WorldPaused = false;

        bool NotPaused_SimulationUpdate { get { return !SimulationPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused
        bool NotPaused_UnitOrders { get { return !SimulationPaused || MapEditor && !WorldPaused; } } // Allow unit orders even if simulation is paused, as long as we're in the map editor and the world isn't paused

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

        public RenderTarget2D Minimap;

        double SecondsSinceLastUpdate = DelayBetweenUpdates;
        float PercentSimStepComplete = 0;

        int DrawCount = 0;

        float PlayerValue = Player.One;
        int PlayerNumber { get { return Int(PlayerValue); } }

        float TeamValue = Team.One;
        int TeamNumber { get { return Int(TeamValue); } }

        enum UserMode { PlaceBuilding, Select, CastSpell, };
        UserMode CurUserMode = UserMode.Select;
        float BuildingType = UnitType.GoldMine;
        bool UnselectAll = false;

        enum Spell { None, Fireball, RaiseSkeletons, SummonNecromancer, RaiseTerracotta, Convert, Flamewall, Resurrect, CorpseExplode, }
        Spell CurSpell = Spell.None;

        bool CanPlaceBuilding = false;
        bool[] CanPlace = new bool[3 * 3];

        vec2 SelectSize
        {
            get
            {
                vec2 Scaled = vec2.Ones / CameraZoom;
                vec2 cell_size = (1 / DataGroup.GridSize);

                if (CurUserMode == UserMode.Select) return .2f * Scaled;

                if (CurUserMode == UserMode.CastSpell)
                {
                    switch (CurSpell)
                    { 
                        case Spell.Fireball:
                            return 30 * cell_size;
                    }
                }

                return .2f * Scaled;
            }
        }
    }
}
