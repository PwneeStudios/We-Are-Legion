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
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_Options()
        {
            xnaObj.Bind("SetMusicVolume", SetMusicVolume);
            xnaObj.Bind("GetMusicVolume", GetMusicVolume);
            xnaObj.Bind("SetSoundVolume", SetSoundVolume);
            xnaObj.Bind("GetSoundVolume", GetSoundVolume);
            xnaObj.Bind("SetFullscreen", SetFullscreen);
            xnaObj.Bind("GetFullscreen", GetFullscreen);
            xnaObj.Bind("GetFullscreenValues", GetFullscreenValues);
            xnaObj.Bind("SetResolution", SetResolution);
            xnaObj.Bind("GetResolution", GetResolution);
            xnaObj.Bind("GetResolutionValues", GetResolutionValues);
        }

        float ArgTo0to1(JSValue arg)
        {
            float val = (float)double.Parse(arg.ToString());
            
            if (val < 0) val = 0;
            if (val > 1) val = 1;

            return val;
        }

        JSValue SetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            CurrentConfig.SoundVolume = ArgTo0to1(e.Arguments[0]);
            SaveConfig();

            return JSValue.Null;
        }

        JSValue GetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            return CurrentConfig.SoundVolume;
        }

        JSValue SetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            CurrentConfig.MusicVolume = ArgTo0to1(e.Arguments[0]);
            SaveConfig();

            return JSValue.Null;
        }

        JSValue GetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            return CurrentConfig.MusicVolume;
        }

        JSValue SetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            CurrentConfig.Fullscreen = (bool)e.Arguments[0];
            
            SaveConfig();
            ApplyConfig();

            return JSValue.Null;
        }

        JSValue GetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            return CurrentConfig.Fullscreen;
        }

        JSValue GetFullscreenValues(object sender, JavascriptMethodEventArgs e)
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

            return Jsonify(options);
        }

        JSValue SetResolution(object sender, JavascriptMethodEventArgs e)
        {
            int Resolution = (int)e.Arguments[0];

            if (Resolution >= 0 && Resolution < Resolutions.Count)
            {
                SetResolution(Resolutions[Resolution]);
            }

            return JSValue.Null;
        }

        void SetResolution(DisplayMode mode)
        {
            CurrentConfig.Width = mode.Width;
            CurrentConfig.Height = mode.Height;

            SaveConfig();
            ApplyConfig();
        }

        JSValue GetResolution(object sender, JavascriptMethodEventArgs e)
        {
            int index = Resolutions.FindIndex(match =>
                match.Width == graphics.PreferredBackBufferWidth &&
                match.Height == graphics.PreferredBackBufferHeight);

            if (index >= 0)
            {
                return index;
            }
            else
            {
                return 0;
            }
        }

        static List<DisplayMode> _Modes = null;
        List<DisplayMode> Resolutions
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

        JSValue GetResolutionValues(object sender, JavascriptMethodEventArgs e)
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

            return Jsonify(options);
        }
    }
}
