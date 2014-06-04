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
                
                //TargetData[_i * GridHeight + _j] = new Color(0, 0, 0, 0);
                TargetData[_i * GridHeight + _j] = new Color(rnd.Next(0, 4), rnd.Next(0, 256), rnd.Next(0, 4), rnd.Next(0, 256));
            }
        }

        public static void PlaceBuilding(DataGroup d, vec2 coord, float building_type)
        {
            MakeBuilding(building_type, Player.One, Team.One, 0, 0, 3, 3, _unit, _data, _target);

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

    public class DataGroup : SimShader
    {
        public readonly int w, h;
        public readonly vec2 GridSize;

        public DataGroup(int w, int h)
        {
            this.w = w;
            this.h = h;
            GridSize = new vec2(w, h);

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
            Corspes,
            SelectField,
            PreviousDraw, CurrentDraw,
            Paths_Right, Paths_Left, Paths_Up, Paths_Down,
            PathToOtherTeams;
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

            PathToOtherTeams = MakeTarget(w, h);

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
            return new RenderTarget2D(M3ngineGame.Game.GraphicsDevice, w, h);
        }

        RenderTarget2D MakeTarget(int w, int h)
        {
            return new RenderTarget2D(M3ngineGame.Game.GraphicsDevice, w, h);
        }

        void InitialConditions()
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

                if (false)
                //if (rnd.NextDouble() > 0.85f)
                {
                    int dir = rnd.Next(1, 5);

                    int action = (int)(255f * SimShader.UnitAction.Attacking);

                    int g = 0;
                    int b = 0;

                    int player = rnd.Next(1, 2);
                    int team = player;
                    int type = rnd.Next(1, 2);

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
                Create.MakeBuilding(SimShader.UnitType.GoldSource, Player.None, Team.None, i, j, w, h, _unit, _data, _target);
                //Create.MakeBuilding(SimShader.UnitType.Barracks, Player.Three, Team.None, i, j, w, h, _unit, _data, _target);
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
    }
}
