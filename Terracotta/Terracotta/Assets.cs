using System.IO;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GpuSim
{
    public static class Assets
    {
        public static Texture2D
            DebugTexture_Arrows, DebugTexture_Num,

            BuildingTexture_1,
            ExplosionTexture_1,
            UnitTexture_1, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            
            TileSpriteSheet,

            Cursor, SelectCircle, SelectCircle_Data, SelectDot, AttackMarker;

        public static ContentManager Content { get { return GameClass.Game.Content; } }

        public static void Initialize()
        {
            DebugTexture_Arrows = LoadTexture("Debug_Arrows");
            DebugTexture_Num    = LoadTexture("Debug_Num");

            BuildingTexture_1   = LoadTexture("Buildings_1");
            ExplosionTexture_1  = LoadTexture("BuildingExplosion_1");

            UnitTexture_1       = LoadTexture("Soldier_1");
            UnitTexture_2       = LoadTexture("Soldier_2");
            UnitTexture_4       = LoadTexture("Soldier_4");
            UnitTexture_8       = LoadTexture("Soldier_8");
            UnitTexture_16      = LoadTexture("Soldier_16");

            TileSpriteSheet     = LoadTexture("TileSet");

            Cursor              = LoadTexture("Cursor");
            SelectCircle        = LoadTexture("SelectCircle");
            SelectCircle_Data   = LoadTexture("SelectCircle_Data");
            SelectDot          = LoadTexture("SelectDot");
            AttackMarker        = LoadTexture("AttackMarker");
        }

#if DEBUG
        static bool HotSwap = true;
#else
        static bool HotSwap = false;
#endif

        static Texture2D LoadTexture(string FileName)
        {
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
        }
    }
}
