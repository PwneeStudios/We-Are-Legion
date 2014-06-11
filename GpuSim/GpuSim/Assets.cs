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
    public static class Assets
    {
        public static Texture2D
            BuildingTexture_1,
            ExplosionTexture_1,
            UnitTexture_1, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            GroundTexture,

            Cursor, SelectCircle, SelectCircle_Data, AttackMarker;

        public static ContentManager Content { get { return GameClass.Game.Content; } }

        public static void Initialize()
        {
            BuildingTexture_1 = Content.Load<Texture2D>("Art\\Buildings_1");
            ExplosionTexture_1 = Content.Load<Texture2D>("Art\\BuildingExplosion_1");

            UnitTexture_1 = Content.Load<Texture2D>("Art\\Units_1");
            UnitTexture_2 = Content.Load<Texture2D>("Art\\Units_2");
            UnitTexture_4 = Content.Load<Texture2D>("Art\\Units_4");
            UnitTexture_8 = Content.Load<Texture2D>("Art\\Units_8");
            UnitTexture_16 = Content.Load<Texture2D>("Art\\Units_16");

            GroundTexture = Content.Load<Texture2D>("Art\\Grass");

            Cursor = Content.Load<Texture2D>("Art\\Cursor");
            SelectCircle = Content.Load<Texture2D>("Art\\SelectCircle");
            SelectCircle_Data = Content.Load<Texture2D>("Art\\SelectCircle_Data");
            AttackMarker = Content.Load<Texture2D>("Art\\AttackMarker");
        }
    }
}
