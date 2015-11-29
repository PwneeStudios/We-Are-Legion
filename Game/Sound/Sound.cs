using System;

using Microsoft.Xna.Framework.Audio;

namespace Game
{
    public class Sound
    {
        public SoundEffect sound;

        public string Name;
        public int MaxInstances;
        public float DefaultVolume;
        public int DelayTillNextSoundCanPlay;
        int LastPlayedStamp;

        public Sound()
        {
            DelayTillNextSoundCanPlay = 1;

            DefaultVolume = 1f;
        }

        public void Play()
        {
            if (SoundWad.SuppressSounds) return;

            if (GameClass.Game.DrawCount - LastPlayedStamp <= DelayTillNextSoundCanPlay)
                return;

            try
            {
                sound.Play(GameClass.Game.CurrentConfig.SoundVolume * DefaultVolume, 0, 0);
            }
            catch (Exception e)
            {
            }

            LastPlayedStamp = GameClass.Game.DrawCount;
        }

        /// <summary>
        /// Plays the sound with a random modulation to the pitch.
        /// </summary>
        /// <param name="PitchModulationRange"></param>
        public void PlayModulated(float PitchModulationRange)
        {
            if (SoundWad.SuppressSounds) return;

            //Play(1, Tools.GlobalRnd.RndFloat(-PitchModulationRange, PitchModulationRange), 0);
            Play(1, 0, 0);
        }

        public void Play(float volume)
        {
            if (SoundWad.SuppressSounds) return;

            volume = CoreMath.Restrict(0, 1, volume);

            try
            {
                sound.Play(volume * GameClass.Game.CurrentConfig.SoundVolume * DefaultVolume, 0, 0);
            }
            catch (Exception e)
            {
            }
        }

        public void Play(float volume, float pitch, float pan)
        {
            if (SoundWad.SuppressSounds) return;

            volume = CoreMath.Restrict(0, 1, volume);

            try
            {
                sound.Play(volume * GameClass.Game.CurrentConfig.SoundVolume * DefaultVolume, CoreMath.Restrict(-1, 1, pitch), CoreMath.Restrict(-1, 1, pan));
            }
            catch (Exception e)
            {
            }
        }
    }
}