using System.IO;

using Windows = System.Windows.Forms;

using Newtonsoft.Json;
using Awesomium.Core;

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

        void LoadConfig()
        {
            try
            {
                var config = File.ReadAllText(ConfigFilePath);
                _CurrentConfig = (Config)JsonConvert.DeserializeObject(config, typeof(Config));
            }
            catch
            {
                _CurrentConfig = new Config();
            }
        }

        void SaveConfig()
        {
            var config = Jsonify(CurrentConfig);
            File.WriteAllText(ConfigFilePath, config);
        }

        bool NeedsApplication = false;

        void ApplyConfig(bool Activate = true)
        {
            if (CurrentConfig.Fullscreen)
            {
                ApplyConfigToForm();

                var s = Windows.Screen.FromControl(Control).Bounds;
                graphics.PreferredBackBufferWidth = s.Width;
                graphics.PreferredBackBufferHeight = s.Height;

                graphics.ApplyChanges();
            }
            else
            {
                ApplyConfigToForm();

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

        void DoActivation()
        {
            NeedsApplication = false;

            graphics.ApplyChanges();

            Send("dumpState");
            WebCore.Update();
            AwesomiumInitialize();
            WebCore.Update();
            Send("restoreState", DumpedState);
            WebCore.Update();
        }
    }
}
