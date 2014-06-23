using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace GpuSim
{
    public class Create : SimShader
    {
        public static Random rnd = new Random();

        public static void MakeBuilding(float type, float player, float team, int i, int j, int GridWidth, int GridHeight, Color[] Units, Color[] Data, Color[] TargetData)
        {
            for (int _i = i; _i < i + 3; _i++)
            for (int _j = j; _j < j + 3; _j++)
            {
                Units[_i * GridHeight + _j] = (Color) new unit(type, player, team, 0);
                Data[_i * GridHeight + _j]  = (Color) new data(Dir.Stationary, _[_j - j], 0, _[_i - i]);
                
                TargetData[_i * GridHeight + _j] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
            }
        }

        public static void PlaceBuilding(DataGroup d, vec2 coord, float building_type, float player, float team)
        {
            MakeBuilding(building_type, player, team, 0, 0, 3, 3, _unit, _data, _target);

            vec2 size = new vec2(3, 3);
            d.CurrentUnits.SetData(coord, size, _unit);
            d.CurrentData.SetData(coord, size, _data);
            d.TargetData.SetData(coord, size, _target);
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
        public readonly vec2 GridSize, CellSize;

        GraphicsDevice GraphicsDevice { get { return GameClass.Game.GraphicsDevice; } }

        public DataGroup(int w, int h)
        {
            this.w = w;
            this.h = h;
            GridSize = new vec2(w, h);
            CellSize = 1 / GridSize;

            Initialize();
        }

        public void Initialize()
        {
            CreateRenderTargets();
            InitialConditions();
        }

        public RenderTarget2D
            Temp1, Temp2,
            PreviousUnits, CurrentUnits, PreviousData, CurrentData, Extra, TargetData,
            RandomField,
            Tiles, Corspes,
            SelectField,
            PreviousDraw, CurrentDraw,
            Paths_Right, Paths_Left, Paths_Up, Paths_Down,
            DistanceToPlayers, DistanceToOtherTeams, DistanceToBuildings;
        
        public List<RenderTarget2D>
            Multigrid;

        void CreateRenderTargets()
        {
            CurrentUnits = MakeTarget(w, h);
            PreviousUnits = MakeTarget(w, h);

            CurrentData = MakeTarget(w, h);
            PreviousData = MakeTarget(w, h);

            Extra = MakeTarget(w, h);
            TargetData = MakeTarget(w, h);

            Tiles = MakeTarget(w, h);
            Corspes = MakeTarget(w, h);

            SelectField = MakeTarget(w, h);

            RandomField = MakeTarget(w, h);

            CurrentDraw = MakeTarget(w, h);
            PreviousDraw = MakeTarget(w, h);

            Temp1 = MakeTarget(w, h);
            Temp2 = MakeTarget(w, h);

            Paths_Right = MakeTarget(w, h);
            Paths_Left = MakeTarget(w, h);
            Paths_Up = MakeTarget(w, h);
            Paths_Down = MakeTarget(w, h);

            DistanceToPlayers = MakeTarget(w, h);
            DistanceToOtherTeams = MakeTarget(w, h);
            DistanceToBuildings = MakeTarget(w, h);

            Multigrid = new List<RenderTarget2D>();
            int n = w;
            while (n >= 1)
            {
                Multigrid.Add(MakeTarget(n, n));
                n /= 2;
            }
        }

        RenderTarget2D MakeTarget()
        {
            return new RenderTarget2D(GameClass.Game.GraphicsDevice, w, h);
        }

        RenderTarget2D MakeTarget(int w, int h)
        {
            return new RenderTarget2D(GameClass.Game.GraphicsDevice, w, h);
        }
    }
}
