using System;
using FragSharpFramework;

namespace GpuSim
{
    public partial class SimShader : GridComputation
    {
#if DEBUG
        static Field<color> FarColor;
#else
        static readonly color
            __0_0   = rgba(0x000000, 1),
            __0_1   = rgba(0x89EAE9, 1),
            __0_2   = rgba(0xEA8556, 1),
            __0_3   = rgba(0x000000, 1),
            __0_4   = rgba(0x000000, 1),
            __0_5   = rgba(0x000000, 0),
            __0_6   = rgba(0x000000, 0),
            __0_7   = rgba(0x000000, 0),
            __0_8   = rgba(0x000000, 0),
            __0_9   = rgba(0x000000, 0),
            __0_10  = rgba(0x000000, 0),
            __0_11  = rgba(0x000000, 0),
            __0_12  = rgba(0x000000, 0),
            __0_13  = rgba(0x000000, 0),
            __0_14  = rgba(0x000000, 0),
            __0_15  = rgba(0x000000, 0),
            __1_0   = rgba(0x000000, 1),
            __1_1   = rgba(0x26A822, 1),
            __1_2   = rgba(0x26A822, 1),
            __1_3   = rgba(0x000000, 1),
            __1_4   = rgba(0x000000, 1),
            __1_5   = rgba(0x000000, 0),
            __1_6   = rgba(0x000000, 0),
            __1_7   = rgba(0x000000, 0),
            __1_8   = rgba(0x000000, 0),
            __1_9   = rgba(0x000000, 0),
            __1_10  = rgba(0x000000, 0),
            __1_11  = rgba(0x000000, 0),
            __1_12  = rgba(0x000000, 0),
            __1_13  = rgba(0x000000, 0),
            __1_14  = rgba(0x000000, 0),
            __1_15  = rgba(0x000000, 0),
            __2_0   = rgba(0x000000, 1),
            __2_1   = rgba(0x0064DC, 1),
            __2_2   = rgba(0xFF3220, 1),
            __2_3   = rgba(0x000000, 1),
            __2_4   = rgba(0x000000, 1),
            __2_5   = rgba(0x000000, 0),
            __2_6   = rgba(0x000000, 0),
            __2_7   = rgba(0x000000, 0),
            __2_8   = rgba(0x000000, 0),
            __2_9   = rgba(0x000000, 0),
            __2_10  = rgba(0x000000, 0),
            __2_11  = rgba(0x000000, 0),
            __2_12  = rgba(0x000000, 0),
            __2_13  = rgba(0x000000, 0),
            __2_14  = rgba(0x000000, 0),
            __2_15  = rgba(0x000000, 0),
            __3_0   = rgba(0x000000, 1),
            __3_1   = rgba(0x003060, 1),
            __3_2   = rgba(0x4C2200, 1),
            __3_3   = rgba(0x000000, 1),
            __3_4   = rgba(0x000000, 1),
            __3_5   = rgba(0x000000, 0),
            __3_6   = rgba(0x000000, 0),
            __3_7   = rgba(0x000000, 0),
            __3_8   = rgba(0x000000, 0),
            __3_9   = rgba(0x000000, 0),
            __3_10  = rgba(0x000000, 0),
            __3_11  = rgba(0x000000, 0),
            __3_12  = rgba(0x000000, 0),
            __3_13  = rgba(0x000000, 0),
            __3_14  = rgba(0x000000, 0),
            __3_15  = rgba(0x000000, 0),
            __4_0   = rgba(0xF8E621, 1),
            __4_1   = rgba(0x003060, 1),
            __4_2   = rgba(0x4C2200, 1),
            __4_3   = rgba(0x000000, 1),
            __4_4   = rgba(0x000000, 1),
            __4_5   = rgba(0x000000, 0),
            __4_6   = rgba(0x000000, 0),
            __4_7   = rgba(0x000000, 0),
            __4_8   = rgba(0x000000, 0),
            __4_9   = rgba(0x000000, 0),
            __4_10  = rgba(0x000000, 0),
            __4_11  = rgba(0x000000, 0),
            __4_12  = rgba(0x000000, 0),
            __4_13  = rgba(0x000000, 0),
            __4_14  = rgba(0x000000, 0),
            __4_15  = rgba(0x000000, 0),
            __5_0   = rgba(0x11C511, 1),
            __5_1   = rgba(0x003060, 1),
            __5_2   = rgba(0x4C2200, 1),
            __5_3   = rgba(0x000000, 1),
            __5_4   = rgba(0x000000, 1),
            __5_5   = rgba(0x000000, 0),
            __5_6   = rgba(0x000000, 0),
            __5_7   = rgba(0x000000, 0),
            __5_8   = rgba(0x000000, 0),
            __5_9   = rgba(0x000000, 0),
            __5_10  = rgba(0x000000, 0),
            __5_11  = rgba(0x000000, 0),
            __5_12  = rgba(0x000000, 0),
            __5_13  = rgba(0x000000, 0),
            __5_14  = rgba(0x000000, 0),
            __5_15  = rgba(0x000000, 0),
            __6_0   = rgba(0x000000, 0),
            __6_1   = rgba(0x000000, 0),
            __6_2   = rgba(0x000000, 0),
            __6_3   = rgba(0x000000, 0),
            __6_4   = rgba(0x000000, 0),
            __6_5   = rgba(0x000000, 0),
            __6_6   = rgba(0x000000, 0),
            __6_7   = rgba(0x000000, 0),
            __6_8   = rgba(0x000000, 0),
            __6_9   = rgba(0x000000, 0),
            __6_10  = rgba(0x000000, 0),
            __6_11  = rgba(0x000000, 0),
            __6_12  = rgba(0x000000, 0),
            __6_13  = rgba(0x000000, 0),
            __6_14  = rgba(0x000000, 0),
            __6_15  = rgba(0x000000, 0),
            __7_0   = rgba(0x000000, 0),
            __7_1   = rgba(0x000000, 0),
            __7_2   = rgba(0x000000, 0),
            __7_3   = rgba(0x000000, 0),
            __7_4   = rgba(0x000000, 0),
            __7_5   = rgba(0x000000, 0),
            __7_6   = rgba(0x000000, 0),
            __7_7   = rgba(0x000000, 0),
            __7_8   = rgba(0x000000, 0),
            __7_9   = rgba(0x000000, 0),
            __7_10  = rgba(0x000000, 0),
            __7_11  = rgba(0x000000, 0),
            __7_12  = rgba(0x000000, 0),
            __7_13  = rgba(0x000000, 0),
            __7_14  = rgba(0x000000, 0),
            __7_15  = rgba(0x000000, 0),
            __8_0   = rgba(0x000000, 0),
            __8_1   = rgba(0x000000, 0),
            __8_2   = rgba(0x000000, 0),
            __8_3   = rgba(0x000000, 0),
            __8_4   = rgba(0x000000, 0),
            __8_5   = rgba(0x000000, 0),
            __8_6   = rgba(0x000000, 0),
            __8_7   = rgba(0x000000, 0),
            __8_8   = rgba(0x000000, 0),
            __8_9   = rgba(0x000000, 0),
            __8_10  = rgba(0x000000, 0),
            __8_11  = rgba(0x000000, 0),
            __8_12  = rgba(0x000000, 0),
            __8_13  = rgba(0x000000, 0),
            __8_14  = rgba(0x000000, 0),
            __8_15  = rgba(0x000000, 0),
            __9_0   = rgba(0x000000, 0),
            __9_1   = rgba(0x000000, 0),
            __9_2   = rgba(0x000000, 0),
            __9_3   = rgba(0x000000, 0),
            __9_4   = rgba(0x000000, 0),
            __9_5   = rgba(0x000000, 0),
            __9_6   = rgba(0x000000, 0),
            __9_7   = rgba(0x000000, 0),
            __9_8   = rgba(0x000000, 0),
            __9_9   = rgba(0x000000, 0),
            __9_10  = rgba(0x000000, 0),
            __9_11  = rgba(0x000000, 0),
            __9_12  = rgba(0x000000, 0),
            __9_13  = rgba(0x000000, 0),
            __9_14  = rgba(0x000000, 0),
            __9_15  = rgba(0x000000, 0),
            __10_0  = rgba(0x000000, 0),
            __10_1  = rgba(0x000000, 0),
            __10_2  = rgba(0x000000, 0),
            __10_3  = rgba(0x000000, 0),
            __10_4  = rgba(0x000000, 0),
            __10_5  = rgba(0x000000, 0),
            __10_6  = rgba(0x000000, 0),
            __10_7  = rgba(0x000000, 0),
            __10_8  = rgba(0x000000, 0),
            __10_9  = rgba(0x000000, 0),
            __10_10 = rgba(0x000000, 0),
            __10_11 = rgba(0x000000, 0),
            __10_12 = rgba(0x000000, 0),
            __10_13 = rgba(0x000000, 0),
            __10_14 = rgba(0x000000, 0),
            __10_15 = rgba(0x000000, 0),
            __11_0  = rgba(0x000000, 0),
            __11_1  = rgba(0x000000, 0),
            __11_2  = rgba(0x000000, 0),
            __11_3  = rgba(0x000000, 0),
            __11_4  = rgba(0x000000, 0),
            __11_5  = rgba(0x000000, 0),
            __11_6  = rgba(0x000000, 0),
            __11_7  = rgba(0x000000, 0),
            __11_8  = rgba(0x000000, 0),
            __11_9  = rgba(0x000000, 0),
            __11_10 = rgba(0x000000, 0),
            __11_11 = rgba(0x000000, 0),
            __11_12 = rgba(0x000000, 0),
            __11_13 = rgba(0x000000, 0),
            __11_14 = rgba(0x000000, 0),
            __11_15 = rgba(0x000000, 0),
            __12_0  = rgba(0x000000, 0),
            __12_1  = rgba(0x000000, 0),
            __12_2  = rgba(0x000000, 0),
            __12_3  = rgba(0x000000, 0),
            __12_4  = rgba(0x000000, 0),
            __12_5  = rgba(0x000000, 0),
            __12_6  = rgba(0x000000, 0),
            __12_7  = rgba(0x000000, 0),
            __12_8  = rgba(0x000000, 0),
            __12_9  = rgba(0x000000, 0),
            __12_10 = rgba(0x000000, 0),
            __12_11 = rgba(0x000000, 0),
            __12_12 = rgba(0x000000, 0),
            __12_13 = rgba(0x000000, 0),
            __12_14 = rgba(0x000000, 0),
            __12_15 = rgba(0x000000, 0),
            __13_0  = rgba(0x000000, 0),
            __13_1  = rgba(0x000000, 0),
            __13_2  = rgba(0x000000, 0),
            __13_3  = rgba(0x000000, 0),
            __13_4  = rgba(0x000000, 0),
            __13_5  = rgba(0x000000, 0),
            __13_6  = rgba(0x000000, 0),
            __13_7  = rgba(0x000000, 0),
            __13_8  = rgba(0x000000, 0),
            __13_9  = rgba(0x000000, 0),
            __13_10 = rgba(0x000000, 0),
            __13_11 = rgba(0x000000, 0),
            __13_12 = rgba(0x000000, 0),
            __13_13 = rgba(0x000000, 0),
            __13_14 = rgba(0x000000, 0),
            __13_15 = rgba(0x000000, 0),
            __14_0  = rgba(0x000000, 0),
            __14_1  = rgba(0x000000, 0),
            __14_2  = rgba(0x000000, 0),
            __14_3  = rgba(0x000000, 0),
            __14_4  = rgba(0x000000, 0),
            __14_5  = rgba(0x000000, 0),
            __14_6  = rgba(0x000000, 0),
            __14_7  = rgba(0x000000, 0),
            __14_8  = rgba(0x000000, 0),
            __14_9  = rgba(0x000000, 0),
            __14_10 = rgba(0x000000, 0),
            __14_11 = rgba(0x000000, 0),
            __14_12 = rgba(0x000000, 0),
            __14_13 = rgba(0x000000, 0),
            __14_14 = rgba(0x000000, 0),
            __14_15 = rgba(0x000000, 0),
            __15_0  = rgba(0x000000, 0),
            __15_1  = rgba(0x000000, 0),
            __15_2  = rgba(0x000000, 0),
            __15_3  = rgba(0x000000, 0),
            __15_4  = rgba(0x000000, 0),
            __15_5  = rgba(0x000000, 0),
            __15_6  = rgba(0x000000, 0),
            __15_7  = rgba(0x000000, 0),
            __15_8  = rgba(0x000000, 0),
            __15_9  = rgba(0x000000, 0),
            __15_10 = rgba(0x000000, 0),
            __15_11 = rgba(0x000000, 0),
            __15_12 = rgba(0x000000, 0),
            __15_13 = rgba(0x000000, 0),
            __15_14 = rgba(0x000000, 0),
            __15_15 = rgba(0x000000, 0);
#endif

        public class UnitColor
        {
            public static color Get(float player)
            {
#if DEBUG
                if (player == Player.One)   return FarColor[0, 1 + (int)player];
                if (player == Player.Two)   return FarColor[0, 2 + (int)player];
                if (player == Player.Three) return FarColor[0, 3 + (int)player];
                if (player == Player.Four)  return FarColor[0, 4 + (int)player];
#else
                if (player == Player.One)   return __0_1;
                if (player == Player.Two)   return __0_2;
                if (player == Player.Three) return __0_3;
                if (player == Player.Four)  return __0_4;
#endif
                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class SelectedUnitColor
        {
            public static color Get(float player)
            {
#if DEBUG
                if (player == Player.One)   return FarColor[1, 1 + (int)player];
                if (player == Player.Two)   return FarColor[1, 2 + (int)player];
                if (player == Player.Three) return FarColor[1, 3 + (int)player];
                if (player == Player.Four)  return FarColor[1, 4 + (int)player];
#else
                if (player == Player.One)   return __1_1;
                if (player == Player.Two)   return __1_2;
                if (player == Player.Three) return __1_3;
                if (player == Player.Four)  return __1_4;
#endif
                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class TerritoryColor
        {
#if DEBUB
            public static color Player1() { return FarColor[2, 1 + (int)player]; }
            public static color Player2() { return FarColor[2, 2 + (int)player]; }
            public static color Player3() { return FarColor[2, 3 + (int)player]; }
            public static color Player4() { return FarColor[2, 4 + (int)player]; }
#else
            public static color Player1() { return __2_1; }
            public static color Player2() { return __2_2; }
            public static color Player3() { return __2_3; }
            public static color Player4() { return __2_4; }
#endif
            public static color Get(float player)
            {
#if DEBUG
                if (player == Player.One)   return FarColor[2, 1 + (int)player];
                if (player == Player.Two)   return FarColor[2, 2 + (int)player];
                if (player == Player.Three) return FarColor[2, 3 + (int)player];
                if (player == Player.Four)  return FarColor[2, 4 + (int)player];
#else
                if (player == Player.One)   return __2_1;
                if (player == Player.Two)   return __2_2;
                if (player == Player.Three) return __2_3;
                if (player == Player.Four)  return __2_4;
#endif
                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class BuildingMarkerColors
        {
            public static color Get(float player)
            {
#if DEBUG
                if (player == Player.One)   return FarColor[3, 1 + (int)player];
                if (player == Player.Two)   return FarColor[3, 2 + (int)player];
                if (player == Player.Three) return FarColor[3, 3 + (int)player];
                if (player == Player.Four)  return FarColor[3, 4 + (int)player];
#else
                if (player == Player.One)   return __3_1;
                if (player == Player.Two)   return __3_2;
                if (player == Player.Three) return __3_3;
                if (player == Player.Four)  return __3_4;
#endif
                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }
    }
}