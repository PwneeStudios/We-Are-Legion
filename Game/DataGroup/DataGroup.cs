using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public class Create : SimShader
    {
        public static Random rnd = new Random();

        public static void MakeBuilding(vec2 coord, float type, float player, float team, int i, int j, int GridWidth, int GridHeight, Color[] Units, Color[] Data, Color[] TargetData)
        {
            for (int _i = i; _i < i + 3; _i++)
            for (int _j = j; _j < j + 3; _j++)
            {
                Units[_i * GridHeight + _j] = (Color) new unit(type, player, team, 0);
                Data[_i * GridHeight + _j]  = (Color) new data(Dir.Stationary, _[_j - j], 0, _[_i - i]);

                // Set the target vector to the provided coordinate.
                vec4 packed_coord = pack_vec2(coord);
                TargetData[_i * GridHeight + _j] = (Color)packed_coord;

                // Randomize the target vector for the buildings tiles.
                // This makes units coming from this building head in random directions.
                // Otherwise units will naturally pile up around the building until an order is given.
                //TargetData[_i * GridHeight + _j] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
            }
        }

        public static void PlaceBuilding(DataGroup d, vec2 coord, float building_type, float player, float team)
        {
            MakeBuilding(coord, building_type, player, team, 0, 0, 3, 3, _unit, _data, _target);

            vec2 size = new vec2(3, 3);
            d.CurrentUnits.SetData(coord, size, _unit);
            d.CurrentData.SetData(coord, size, _data);
            d.TargetData.SetData(coord, size, _target);
        }

        public static void MakeUnit(float type, float player, float team, int i, int j, int GridWidth, int GridHeight, Color[] Units, Color[] Data, Color[] TargetData)
        {
            Units[0] = (Color)new unit(type, player, team, 0);
            var d = new data(Dir.Up, Change.Stayed, 0, UnitAction.Guard);
            set_prior_direction(ref d, Dir.Down);
            set_selected(ref d, false);
            Data[0] = (Color)d;

            TargetData[0] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
        }

        public static void PlaceUnit(DataGroup d, vec2 coord, float unit_type, float player, float team, bool SetPrevious = false)
        {
            MakeUnit(unit_type, player, team, 0, 0, 1, 1, _unit, _data, _target);

            vec2 size = new vec2(1, 1);
            d.CurrentUnits.SetData(coord, size, _unit);
            d.CurrentData.SetData(coord, size, _data);
            d.TargetData.SetData(coord, size, _target);

            if (SetPrevious)
            {
                d.PreviousUnits.SetData(coord, size, _unit);
                d.PreviousData.SetData(coord, size, _data);                
            }
        }

        /// <summary>
        /// Scratch space.
        /// </summary>
        static Color[]
            _unit = new Color[3 * 3],
            _data = new Color[3 * 3],
            _target = new Color[3 * 3];
    }

    public partial class DataGroup : SimShader
    {
        public readonly int w, h;
        public readonly vec2 GridSize, CellSpacing;

        GraphicsDevice GraphicsDevice { get { return GameClass.Game.GraphicsDevice; } }

        public DataGroup(int w, int h)
        {
            this.w = w;
            this.h = h;
            GridSize = new vec2(w, h);
            CellSpacing = 1 / GridSize;

            Initialize();
        }

        public void Initialize()
        {
            CreateRenderTargets(true);
            InitialConditions();
        }

        public RenderTarget2D
            HashField,

            Temp1, Temp2,
            PreviousUnits, CurrentUnits, PreviousData, CurrentData, Extra, TargetData,
            RandomField,
            Tiles, Corpses, Magic, Necromancy, AntiMagic,
            SelectField,

            DistanceToPlayers, DistanceToOtherTeams, DistanceToBuildings,

            Geo, AntiGeo, OuterGeo, TempGeo, GeoInfo,
            ShiftedGeo, ShiftedGeoInfo,
            MockTiles;

        public Dictionary<float, RenderTarget2D>
            Dirward = new Dictionary<float, RenderTarget2D>();
        
        public List<RenderTarget2D>
            Multigrid;

        void CreateRenderTargets(bool Editor)
        {
            CreateRenderTargets_InGameRequired();
            
            if (Editor) CreateRenderTargets_EditorRequired();
        }

        void CreateRenderTargets_InGameRequired()
        {
            MakeHashField();

            CurrentUnits = MakeTarget(w, h);
            PreviousUnits = MakeTarget(w, h);

            CurrentData = MakeTarget(w, h);
            PreviousData = MakeTarget(w, h);

            Extra = MakeTarget(w, h);
            TargetData = MakeTarget(w, h);

            Tiles = MakeTarget(w, h);
            Corpses = MakeTarget(w, h);
            Magic = MakeTarget(w, h);
            Necromancy = MakeTarget(w, h);
            AntiMagic = MakeTarget(w, h);

            SelectField = MakeTarget(w, h);

            RandomField = MakeTarget(w, h);

            Temp1 = MakeTarget(w, h);
            Temp2 = MakeTarget(w, h);

            DistanceToPlayers = MakeTarget(w, h);
            DistanceToOtherTeams = MakeTarget(w, h);
            DistanceToBuildings = MakeTarget(w, h);

            Geo = MakeTarget(w, h);
            AntiGeo = MakeTarget(w, h);
            
            foreach (float dir in Dir.Vals)
                Dirward.Add(dir, MakeTarget(w, h));

            Multigrid = new List<RenderTarget2D>();
            int n = w;
            while (n >= 1)
            {
                Multigrid.Add(MakeTarget(n, n));
                n /= 2;
            }
        }
        
        void CreateRenderTargets_EditorRequired()
        {
            OuterGeo = MakeTarget(w, h);
            TempGeo = MakeTarget(w, h);
            GeoInfo = MakeTarget(w, h);
            ShiftedGeo = MakeTarget(w, h);
            ShiftedGeoInfo = MakeTarget(w, h);
            MockTiles = MakeTarget(w, h);
        }

        RenderTarget2D MakeTarget()
        {
            return new RenderTarget2D(GameClass.Game.GraphicsDevice, w, h);
        }

        RenderTarget2D MakeTarget(int w, int h)
        {
            return new RenderTarget2D(GameClass.Game.GraphicsDevice, w, h);
            //return new RenderTarget2D(GameClass.Game.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None, 1, RenderTargetUsage.PreserveContents);
        }
    }
}
