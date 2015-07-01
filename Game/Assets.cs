using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using FragSharpFramework;

namespace Game
{
    public static class Assets
    {
        public static Texture2D
            ScreenTitle, ScreenDark, ScreenLoading,

            White, FarColors,

            DebugTexture_Arrows, DebugTexture_Num,

            BuildingTexture_1,
            ExplosionTexture_1, MagicTexture, SmokeTexture,
            UnitTexture_1, UnitTexture_2, UnitTexture_4, UnitTexture_8, UnitTexture_16,
            ShadowTexture,

            TileSpriteSheet_1, TileSpriteSheet_2, TileSpriteSheet_4, TileSpriteSheet_8, TileSpriteSheet_16,

            Cursor, SelectCircle, SelectCircle_Data, SelectDot, AttackMarker,
            AoE_Fire, AoE_Skeleton, AoE_Terra,
            DragonLord_Marker, AoE_DragonLord_Selected,

            TopUi, Minimap;

        public static Texture2D[] AoE_DragonLord;

        public static BaseSong Song_MenuMusic, Song_Heavens;
        public static List<BaseSong> SongList_Standard = new List<BaseSong>();

        public static ContentManager Content { get { return GameClass.Game.Content; } }

        public static void Initialize()
        {
            LoadTextures();
            LoadMusic();
            LoadSound();
        }

        private static void LoadTextures()
        {
            ScreenTitle = LoadTexture("Screen-Title");
            ScreenLoading = LoadTexture("Screen-Loading");
            ScreenDark = LoadTexture("Screen-Paint");

            White = LoadTexture("White");
            FarColors = LoadTexture("FarColors");
            SimShader.FarColor = new Field<color>(FarColors);

            DebugTexture_Arrows = LoadTexture("Debug_Arrows");
            DebugTexture_Num = LoadTexture("Debug_Num");

            BuildingTexture_1 = LoadTexture("Buildings_1");

            ExplosionTexture_1 = LoadTexture("BuildingExplosion_1");
            MagicTexture = LoadTexture("MagicEffect");
            SmokeTexture = LoadTexture("SmokeEffect");

            UnitTexture_1 = LoadTexture("Soldier_1");
            UnitTexture_2 = LoadTexture("Soldier_2");
            UnitTexture_4 = LoadTexture("Soldier_4");

            ShadowTexture = LoadTexture("Shadow");

            TileSpriteSheet_1 = LoadTexture("TileSet_1");
            TileSpriteSheet_2 = LoadTexture("TileSet_2");
            TileSpriteSheet_4 = LoadTexture("TileSet_4");
            TileSpriteSheet_8 = LoadTexture("TileSet_8");

            Cursor = LoadTexture("Cursor");
            SelectCircle = LoadTexture("SelectCircle");
            SelectCircle_Data = LoadTexture("SelectCircle_Data");
            SelectDot = LoadTexture("SelectDot");
            AttackMarker = LoadTexture("AttackMarker");

            AoE_Fire = LoadTexture("AoE_Fire");
            AoE_Skeleton = LoadTexture("AoE_Skeleton");
            AoE_Terra = LoadTexture("AoE_Terra");

            DragonLord_Marker = LoadTexture("DragonLord_Marker");
            AoE_DragonLord_Selected = LoadTexture("AoE_DragonLord_Selected");
            AoE_DragonLord = new Texture2D[5];
            for (int player = 1; player <= 4; player++)
            {
                AoE_DragonLord[player] = LoadTexture("AoE_DragonLord_" + player.ToString());
            }

            TopUi = LoadTexture("TopUi");
            Minimap = LoadTexture("Minimap");
        }

        private static void LoadMusic()
        {
            SongWad.Wad = new SongWad();

            SongWad.Wad.PlayerControl = SongWad.Wad.DisplayInfo = true;

            string path = Path.Combine(Content.RootDirectory, "Music");
            string[] files = Directory.GetFiles(path);

            foreach (String file in files)
            {
                int i = file.IndexOf("Music") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    SongWad.Wad.AddSong(name);
                }
            }

            float ReduceAll = .8f;

            Song_MenuMusic = SongWad.Wad.FindByName("Menu-Music^Unknown");
            Song_MenuMusic.Volume = .9f * ReduceAll;

            Song_Heavens = SongWad.Wad.FindByName("The_Heavens_Opened^Peacemaker");
            Song_Heavens.Volume = 1f * ReduceAll;

            // Create the standard playlist
            SongList_Standard.AddRange(SongWad.Wad.SongList);
            SongList_Standard.Remove(Song_MenuMusic);
        }

        static void LoadSound()
        {
            string path = Path.Combine(Content.RootDirectory, "Sound");
            string[] files = Directory.GetFiles(path);

            foreach (String file in files)
            {
                int i = file.IndexOf("Sound") + 5 + 1;
                if (i < 0) continue;
                int j = file.IndexOf(".", i);
                if (j <= i) continue;
                String name = file.Substring(i, j - i);
                String extension = file.Substring(j + 1);

                if (extension == "xnb")
                {
                    try
                    {
                        SoundWad.Wad.AddSound(Content.Load<SoundEffect>("Sound\\" + name), name);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            AmbientSounds.StartAmbientSounds();
        }

#if DEBUG
        public static bool HotSwap = false;
#endif

        static Texture2D LoadTexture(string FileName)
        {
#if DEBUG
            try
            {
                if (HotSwap && !Program.Server && !Program.Client)
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
