using System;
using FragSharpFramework;

namespace GpuSim
{
    public partial class SimShader : GridComputation
    {
#if DEBUG
        public static Field<color> FarColor;
#endif

        protected color PlayerColorize(color clr, float player)
        {
            if (player == Player.One)
            {
            }
            else if (player == Player.Two)
            {
                float r = clr.r;
                clr.r = clr.g;
                clr.g = r;
                clr.rgb *= .5f;
            }
            else if (player == Player.Three)
            {
                float b = clr.b;
                clr.b = clr.g;
                clr.g = b;
            }
            else if (player == Player.Four)
            {
                float r = clr.r;
                clr.r = clr.b;
                clr.b = r;
            }
            else
            {
                clr.rgb *= .1f;
            }

            return clr;
        }

        public class UnitColor
        {
            public static readonly color
                Player1 = rgb(0x000000),//rgb(0x89eae9),
                Player2 = rgb(0xea8556),
                //Player1 = rgb(0x917c82),
                //Player2 = rgb(0xf0b021),

                //Player2 = new color(.2f, .7f, .2f, .5f),
                Player3 = new color(.4f, .85f, .65f, .5f),
                Player4 = new color(.4f, .4f, .85f, .5f);

            public static color Get(float player)
            {
#if DEBUG
                if (player == Player.One)   return FarColor[2, 1 + (int)player];
                if (player == Player.Two)   return FarColor[2, 2 + (int)player];
                if (player == Player.Three) return FarColor[2, 3 + (int)player];
                if (player == Player.Four)  return FarColor[2, 4 + (int)player];
#endif
                if (player == Player.One)   return Player1;
                if (player == Player.Two)   return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four)  return Player4;

                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class SelectedUnitColor
        {
            // rgb(0x54c96b) // Lighter selection

            public static readonly color
                Player1 = rgb(0x10AA10),
                Player2 = rgb(0x10AA10),
                Player3 = rgb(0x10AA10),
                Player4 = rgb(0x10AA10);

            public static color Get(float player)
            {
                if (player == Player.One) return Player1;
                if (player == Player.Two) return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four) return Player4;

                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class TerritoryColor
        {
            public static readonly color
                Player1 = rgb(0x0064dc),
                Player2 = rgb(0xff3220),
                //Player1 = rgb(0xc097a2),
                //Player2 = rgb(0xfbc34b),

                //Player1 = new color(.7f, .3f, .3f, .5f),
                //Player2 = new color(.1f, .5f, .1f, .5f),
                Player3 = new color(.3f, .7f, .55f, .5f),
                Player4 = new color(.3f, .3f, .7f, .5f);

            public static color Get(float player)
            {
                if (player == Player.One)   return Player1;
                if (player == Player.Two)   return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four)  return Player4;

                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class BuildingMarkerColors
        {
            public static readonly color
                Neutral = new color(248/255.0f, 230/255.0f, 33/255.0f, 1f),
                Player1 = new color(.6f, .2f, .2f, 1f),
                Player2 = new color(.0f, .4f, .0f, 1f),
                Player3 = new color(.2f, .6f, .45f, 1f),
                Player4 = new color(.2f, .2f, .7f, 1f);

            public static color Get(float player)
            {
                if (player == Player.One) return Player1;
                if (player == Player.Two) return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four) return Player4;

                throw new BadPlayerNumberException(player);
                return Neutral;
            }
        }
    }
}