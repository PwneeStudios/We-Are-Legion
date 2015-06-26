using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;

namespace Game
{
    public static class SoundEffectInstanceExtension
    {
        public static void SetVolume(this SoundEffectInstance instance, double volume)
        {
            instance.SetVolume((float)volume);
        }

        public static void SetVolume(this SoundEffectInstance instance, float volume)
        {
            float restrictedVolume = CoreMath.Restrict(0, 1, volume);
            instance.Volume = restrictedVolume;
        }
    }

    public class AmbientSound
    {
        SoundEffectInstance instance;

        public AmbientSound(string name)
        {
            instance = SoundWad.Wad.FindByName(name).sound.CreateInstance();
            instance.IsLooped = true;
            instance.Play();

            UpdateVolume();

            AmbientSounds.Sounds.Add(this);
        }

        float _Volume = 1;
        public float Volume
        {
            get
            {
                return _Volume;
            }

            set
            {
                _Volume = value;
                UpdateVolume();
            }
        }

        public void UpdateVolume()
        {
            instance.SetVolume(_Volume * GameClass.Game.CurrentConfig.SoundVolume);
        }

        public void EaseIntoVolume(float volume)
        {
            if (volume > _Volume)
            {
                _Volume = .25f * _Volume + .75f * volume;
            }
            else
            {
                _Volume = .5f * _Volume + .5f * volume;
            }

            UpdateVolume();
        }
    }

    public class AmbientSounds
    {
        public static AmbientSound ambient1;
        public static List<AmbientSound> Sounds = new List<AmbientSound>();

        public static void StartAmbientSounds()
        {
            ambient1 = new AmbientSound("CombinedSwordFight");
        }
        
        public static void UpdateVolumes()
        {
            foreach (var sound in Sounds)
            {
                sound.UpdateVolume();
            }
        }
    }
}