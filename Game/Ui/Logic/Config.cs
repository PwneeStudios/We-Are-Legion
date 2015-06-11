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
        class Config
        {
            public float Version = 1;
            public bool Fullscreen;
            public int Width, Height;

            public Config()
            {
#if DEBUG
                Fullscreen = false;
                Width = 1280;
                Height = 720;
#else
                FullScreen = true;
                Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#endif
            }
        }

        Config _CurrentConfig = null;
        Config CurrentConfig
        {
            get
            {
                if (_CurrentConfig != null) return _CurrentConfig;

                try
                {
                    LoadConfig();
                }
                catch
                {
                    _CurrentConfig = new Config();
                }

                return _CurrentConfig;
            }

            set
            {
                _CurrentConfig = value;
            }
        }

        void LoadConfig()
        {
            var config = File.ReadAllText(ConfigFilePath);
            _CurrentConfig = (Config)JsonConvert.DeserializeObject(config, typeof(Config));
        }

        void SaveConfig()
        {
            var config = Jsonify(CurrentConfig);
            File.WriteAllText(ConfigFilePath, config);
        }

        void ApplyConfig(bool Activate=true)
        {
            if (CurrentConfig.Fullscreen)
            {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                FakeFullscreen();
            }
            else
            {
                graphics.PreferredBackBufferWidth = CurrentConfig.Width;
                graphics.PreferredBackBufferHeight = CurrentConfig.Height;                
            }

            if (Activate)
            {
                graphics.ApplyChanges();
            }
        }
    }
}
