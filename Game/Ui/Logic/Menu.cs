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

using SteamWrapper;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_Menu()
        {
            xnaObj.Bind("LeaveGame", LeaveGame);
            xnaObj.Bind("QuitApp", QuitApp);
            xnaObj.Bind("DumpState", DumpState);
        }

        static string DumpedState = "";
        JSValue DumpState(object sender, JavascriptMethodEventArgs e)
        {
            DumpedState = (string)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue LeaveGame(object sender, JavascriptMethodEventArgs e)
        {
            SteamMatches.LeaveLobby();

            Send("removeMode", "in-game");
            Send("removeMode", "main-menu");
            
            Send("setMode", "main-menu");
            Send("setScreen", "game-menu");

            State = GameState.MainMenu;
            awesomium.AllowMouseEvents = true;

            return JSValue.Null;
        }

        JSValue QuitApp(object sender, JavascriptMethodEventArgs e)
        {
            Exit();

            return JSValue.Null;
        }
    }
}
