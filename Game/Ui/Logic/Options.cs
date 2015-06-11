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

        double MusicVolume, SoundVolume;
        JSValue SetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            SoundVolume = double.Parse(e.Arguments[0].ToString());

            return JSValue.Null;
        }

        JSValue GetSoundVolume(object sender, JavascriptMethodEventArgs e)
        {
            return SoundVolume;
        }

        JSValue SetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            MusicVolume = double.Parse(e.Arguments[0].ToString());

            return JSValue.Null;
        }

        JSValue GetMusicVolume(object sender, JavascriptMethodEventArgs e)
        {
            return MusicVolume;
        }

        bool Fullscreen;
        JSValue SetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            Fullscreen = (bool)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue GetFullscreen(object sender, JavascriptMethodEventArgs e)
        {
            return Fullscreen;
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

        int Resolution = 1;
        JSValue SetResolution(object sender, JavascriptMethodEventArgs e)
        {
            Resolution = (int)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue GetResolution(object sender, JavascriptMethodEventArgs e)
        {
            return Resolution;
        }

        static List<DisplayMode> Modes;
        void GetResolutions()
        {
            Modes = new List<DisplayMode>();
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (Modes.Any(existing => existing.Width == mode.Width && existing.Height == mode.Height))
                    continue;
                else
                    Modes.Add(mode);
            }

            Modes.Sort((a, b) => { return a.Width.CompareTo(b.Width); });
        }

        JSValue GetResolutionValues(object sender, JavascriptMethodEventArgs e)
        {
            GetResolutions();

            var options = new List<object>();

            for (int i = 0; i < Modes.Count; i++)
            {
                var mode = Modes[i];

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
