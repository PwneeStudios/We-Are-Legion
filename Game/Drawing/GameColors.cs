using FragSharpFramework;

namespace Game
{
    public partial class SimShader : GridComputation
    {
        public static Field<color> FarColor;

#if !DEBUG
        static readonly color
            __0_0   = rgba(0x000000, 1f),
            __0_1   = rgba(0x70689C, 1f),
            __0_2   = rgba(0x924239, 1f),
            __0_3   = rgba(0x619CB4, 1f),
            __0_4   = rgba(0xF6A4B2, 1f),
            __0_5   = rgba(0x000000, 0f),
            __0_6   = rgba(0x000000, 1f),
            __0_7   = rgba(0x000000, 0f),
            __0_8   = rgba(0x000000, 0f),
            __0_9   = rgba(0x000000, 0f),
            __0_10  = rgba(0x000000, 0f),
            __0_11  = rgba(0x000000, 0f),
            __0_12  = rgba(0x000000, 0f),
            __0_13  = rgba(0x000000, 0f),
            __0_14  = rgba(0x000000, 0f),
            __0_15  = rgba(0x000000, 0f),
            __1_0   = rgba(0x000000, 1f),
            __1_1   = rgba(0x26A822, 1f),
            __1_2   = rgba(0x26A822, 1f),
            __1_3   = rgba(0x26A822, 1f),
            __1_4   = rgba(0x26A822, 1f),
            __1_5   = rgba(0x000000, 0f),
            __1_6   = rgba(0x2C5816, 1f),
            __1_7   = rgba(0x000000, 0f),
            __1_8   = rgba(0x000000, 0f),
            __1_9   = rgba(0x000000, 0f),
            __1_10  = rgba(0x000000, 0f),
            __1_11  = rgba(0x000000, 0f),
            __1_12  = rgba(0x000000, 0f),
            __1_13  = rgba(0x000000, 0f),
            __1_14  = rgba(0x000000, 0f),
            __1_15  = rgba(0x000000, 0f),
            __2_0   = rgba(0x000000, 1f),
            __2_1   = rgba(0x4535A4, 1f),
            __2_2   = rgba(0x7A2C23, 1f),
            __2_3   = rgba(0x3F819C, 1f),
            __2_4   = rgba(0xE16178, 1f),
            __2_5   = rgba(0x000000, 0f),
            __2_6   = rgba(0x946826, 1f),
            __2_7   = rgba(0x000000, 0f),
            __2_8   = rgba(0x000000, 0f),
            __2_9   = rgba(0x000000, 0f),
            __2_10  = rgba(0x000000, 0f),
            __2_11  = rgba(0x000000, 0f),
            __2_12  = rgba(0x000000, 0f),
            __2_13  = rgba(0x000000, 0f),
            __2_14  = rgba(0x000000, 0f),
            __2_15  = rgba(0x000000, 0f),
            __3_0   = rgba(0x000000, 1f),
            __3_1   = rgba(0x003060, 1f),
            __3_2   = rgba(0x4C2200, 1f),
            __3_3   = rgba(0x144D64, 1f),
            __3_4   = rgba(0xCB2D48, 1f),
            __3_5   = rgba(0x000000, 0f),
            __3_6   = rgba(0x000000, 1f),
            __3_7   = rgba(0x000000, 0f),
            __3_8   = rgba(0x000000, 0f),
            __3_9   = rgba(0x000000, 0f),
            __3_10  = rgba(0x000000, 0f),
            __3_11  = rgba(0x000000, 0f),
            __3_12  = rgba(0x000000, 0f),
            __3_13  = rgba(0x000000, 0f),
            __3_14  = rgba(0x000000, 0f),
            __3_15  = rgba(0x000000, 0f),
            __4_0   = rgba(0xF8E621, 1f),
            __4_1   = rgba(0x003060, 1f),
            __4_2   = rgba(0x4C2200, 1f),
            __4_3   = rgba(0x144D64, 1f),
            __4_4   = rgba(0xCB2D48, 1f),
            __4_5   = rgba(0x000000, 0f),
            __4_6   = rgba(0x000000, 1f),
            __4_7   = rgba(0x000000, 0f),
            __4_8   = rgba(0x000000, 0f),
            __4_9   = rgba(0x000000, 0f),
            __4_10  = rgba(0x000000, 0f),
            __4_11  = rgba(0x000000, 0f),
            __4_12  = rgba(0x000000, 0f),
            __4_13  = rgba(0x000000, 0f),
            __4_14  = rgba(0x000000, 0f),
            __4_15  = rgba(0x000000, 0f),
            __5_0   = rgba(0x11C511, 1f),
            __5_1   = rgba(0x003060, 1f),
            __5_2   = rgba(0x4C2200, 1f),
            __5_3   = rgba(0x144D64, 1f),
            __5_4   = rgba(0xCB2D48, 1f),
            __5_5   = rgba(0x000000, 0f),
            __5_6   = rgba(0x0D3406, 1f),
            __5_7   = rgba(0x000000, 0f),
            __5_8   = rgba(0x000000, 0f),
            __5_9   = rgba(0x000000, 0f),
            __5_10  = rgba(0x000000, 0f),
            __5_11  = rgba(0x000000, 0f),
            __5_12  = rgba(0x000000, 0f),
            __5_13  = rgba(0x000000, 0f),
            __5_14  = rgba(0x000000, 0f),
            __5_15  = rgba(0x000000, 0f),
            __6_0   = rgba(0x000000, 1f),
            __6_1   = rgba(0x000000, 1f),
            __6_2   = rgba(0x000000, 1f),
            __6_3   = rgba(0x000000, 1f),
            __6_4   = rgba(0x000000, 1f),
            __6_5   = rgba(0x000000, 0f),
            __6_6   = rgba(0x000000, 0f),
            __6_7   = rgba(0x000000, 0f),
            __6_8   = rgba(0x000000, 0f),
            __6_9   = rgba(0x000000, 0f),
            __6_10  = rgba(0x000000, 0f),
            __6_11  = rgba(0x000000, 0f),
            __6_12  = rgba(0x000000, 0f),
            __6_13  = rgba(0x000000, 0f),
            __6_14  = rgba(0x000000, 0f),
            __6_15  = rgba(0x000000, 0f),
            __7_0   = rgba(0x3F3F3F, 1f),
            __7_1   = rgba(0x3F3F3F, 1f),
            __7_2   = rgba(0x3F3F3F, 1f),
            __7_3   = rgba(0x3F3F3F, 1f),
            __7_4   = rgba(0x3F3F3F, 1f),
            __7_5   = rgba(0x000000, 0f),
            __7_6   = rgba(0x000000, 0f),
            __7_7   = rgba(0x000000, 0f),
            __7_8   = rgba(0x000000, 0f),
            __7_9   = rgba(0x000000, 0f),
            __7_10  = rgba(0x000000, 0f),
            __7_11  = rgba(0x000000, 0f),
            __7_12  = rgba(0x000000, 0f),
            __7_13  = rgba(0x000000, 0f),
            __7_14  = rgba(0x000000, 0f),
            __7_15  = rgba(0x000000, 0f),
            __8_0   = rgba(0x000000, 0f),
            __8_1   = rgba(0x000000, 0f),
            __8_2   = rgba(0x000000, 0f),
            __8_3   = rgba(0x000000, 0f),
            __8_4   = rgba(0x000000, 0f),
            __8_5   = rgba(0x000000, 0f),
            __8_6   = rgba(0x000000, 0f),
            __8_7   = rgba(0x000000, 0f),
            __8_8   = rgba(0x000000, 0f),
            __8_9   = rgba(0x000000, 0f),
            __8_10  = rgba(0x000000, 0f),
            __8_11  = rgba(0x000000, 0f),
            __8_12  = rgba(0x000000, 0f),
            __8_13  = rgba(0x000000, 0f),
            __8_14  = rgba(0x000000, 0f),
            __8_15  = rgba(0x000000, 0f),
            __9_0   = rgba(0x000000, 0f),
            __9_1   = rgba(0x000000, 0f),
            __9_2   = rgba(0x000000, 0f),
            __9_3   = rgba(0x000000, 0f),
            __9_4   = rgba(0x000000, 0f),
            __9_5   = rgba(0x000000, 0f),
            __9_6   = rgba(0x000000, 0f),
            __9_7   = rgba(0x000000, 0f),
            __9_8   = rgba(0x000000, 0f),
            __9_9   = rgba(0x000000, 0f),
            __9_10  = rgba(0x000000, 0f),
            __9_11  = rgba(0x000000, 0f),
            __9_12  = rgba(0x000000, 0f),
            __9_13  = rgba(0x000000, 0f),
            __9_14  = rgba(0x000000, 0f),
            __9_15  = rgba(0x000000, 0f),
            __10_0  = rgba(0x000000, 0f),
            __10_1  = rgba(0x000000, 0f),
            __10_2  = rgba(0x000000, 0f),
            __10_3  = rgba(0x000000, 0f),
            __10_4  = rgba(0x000000, 0f),
            __10_5  = rgba(0x000000, 0f),
            __10_6  = rgba(0x000000, 0f),
            __10_7  = rgba(0x000000, 0f),
            __10_8  = rgba(0x000000, 0f),
            __10_9  = rgba(0x000000, 0f),
            __10_10 = rgba(0x000000, 0f),
            __10_11 = rgba(0x000000, 0f),
            __10_12 = rgba(0x000000, 0f),
            __10_13 = rgba(0x000000, 0f),
            __10_14 = rgba(0x000000, 0f),
            __10_15 = rgba(0x000000, 0f),
            __11_0  = rgba(0x000000, 0f),
            __11_1  = rgba(0x000000, 0f),
            __11_2  = rgba(0x000000, 0f),
            __11_3  = rgba(0x000000, 0f),
            __11_4  = rgba(0x000000, 0f),
            __11_5  = rgba(0x000000, 0f),
            __11_6  = rgba(0x000000, 0f),
            __11_7  = rgba(0x000000, 0f),
            __11_8  = rgba(0x000000, 0f),
            __11_9  = rgba(0x000000, 0f),
            __11_10 = rgba(0x000000, 0f),
            __11_11 = rgba(0x000000, 0f),
            __11_12 = rgba(0x000000, 0f),
            __11_13 = rgba(0x000000, 0f),
            __11_14 = rgba(0x000000, 0f),
            __11_15 = rgba(0x000000, 0f),
            __12_0  = rgba(0x000000, 0f),
            __12_1  = rgba(0x000000, 0f),
            __12_2  = rgba(0x000000, 0f),
            __12_3  = rgba(0x000000, 0f),
            __12_4  = rgba(0x000000, 0f),
            __12_5  = rgba(0x000000, 0f),
            __12_6  = rgba(0x000000, 0f),
            __12_7  = rgba(0x000000, 0f),
            __12_8  = rgba(0x000000, 0f),
            __12_9  = rgba(0x000000, 0f),
            __12_10 = rgba(0x000000, 0f),
            __12_11 = rgba(0x000000, 0f),
            __12_12 = rgba(0x000000, 0f),
            __12_13 = rgba(0x000000, 0f),
            __12_14 = rgba(0x000000, 0f),
            __12_15 = rgba(0x000000, 0f),
            __13_0  = rgba(0x000000, 0f),
            __13_1  = rgba(0x000000, 0f),
            __13_2  = rgba(0x000000, 0f),
            __13_3  = rgba(0x000000, 0f),
            __13_4  = rgba(0x000000, 0f),
            __13_5  = rgba(0x000000, 0f),
            __13_6  = rgba(0x000000, 0f),
            __13_7  = rgba(0x000000, 0f),
            __13_8  = rgba(0x000000, 0f),
            __13_9  = rgba(0x000000, 0f),
            __13_10 = rgba(0x000000, 0f),
            __13_11 = rgba(0x000000, 0f),
            __13_12 = rgba(0x000000, 0f),
            __13_13 = rgba(0x000000, 0f),
            __13_14 = rgba(0x000000, 0f),
            __13_15 = rgba(0x000000, 0f),
            __14_0  = rgba(0x000000, 0f),
            __14_1  = rgba(0x000000, 0f),
            __14_2  = rgba(0x000000, 0f),
            __14_3  = rgba(0x000000, 0f),
            __14_4  = rgba(0x000000, 0f),
            __14_5  = rgba(0x000000, 0f),
            __14_6  = rgba(0x000000, 0f),
            __14_7  = rgba(0x000000, 0f),
            __14_8  = rgba(0x000000, 0f),
            __14_9  = rgba(0x000000, 0f),
            __14_10 = rgba(0x000000, 0f),
            __14_11 = rgba(0x000000, 0f),
            __14_12 = rgba(0x000000, 0f),
            __14_13 = rgba(0x000000, 0f),
            __14_14 = rgba(0x000000, 0f),
            __14_15 = rgba(0x000000, 0f),
            __15_0  = rgba(0x000000, 0f),
            __15_1  = rgba(0x000000, 0f),
            __15_2  = rgba(0x000000, 0f),
            __15_3  = rgba(0x000000, 0f),
            __15_4  = rgba(0x000000, 0f),
            __15_5  = rgba(0x000000, 0f),
            __15_6  = rgba(0x000000, 0f),
            __15_7  = rgba(0x000000, 0f),
            __15_8  = rgba(0x000000, 0f),
            __15_9  = rgba(0x000000, 0f),
            __15_10 = rgba(0x000000, 0f),
            __15_11 = rgba(0x000000, 0f),
            __15_12 = rgba(0x000000, 0f),
            __15_13 = rgba(0x000000, 0f),
            __15_14 = rgba(0x000000, 0f),
            __15_15 = rgba(0x000000, 0f);
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
#if DEBUG
            //public static color Player1() { return Get(Player.One); }
            //public static color Player2() { return Get(Player.Two); }
            //public static color Player3() { return Get(Player.Three); }
            //public static color Player4() { return Get(Player.Four); }

            public static color Player1() { return FarColor[2, 1]; }
            public static color Player2() { return FarColor[2, 2]; }
            public static color Player3() { return FarColor[2, 3]; }
            public static color Player4() { return FarColor[2, 4]; }

            //public static color Player1() { return FarColor[2, 1 + (int)player]; }
            //public static color Player2() { return FarColor[2, 2 + (int)player]; }
            //public static color Player3() { return FarColor[2, 3 + (int)player]; }
            //public static color Player4() { return FarColor[2, 4 + (int)player]; }
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
            public static color Get(float player, float type)
            {
                return FarColor[3 + Int(UnitType.BuildingIndex(type)), Int(player)];
            }
        }
    }
}