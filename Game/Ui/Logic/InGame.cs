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
        void BindMethods_InGame()
        {
            xnaObj.Bind("ActionButtonPressed", ActionButtonPressed);
            xnaObj.Bind("OnChatEnter", OnChatEnter);
        }

        void UpdateJsData()
        {
            obj["UnitCount"] = World.DataGroup.UnitCountUi;
            obj["MyPlayerInfo"] = World.MyPlayerInfo;
            obj["MyPlayerNumber"] = World.MyPlayerNumber;
            obj["PlayerInfo"] = ShowAllPlayers ? World.PlayerInfo : null;

            var json = Jsonify(obj);
            SendDict("update", obj);
        }

        void UpdateParams()
        {
            var obj = new Dictionary<string, object>();
            obj["Spells"] = Spells.SpellDict;
            obj["Buildings"] = World.MyPlayerInfo.Params.Buildings;

            SendDict("setParams", obj);
        }

        void UpdateShow()
        {
            var obj = new Dictionary<string, object>();
            obj["ShowChat"] = ShowChat;
            obj["ShowAllPlayers"] = ShowAllPlayers;

            SendDict("show", obj);
        }

        public bool ShowChat = false;
        public void ToggleChat(Toggle value = Toggle.Flip)
        {
            value.Apply(ref ShowChat);
            UpdateShow();
        }

        public bool ShowAllPlayers = false;
        public void ToggleAllPlayers(Toggle value = Toggle.Flip)
        {
            value.Apply(ref ShowAllPlayers);
            UpdateShow();
        }

        public void AddChatMessage(string name, string message)
        {
            var obj = new Dictionary<string, object>();
            obj["message"] = message;
            obj["name"] = name;

            SendDict("addChatMessage", obj);
        }

        JSValue ActionButtonPressed(object sender, JavascriptMethodEventArgs e)
        {
            try
            {
                string action = (string)e.Arguments[0];
                Console.WriteLine(action);

                try
                {
                    World.Start(action);
                }
                catch
                {
                    Console.WriteLine("Unrecognized action {0}", action);
                }
            }
            catch
            {
                Console.WriteLine("Action did not specify a name:string.");
            }

            return JSValue.Null;
        }

        JSValue OnChatEnter(object sender, JavascriptMethodEventArgs e)
        {
            string message = e.Arguments[0];

            if (message != null && message.Length > 0)
            {
                Console.WriteLine("ui chat message: " + message);
                Networking.ToServer(new MessageChat(true, Game.PlayerName(), message));
            }

            ToggleChat(Toggle.Off);

            return JSValue.Null;
        }
    }
}
