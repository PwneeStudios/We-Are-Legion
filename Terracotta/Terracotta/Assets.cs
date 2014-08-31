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
            
            GroundTexture, TileSpriteSheet,

            Cursor, SelectCircle, SelectCircle_Data, AttackMarker;

        public static ContentManager Content { get { return GameClass.Game.Content; } }

        public static void Initialize()
        {
            DebugTexture_Arrows = Content.Load<Texture2D>("Art\\Debug_Arrows");
            DebugTexture_Num = Content.Load<Texture2D>("Art\\Debug_Num");

            BuildingTexture_1 = Content.Load<Texture2D>("Art\\Buildings_1");
            ExplosionTexture_1 = Content.Load<Texture2D>("Art\\BuildingExplosion_1");

            UnitTexture_1 = Content.Load<Texture2D>("Art\\Units_1");
            UnitTexture_2 = Content.Load<Texture2D>("Art\\Units_2");
            UnitTexture_4 = Content.Load<Texture2D>("Art\\Units_4");
            UnitTexture_8 = Content.Load<Texture2D>("Art\\Units_8");
            UnitTexture_16 = Content.Load<Texture2D>("Art\\Units_16");

            //string unit = "Art\\infantry2";
            //UnitTexture_1 = Content.Load<Texture2D>(unit);
            //UnitTexture_2 = Content.Load<Texture2D>(unit);
            //UnitTexture_4 = Content.Load<Texture2D>(unit);
            //UnitTexture_8 = Content.Load<Texture2D>(unit);
            //UnitTexture_16 = Content.Load<Texture2D>(unit);


            GroundTexture = Content.Load<Texture2D>("Art\\Grass");
            TileSpriteSheet = Content.Load<Texture2D>("Art\\TileSet\\TileSet");

            Cursor = Content.Load<Texture2D>("Art\\Cursor");
            SelectCircle = Content.Load<Texture2D>("Art\\SelectCircle");
            SelectCircle_Data = Content.Load<Texture2D>("Art\\SelectCircle_Data");
            AttackMarker = Content.Load<Texture2D>("Art\\AttackMarker");
        }
    }
}
