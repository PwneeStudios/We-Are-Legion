using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;

namespace Game
{
    public class SoundWad
    {
        public static SoundWad Wad = new SoundWad(4);

        /// <summary>
        /// When true all new sounds to be played are suppressed.
        /// </summary>
        public static bool SuppressSounds = false;

        public List<Sound> SoundList;
        public int MaxInstancesPerSound;

        public SoundWad(int MaxInstancesPerSound)
        {
            this.MaxInstancesPerSound = MaxInstancesPerSound;

            SoundList = new List<Sound>();
        }

        public Sound FindByName(string name)
        {
            foreach (Sound Snd in SoundList)
                if (String.Compare(Snd.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Snd;

            if (SoundList.Count == 0) return null;

            return SoundList[0];
        }

        public void AddSound(SoundEffect sound, string Name)
        {
            Sound NewSound = new Sound();
            NewSound.Name = Name;
            NewSound.sound = sound;
            NewSound.MaxInstances = MaxInstancesPerSound;

            SoundList.Add(NewSound);
        }
    }
}