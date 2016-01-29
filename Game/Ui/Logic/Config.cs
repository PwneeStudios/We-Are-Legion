using System.IO;

using Newtonsoft.Json;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        static string ConfigFilePath = "config.txt";
        public class Config
        {
            public float Version = 1;
            public bool Fullscreen;
            public int Width, Height;

            public float MusicVolume, SoundVolume;

            public Config()
            {
#if DEBUG
                Fullscreen = false;
                Width = 1280;
                Height = 720;

                MusicVolume = 0;
                SoundVolume = .5f;
#else
                Fullscreen = false;
                Width = 1280;
                Height = 720;

                //Fullscreen = true;
                //Width = GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                //Height = GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

                MusicVolume = 1;
                SoundVolume = 1;
                AmbientSounds.UpdateVolumes();
#endif
            }
        }

        Config _CurrentConfig = null;
        public Config CurrentConfig
        {
            get
            {
                if (_CurrentConfig != null) return _CurrentConfig;

                LoadConfig();

                return _CurrentConfig;
            }

            set
            {
                _CurrentConfig = value;
            }
        }

        public void LoadConfig()
        {
            try
            {
                var config = File.ReadAllText(ConfigFilePath);
                _CurrentConfig = (Config)JsonConvert.DeserializeObject(config, typeof(Config));
                if (_CurrentConfig.Fullscreen)
                {
                    _CurrentConfig.Width = GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                    _CurrentConfig.Height = GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                }
            }
            catch
            {
                _CurrentConfig = new Config();
            }
        }

        public void SaveConfig()
        {
            var config = Jsonify(CurrentConfig);
            File.WriteAllText(ConfigFilePath, config);
        }

        bool NeedsApplication = false;

        public void ApplyConfig(bool Activate = true)
        {
            if (CurrentConfig.Fullscreen)
            {
                ApplyConfigToForm();

                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            else
            {
                ApplyConfigToForm();

                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferWidth = CurrentConfig.Width;
                graphics.PreferredBackBufferHeight = CurrentConfig.Height;
                graphics.ApplyChanges();
            }

            AmbientSounds.UpdateVolumes();

            if (Activate)
            {
                NeedsApplication = true;
            }
        }

        public void DoActivation()
        {
            NeedsApplication = false;

            graphics.ApplyChanges();

            Send("dumpState");
            Send("restoreState", DumpedState);
        }
    }
}
