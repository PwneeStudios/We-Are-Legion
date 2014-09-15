using Microsoft.Xna.Framework;

namespace Terracotta
{
    public partial class DataGroup : SimShader
    {
        void InitialConditions()
        {
            Color[] _unit = new Color[w * h];
            Color[] _data = new Color[w * h];
            Color[] _extra = new Color[w * h];
            Color[] _target = new Color[w * h];
            Color[] _random = new Color[w * h];
            Color[] _tiles = new Color[w * h];
            Color[] _corpses = new Color[w * h];

            CurrentData.GetData(_data);

            var rnd = new System.Random();
            for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
            {
                _random[i * h + j] = new Color(rnd.IntRange(0, 256), rnd.IntRange(0, 256), rnd.IntRange(0, 256), rnd.IntRange(0, 256));
                _corpses[i * h + j] = new Color(0, 0, 0, 0);
                _tiles[i * h + j] = new Color(2, rnd.Next(0, 11), 30, 0);

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
                Create.MakeBuilding(SimShader.UnitType.GoldMine, Player.None, Team.None, i, j, w, h, _unit, _data, _target);
            }

            CurrentUnits.SetData(_unit);
            PreviousUnits.SetData(_unit);

            CurrentData.SetData(_data);
            PreviousData.SetData(_data);

            Extra.SetData(_extra);

            TargetData.SetData(_target);

            RandomField.SetData(_random);

            Tiles.SetData(_tiles);
            Corspes.SetData(_corpses);
        }
    }
}
