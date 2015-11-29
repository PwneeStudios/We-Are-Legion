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
            var sound = SoundWad.Wad.FindByName(name, FindStyle.NullIfNotFound);

            if (sound == null)
            {
                instance = null;
            }
            else
            {
                instance = sound.sound.CreateInstance();
                instance.IsLooped = true;
                instance.Play();
            }

            UpdateVolume();

            AmbientSounds.Sounds.Add(this);
        }

        float _Volume = 0;
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
            if (instance == null) return;

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
        public static AmbientSound
            SwordFight_Level1, SwordFight_Level2, SwordFight_Level3,
            Walking_Level1, Walking_Level2, Walking_Level3;

        public static List<AmbientSound> Sounds = new List<AmbientSound>();

        public static void StartAmbientSounds()
        {
            SwordFight_Level1 = new AmbientSound("SwordFight_Level2");
            SwordFight_Level2 = new AmbientSound("SwordFight_Level2");
            SwordFight_Level3 = new AmbientSound("SwordFight_Level2");

            Walking_Level1 = new AmbientSound("Walking_Level1");
            Walking_Level2 = new AmbientSound("Walking_Level2");
            Walking_Level3 = new AmbientSound("Walking_Level3");
        }
        
        public static void UpdateVolumes()
        {
            foreach (var sound in Sounds)
            {
                sound.UpdateVolume();
            }
        }

        public static void EndAll()
        {
            foreach (var sound in Sounds)
            {
                sound.EaseIntoVolume(0);
            }
        }
    }
}