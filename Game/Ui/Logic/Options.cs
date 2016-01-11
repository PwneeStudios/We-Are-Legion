using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public float ArgTo0to1(float arg)
        {
            string stringVal;

            try
            {
                stringVal = arg.ToString();
            }
            catch (Exception e)
            {
                return 0;
            }

            float val = (float)CoreMath.ParseDouble(stringVal);

            if (val < 0) val = 0;
            if (val > 1) val = 1;

            return val;
        }

        public void SetSoundVolume(float volume)
        {
            CurrentConfig.SoundVolume = ArgTo0to1(volume);
            AmbientSounds.UpdateVolumes();
            SaveConfig();
        }

        public void GetSoundVolume()
        {
            Send("getSoundVolume", CurrentConfig.SoundVolume);
        }

        public void SetMusicVolume(float volume)
        {
            CurrentConfig.MusicVolume = ArgTo0to1(volume);
            SaveConfig();
        }

        public void GetMusicVolume()
        {
            Send("getMusicVolume", CurrentConfig.MusicVolume);
        }

        public void SetFullscreen(bool fullscreen)
        {
            CurrentConfig.Fullscreen = fullscreen;
            
            SaveConfig();
            ApplyConfig();
        }

        public void GetFullscreen()
        {
            Send("getFullscreen", CurrentConfig.Fullscreen);
        }

        public void GetFullscreenValues()
        {
            var options = new List<Dict>();
            var dict = new Dict();

            dict = new Dict();
            dict["name"] = "Fullscreen";
            dict["value"] = true;
            options.Add(dict);

            dict = new Dict();
            dict["name"] = "Windowed";
            dict["value"] = false;
            options.Add(dict);

            Send("getFullscreenValues", options);
        }

        public void SetResolution(int Resolution)
        {
            if (Resolution >= 0 && Resolution < Resolutions.Count)
            {
                SetResolution(Resolutions[Resolution]);
            }
        }

        void SetResolution(DisplayMode mode)
        {
            CurrentConfig.Width = mode.Width;
            CurrentConfig.Height = mode.Height;

            SaveConfig();
            ApplyConfig();
        }

        public void GetResolution()
        {
            int index = Resolutions.FindIndex(match =>
                match.Width == graphics.PreferredBackBufferWidth &&
                match.Height == graphics.PreferredBackBufferHeight);

            if (index < 0)
            {
                index = 0;
            }

            Send("getResolution", index);
        }

        static List<DisplayMode> _Modes = null;
        public List<DisplayMode> Resolutions
        {
            get
            {
                if (_Modes != null) return _Modes;

                _Modes = new List<DisplayMode>();
                foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if (_Modes.Any(existing => existing.Width == mode.Width && existing.Height == mode.Height))
                        continue;
                    else
                        _Modes.Add(mode);
                }

                _Modes.Sort((a, b) => { return a.Width.CompareTo(b.Width); });

                return _Modes;
            }
        }

        public void GetResolutionValues()
        {
            var options = new List<object>();

            for (int i = 0; i < Resolutions.Count; i++)
            {
                var mode = Resolutions[i];

                options.Add(
                    new
                    {
                        name = string.Format("{0}x{1}", mode.Width, mode.Height),
                        value = i,
                    }
                );
            }

            Send("getResolutionValues", options);
        }
    }
}
