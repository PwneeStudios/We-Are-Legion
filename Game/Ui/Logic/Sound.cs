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
        void BindMethods_Sound()
        {
            xnaObj.Bind("PlaySound", PlaySound);
        }

        JSValue PlaySound(object sender, JavascriptMethodEventArgs e)
        {
            if (!awesomium.AllowMouseEvents) return JSValue.Null;

            string soundName = e.Arguments[0].ToString();

            var sound = SoundWad.Wad.FindByName(soundName);
            if (sound != null) sound.Play();

            return JSValue.Null;
        }
    }
}
