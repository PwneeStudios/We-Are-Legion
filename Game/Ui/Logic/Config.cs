using System;
using System.IO;

using Windows = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

using Awesomium.Core;
using Awesomium.Core.Data;
using Awesomium.Core.Dynamic;
using AwesomiumXNA;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Script.Serialization;

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
                Fullscreen = true;
                Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

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
        /*
        void ApplyConfig(bool Activate=true)
        {
            if (CurrentConfig.Fullscreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                ApplyConfigToForm();
            }
            else
            {
                graphics.PreferredBackBufferWidth = CurrentConfig.Width;
                graphics.PreferredBackBufferHeight = CurrentConfig.Height;

                ApplyConfigToForm();
            }

            AmbientSounds.UpdateVolumes();

            if (Activate)
            {
                NeedsApplication = true;
            }
        }
        */

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
