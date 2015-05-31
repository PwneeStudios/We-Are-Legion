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
        void BindMethods_Lobby()
        {
            xnaObj.Bind("DrawMapPreviewAt", DrawMapPreviewAt);
            xnaObj.Bind("HideMapPreview", HideMapPreview);
            xnaObj.Bind("SetMap", SetMap);
            xnaObj.Bind("StartGame", StartGame);
            xnaObj.Bind("LeaveLobby", LeaveLobby);
            xnaObj.Bind("SendChat", SendChat);
            xnaObj.Bind("SelectTeam", SelectTeam);
            xnaObj.Bind("SelectKingdom", SelectKingdom);
            xnaObj.Bind("OnLobbyChatEnter", OnLobbyChatEnter);
        }

        void UpdateSettings()
        { 
            // send teams, players, game flags string, etc, all as one string
            // just do this through chat sending, parse out if starts with a '.'
            // everyone subscribes to onchat and calls this themselves
        }

        bool _MapLoading = false;
        void SetMapLoading()
        {
            var obj = new Dict();
            obj["LobbyMapLoading"] = MapLoading;

            SendDict("lobbyMap", obj);
        }

        public bool DrawMapPreview = false;
        public vec2 MapPreviewPos = vec2.Zero;
        public vec2 MapPreviewSize = vec2.Zero;
        JSValue DrawMapPreviewAt(object sender, JavascriptMethodEventArgs e)
        {
            float x = float.Parse(e.Arguments[0].ToString());
            float y = float.Parse(e.Arguments[1].ToString());
            MapPreviewPos = new vec2(x, y);

            float width = float.Parse(e.Arguments[2].ToString());
            float height = float.Parse(e.Arguments[3].ToString());
            MapPreviewSize = new vec2(width, height);

            DrawMapPreview = true;

            return JSValue.Null;
        }

        JSValue HideMapPreview(object sender, JavascriptMethodEventArgs e)
        {
            DrawMapPreview = false;
            return JSValue.Null;
        }

        Thread SetMapThread, PrevMapThread;
        bool MapLoading = false;
        string GameMapName = null;
        JSValue SetMap(object sender, JavascriptMethodEventArgs e)
        {
            string new_map = e.Arguments[0] + ".m3n";

            if ((SetMapThread == null || !SetMapThread.IsAlive) && GameMapName == new_map) return JSValue.Null;

            GameMapName = new_map;

            PrevMapThread = SetMapThread;
            SetMapThread = new Thread(() => SetMap(GameMapName));
            SetMapThread.Priority = ThreadPriority.Highest;
            SetMapThread.Start();

            return JSValue.Null;
        }

        World NewMap;
        void SetMap(string map)
        {
            if (PrevMapThread != null && PrevMapThread.IsAlive) PrevMapThread.Join();

            if (NewMap != null && NewMap.Name == map) return;
            if (map != GameMapName) return;

            NewMap = null;
            World _NewMap = new World();
            
            MapLoading = true;

            try
            {
                _NewMap.Load(Path.Combine("Content", Path.Combine("Maps", map)), Retries: 0, DataOnly: true);
            }
            catch
            {
                _NewMap.Load(Path.Combine("Content", Path.Combine("Maps", "Beset.m3n")), Retries: 0, DataOnly: true);
            }

            NewMap = _NewMap;
        }

        JSValue StartGame(object sender, JavascriptMethodEventArgs e)
        {
            //Program.ParseOptions("--client --ip 127.0.0.1 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --debug --double");
            Program.ParseOptions("--server                --port 13000 --p 1 --t 1234 --n 1 --map Beset.m3n   --debug");
            SetScenarioToLoad("Beset.m3n");
            Networking.Start();

            return JSValue.Null;
        }

        JSValue LeaveLobby(object sender, JavascriptMethodEventArgs e)
        {
            SteamMatches.LeaveLobby();

            return JSValue.Null;
        }

        JSValue SendChat(object sender, JavascriptMethodEventArgs e)
        {
            string msg = e.Arguments[0].ToString();
            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        JSValue SelectTeam(object sender, JavascriptMethodEventArgs e)
        {
            string msg = e.Arguments[0].ToString();
            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        JSValue SelectKingdom(object sender, JavascriptMethodEventArgs e)
        {
            string msg = e.Arguments[0].ToString();
            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        void OnJoinLobby(bool result)
        {
            if (result)
            {
                Console.WriteLine("Failure joining the lobby.");
                return;
            }

            BuildLobbyInfo();

            string lobbyName = SteamMatches.GetLobbyData("name");
            Console.WriteLine("joined lobby {0}", lobbyName);

            var obj = new Dict();
            obj["LobbyLoading"] = false;

            SendDict("lobby", obj);
        }

        void BuildLobbyInfo()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            LobbyInfo = new LobbyInfo();

            int members = SteamMatches.GetLobbyMemberCount();
            for (int i = 0; i < members; i++)
            {
                var player = LobbyInfo.Players[i];

                player.SteamID = SteamMatches.GetMememberId(i);
                player.Name = SteamMatches.GetMememberName(i);
            }

            SetLobbyInfo();
        }

        void SetLobbyInfo()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            string lobby_info = Jsonify(LobbyInfo);
            SteamMatches.SetLobbyData("LobbyInfo", lobby_info);
        }

        JSValue OnLobbyChatEnter(object sender, JavascriptMethodEventArgs e)
        {
            string message = e.Arguments[0];

            if (message != null && message.Length > 0)
            {
                Console.WriteLine("lobby chat message: " + message);
                SteamMatches.SendChatMsg(message);
            }

            return JSValue.Null;
        }

        void OnLobbyDataUpdate()
        {
            Console.WriteLine("lobby data updated");
        }

        void OnLobbyChatUpdate()
        {
            Console.WriteLine("lobby chat updated");

            BuildLobbyInfo();
        }

        void OnLobbyChatMsg(string msg, uint id, string name)
        {
            Console.WriteLine("chat msg = {0}", msg);

            if (!ProcessAsAction(msg, id, name))
            {
                GameClass.Game.AddChatMessage(1, msg);
            }
        }

        bool ProcessAsAction(string msg, uint id, string name)
        {
            if (msg[0] != '%') return false; // Action message must start with a '%'
            if (msg.Length < 3) return false; // Action message must have at least 3 characters, eg '%p3'

            if (!SteamMatches.IsLobbyOwner())
            {
                // Only the lobby owner can act on action messages.
                // Everyone else should ignore them, so return true,
                // signalling this action was already processed.
                return true;
            }

            // The third character in the message stores the numeric value.
            // Parse it and store in this variable.
            int value = -1;

            try
            {
                string valueStr = "" + msg[2];
                int.TryParse(valueStr, out value);
            }
            catch
            {
                Console.WriteLine("bad chat command : {0}", msg);
                return false;
            }

            // The numeric value for team/player must be one of 1, 2, 3, 4.
            if (value <= 0 || value > 4)
            {
                return false;
            }

            // Get the info for the player sending the message.
            var player = LobbyInfo.Players.Where(_player => _player.SteamID == id).First();

            // Update the player's info.
            if (msg[1] == 'p')
            {
                GameClass.Game.AddChatMessage(1, "Has changed kingdoms!");
                player.GamePlayer = value;
            }
            else if (msg[1] == 't')
            {
                GameClass.Game.AddChatMessage(1, "Has changed teams!");
                player.GameTeam = value;
            }
            else
            {
                return false;
            }

            SetLobbyInfo();
            return true;
        }
    }
}
