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
        void BindMethods_GeneralInput()
        {
            xnaObj.Bind("OnMouseOver", OnMouseOver);
            xnaObj.Bind("OnMouseLeave", OnMouseLeave);
            xnaObj.Bind("EnableGameInput", EnableGameInput);
            xnaObj.Bind("DisableGameInput", DisableGameInput);
        }

        public bool GameInputEnabled = true;
        public bool MinimapEnabled = true;
        public bool UnitDisplayEnabled = true;
        JSValue DisableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = false;
            MinimapEnabled = false;
            UnitDisplayEnabled = false;
            return JSValue.Null;
        }

        JSValue EnableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = true;
            MinimapEnabled = true;
            UnitDisplayEnabled = true;
            return JSValue.Null;
        }

        public bool MouseOverHud = false;
        JSValue OnMouseLeave(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = false;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }

        JSValue OnMouseOver(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = true;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }
    }
}
