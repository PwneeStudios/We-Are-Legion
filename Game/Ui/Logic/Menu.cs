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

            xnaObj.Bind("RequestPause", RequestPause);
            xnaObj.Bind("RequestUnpause", RequestUnpause);
        }

        static string DumpedState = "";
        JSValue DumpState(object sender, JavascriptMethodEventArgs e)
        {
            DumpedState = (string)e.Arguments[0];

            return JSValue.Null;
        }

        JSValue LeaveGame(object sender, JavascriptMethodEventArgs e)
        {
            if (Program.Server)
            {
                Networking.ToServer(new Message(MessageType.ServerLeft));
            }
            else
            {
                Networking.ToServer(new Message(MessageType.LeaveGame));
            }
            
            Networking.FinalSend();

            SteamMatches.LeaveLobby();

            ReturnToMainMenu();

            return JSValue.Null;
        }

        public void ReturnToMainMenu()
        {
            Send("removeMode", "in-game");
            Send("removeMode", "main-menu");

            Send("setMode", "main-menu");
            Send("setScreen", "game-menu");

            State = GameState.MainMenu;
            awesomium.AllowMouseEvents = true;
        }

        JSValue QuitApp(object sender, JavascriptMethodEventArgs e)
        {
            Exit();

            return JSValue.Null;
        }

        JSValue RequestPause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestPause));

            return JSValue.Null;
        }

        JSValue RequestUnpause(object sender, JavascriptMethodEventArgs e)
        {
            Networking.ToServer(new Message(MessageType.RequestUnpause));

            return JSValue.Null;
        }
    }
}
