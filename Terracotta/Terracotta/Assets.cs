using System.IO;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace GpuSim
{
    public static class Assets
    {
        public static Texture2D
            FarColors,

            DebugTexture_Arrows, DebugTexture_Num,

            BuildingTexture_1,
            ExplosionTexture_1,
            UnitTexture_1, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            
            TileSpriteSheet_1, TileSpriteSheet_2, TileSpriteSheet_4, TileSpriteSheet_8, TileSpriteSheet_16,

            Cursor, SelectCircle, SelectCircle_Data, SelectDot, AttackMarker;

        public static ContentManager Content { get { return GameClass.Game.Content; } }

        public static void Initialize()
        {
            FarColors           = LoadTexture("FarColors");
            SimShader.FarColor  = new Field<color>(FarColors);

            DebugTexture_Arrows = LoadTexture("Debug_Arrows");
            DebugTexture_Num    = LoadTexture("Debug_Num");

            BuildingTexture_1   = LoadTexture("Buildings_1");
            ExplosionTexture_1  = LoadTexture("BuildingExplosion_1");

            UnitTexture_1       = LoadTexture("Soldier_1");
            UnitTexture_2       = LoadTexture("Soldier_2");
            UnitTexture_4       = LoadTexture("Soldier_4");

            TileSpriteSheet_1   = LoadTexture("TileSet_1");
            TileSpriteSheet_2   = LoadTexture("TileSet_2");
            TileSpriteSheet_4   = LoadTexture("TileSet_4");
            TileSpriteSheet_8   = LoadTexture("TileSet_8");

            Cursor              = LoadTexture("Cursor");
            SelectCircle        = LoadTexture("SelectCircle");
            SelectCircle_Data   = LoadTexture("SelectCircle_Data");
            SelectDot           = LoadTexture("SelectDot");
            AttackMarker        = LoadTexture("AttackMarker");
        }

#if DEBUG
        static bool HotSwap = true;
#endif

        static Texture2D LoadTexture(string FileName)
        {
#if DEBUG
            try
            {
                if (HotSwap)
                {
                    using (var file = File.OpenRead(GameClass.Game.HotSwapDir + FileName + ".png"))
                    {
                        return Texture2D.FromStream(GameClass.Game.GraphicsDevice, file).PremultiplyAlpha();
                    }
                }
                else
                {
                    return Content.Load<Texture2D>("Art\\" + FileName);
                }
            }
            catch
            {
                return new Texture2D(GameClass.Game.GraphicsDevice, 1, 1);
            }
#else
            return Content.Load<Texture2D>("Art\\" + FileName);
#endif
        }
    }
}
