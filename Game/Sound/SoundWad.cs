using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;

namespace Game
{
    public enum FindStyle { MustExist, DefaultIfNotFound, NullIfNotFound };
    public class SoundWad
    {
        public static SoundWad Wad = new SoundWad(4);

        /// <summary>
        /// When true all new sounds to be played are suppressed.
        /// </summary>
        public static bool SuppressSounds = false;

        public List<Sound> SoundList;

        public SoundWad(int MaxInstancesPerSound)
        {
            SoundList = new List<Sound>();
        }

        public Sound FindByName(string name, FindStyle style = FindStyle.DefaultIfNotFound)
        {
            foreach (Sound Snd in SoundList)
                if (String.Compare(Snd.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Snd;

            if (SoundList.Count == 0) return null;

            if (style == FindStyle.DefaultIfNotFound) return SoundList[0];
            if (style == FindStyle.NullIfNotFound) return null;
            if (style == FindStyle.MustExist) throw new Exception(string.Format("Sound {0} not found", name));
            
            return null;
        }

        public void AddSound(SoundEffect sound, string Name)
        {
            Sound NewSound = new Sound();
            NewSound.Name = Name;
            NewSound.sound = sound;

            SoundList.Add(NewSound);
        }
    }
}