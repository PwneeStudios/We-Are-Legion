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
            xnaObj.Bind("LobbyUiCreated", LobbyUiCreated);
            xnaObj.Bind("DrawMapPreviewAt", DrawMapPreviewAt);
            xnaObj.Bind("HideMapPreview", HideMapPreview);
            xnaObj.Bind("SetMap", SetMap);
            xnaObj.Bind("StartGame", StartGame);
            xnaObj.Bind("StartGameCountdown", StartGameCountdown);
            xnaObj.Bind("LeaveLobby", LeaveLobby);
            xnaObj.Bind("SendChat", SendChat);
            xnaObj.Bind("SelectTeam", SelectTeam);
            xnaObj.Bind("SelectKingdom", SelectKingdom);
            xnaObj.Bind("OnLobbyChatEnter", OnLobbyChatEnter);
        }

        PlayerLobbyInfo ThisPlayer
        {
            get
            {
                return LobbyInfo.Players.Where(match => match.SteamID == SteamCore.PlayerId()).First();
            }
        }

        bool _MapLoading = false;
        void SetMapLoading()
        {
            var obj = new Dict();
            obj["LobbyMapLoading"] = MapLoading;

            Send("lobbyMap", obj);
        }

        public World BlankWorld;
        JSValue LobbyUiCreated(object sender, JavascriptMethodEventArgs e)
        {
            World = BlankWorld;
            GameMapName = null;
            NewMap = null;

            return JSValue.Null;
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
            string new_map = e.Arguments[0];
            
            SetMap(new_map);

            return JSValue.Null;
        }

        void SetMap(string map_name)
        {
            Console.WriteLine("set map to {0}", map_name);

            string full_name = map_name + ".m3n";

            bool skip = false;
            if ((SetMapThread == null || !SetMapThread.IsAlive) && GameMapName == full_name) skip = true;

            if (!skip)
            {
                GameMapName = full_name;

                PrevMapThread = SetMapThread;
                SetMapThread = new Thread(() => SetMapThreadFunc(GameMapName));
                SetMapThread.Priority = ThreadPriority.Highest;
                SetMapThread.Start();
            }

            if (SteamMatches.IsLobbyOwner())
            {
                string current = SteamMatches.GetLobbyData("MapName");
                string name = map_name;

                if (name.Length > 0 && name.Contains(".m3n"))
                    name = name.Substring(0, name.Length - 4);

                if (current != map_name)
                {
                    SteamMatches.SetLobbyData("MapName", name);
                    SetLobbyInfo();
                }
            }
        }

        World NewMap = null;
        void SetMapThreadFunc(string map)
        {
            if (PrevMapThread != null && PrevMapThread.IsAlive && Thread.CurrentThread != PrevMapThread)
            {
                PrevMapThread.Join();
            }

            if (NewMap != null && NewMap.Name == map) return;
            if (map != GameMapName) return;

            NewMap = null;
            World _NewMap = new World();
            
            MapLoading = true;

            try
            {
                _NewMap.Load(Path.Combine(MapDirectory, map), Retries: 0, DataOnly: true);
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
            //Program.ParseOptions("--server                --port 13000 --p 1 --t 1234 --n 1 --map Beset.m3n   --debug");
            
            var lobby_data = SteamMatches.GetLobbyData("LobbyInfo");
            var lobby = JsonConvert.DeserializeObject(lobby_data, typeof(LobbyInfo));
            LobbyInfo = (LobbyInfo)lobby;

            Program.ParseOptions(ThisPlayer.Args);

            SetScenarioToLoad(Program.StartupMap);
            Networking.Start();

            return JSValue.Null;
        }

        JSValue StartGameCountdown(object sender, JavascriptMethodEventArgs e)
        {
            // Only the lobby owner can start the match.
            if (!SteamMatches.IsLobbyOwner()) return JSValue.Null;

            SteamMatches.SetLobbyJoinable(false);
            SteamMatches.SetLobbyData("CountDownStarted", "true");

            return JSValue.Null;
        }

        JSValue LeaveLobby(object sender, JavascriptMethodEventArgs e)
        {
            Console.WriteLine("leaving lobby");
            SteamMatches.LeaveLobby();

            return JSValue.Null;
        }

        JSValue SendChat(object sender, JavascriptMethodEventArgs e)
        {
            string msg = e.Arguments[0].ToString();
            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        void SendAnnouncement(string message)
        {
            string msg = string.Format("%a{0}", message);
            SteamMatches.SendChatMsg(msg);
        }

        JSValue SelectTeam(object sender, JavascriptMethodEventArgs e)
        {
            string team = e.Arguments[0].ToString();
            string msg = string.Format("%t{0}", team);

            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        JSValue SelectKingdom(object sender, JavascriptMethodEventArgs e)
        {
            string kingdom = e.Arguments[0].ToString();
            string msg = string.Format("%k{0}", kingdom);

            SteamMatches.SendChatMsg(msg);

            return JSValue.Null;
        }

        void OnJoinLobby(bool result)
        {
            LobbyInfo = new LobbyInfo(4);

            if (result)
            {
                Console.WriteLine("Failure joining the lobby.");
                return;
            }

            BuildMapList();

            if (SteamMatches.IsLobbyOwner())
            {
                GameMapName = null;
                MapLoading = true;
                SetMap(Maps[0]);
            }

            string lobbyName = SteamMatches.GetLobbyData("name");
            Console.WriteLine("joined lobby {0}", lobbyName);

            SendLobbyData();
            BuildLobbyInfo();

            SteamP2P.SetOnP2PSessionRequest(OnP2PSessionRequest);
            SteamP2P.SetOnP2PSessionConnectFail(OnP2PSessionConnectFail);
        }

        void OnP2PSessionRequest(UInt64 Player)
        {
            SteamP2P.AcceptP2PSessionWithPlayer(new SteamPlayer(Player));
            GetNames();
        }

        void OnP2PSessionConnectFail(UInt64 Player)
        {
            Console.WriteLine("Failed connection attempt with {0}", Player);
        }

        string MapDirectory
        {
            get
            {
                if (InTrainingLobby)
                {
                    return Path.Combine("Content", "Training");
                }
                else
                {
                    return Path.Combine("Content", "Maps");
                }
            }
        }

        List<string> Maps;
        void BuildMapList()
        {
            if (InTrainingLobby)
            {
                Maps = new List<string>(new string[] { "Easy", "Medium", "Hard", "Playground" });
                return;
            }

            Maps = new List<string>();

            foreach (string file in Directory.EnumerateFiles(MapDirectory, "*.m3n", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                Maps.Add(name);
            }
        }

        void SendLobbyData()
        {
            var obj = new Dict();
            obj["SteamID"] = SteamCore.PlayerId();
            obj["Maps"] = Maps;

            string lobby_name = SteamMatches.GetLobbyData("name");
            string lobby_info = SteamMatches.GetLobbyData("LobbyInfo");
            if (lobby_info != null && lobby_info.Length > 0 && lobby_name.Length > 0)
            {
                obj["LobbyInfo"] = lobby_info;
                obj["LobbyName"] = lobby_name;
                obj["LobbyLoading"] = false;
                obj["CountDownStarted"] = SteamMatches.GetLobbyData("CountDownStarted");
            }
            else
            {
                obj["LobbyLoading"] = true;
            }

            Send("lobby", obj);
        }

        string Wrap(string s)
        {
            return MessageStr.Seperator + s + MessageStr.Seperator;
        }

        void BuildArgs()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            //"--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --w 1280 --h 720"
            var teams = new StringBuilder("0000");
            int players = 0;
            foreach (var player in LobbyInfo.Players)
            {
                teams[player.GamePlayer - 1] = player.GameTeam.ToString()[0];

                if (player.SteamID != 0)
                {
                    players++;
                }
            }

            string server = "", users = LobbyInfo.Players.Count.ToString();
            foreach (var player in LobbyInfo.Players)
            {
                if (player.Host)
                {
                    server = player.SteamID.ToString();
                }
                
                users += ' ' + player.SteamID.ToString();
            }

            foreach (var player in LobbyInfo.Players)
            {
                string type = player.Host ? "--server" : "--client";
                string networking = string.Format("{0} --steam-networking --steam-server {1} --steam-users {2}", type, server, users);
                string game_params = Jsonify(LobbyInfo.Params);
                string spells = Jsonify(Spells.SpellInfoDict);

                player.Args = string.Format("{0} --p {1} --t {2} --n {3} --map {4} --params {5} --spells {6}", networking, player.GamePlayer, teams, players, Wrap(GameMapName), Wrap(game_params), Wrap(spells));
            }
        }

        void BuildLobbyInfo()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            var PrevInfo = LobbyInfo;
            LobbyInfo = new LobbyInfo(4);

            int members = SteamMatches.GetLobbyMemberCount();
            for (int i = 0; i < members; i++)
            {
                var player = LobbyInfo.Players[i];
                player.SteamID = SteamMatches.GetMememberId(i);

                // If a match from the previous info exists for this player,
                // use the previous data, otherwise use defaults.
                var match = PrevInfo.Players.Find(_match => _match.SteamID == player.SteamID);
                if (match == null)
                {
                    player.GamePlayer = -1;
                    player.GameTeam = -1;
                }
                else
                {
                    player = LobbyInfo.Players[i] = match;
                }

                player.Name = SteamMatches.GetMememberName(i);
            }

            // For every player that doesn't have a kingdom/team set,
            // choose an available initial value.
            foreach (var player in LobbyInfo.Players)
            {
                if (player.GamePlayer <= 0 || player.GamePlayer > 4)
                {
                    player.GamePlayer = FirstKingdomAvailableTo(player);
                }

                if (player.GameTeam <= 0 || player.GameTeam > 4)
                {
                    player.GameTeam = FirstTeamAvailableTo(player);
                    player.HasPickedTeam = true;
                }
            }

            // Set the current player to be the host.
            foreach (var player in LobbyInfo.Players)
            {
                if (player.SteamID == SteamCore.PlayerId())
                {
                    player.Host = true;
                }
                else
                {
                    player.Host = false;
                }
            }

            BuildArgs();
            SetLobbyInfo();
        }

        int FirstTeamAvailableTo(PlayerLobbyInfo player)
        {
            for (int team = 1; team <= 4; team++)
            {
                if (TeamAvailableTo(team, player))
                {
                    return team;
                }
            }

            return 0;
        }

        int FirstKingdomAvailableTo(PlayerLobbyInfo player)
        {
            for (int kingdom = 1; kingdom <= 4; kingdom++)
            {
                if (KingdomAvailableTo(kingdom, player))
                {
                    return kingdom;
                }
            }

            return 0;
        }

        bool TeamAvailableTo(int team, PlayerLobbyInfo player)
        {
            if (player.SteamID == 0)
            {
                return !LobbyInfo.Players.Exists(match =>
                    match != player && match.GameTeam == team);
            }
            else
            {
                bool PreventTeamCollissions = false;

                if (!player.HasPickedTeam) PreventTeamCollissions = true;

                if (PreventTeamCollissions)
                {
                    return !LobbyInfo.Players.Exists(match =>
                        match.SteamID != 0 && match.SteamID != player.SteamID && match.GameTeam == team);
                }
                else
                {
                    return true;
                }
            }
        }

        bool KingdomAvailableTo(int kingdom, PlayerLobbyInfo player)
        {
            if (player.SteamID == 0)
            {
                return !LobbyInfo.Players.Exists(match =>
                    match != player && match.GamePlayer == kingdom);
            }
            else
            {
                return !LobbyInfo.Players.Exists(match =>
                    match.SteamID != 0 && match.SteamID != player.SteamID && match.GamePlayer == kingdom);
            }
        }

        void SetLobbyInfo()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            // This ensures the lobby info changes and forces and update,
            // even if no relevant values changed.
            LobbyInfo.MarkAsChanged();

            // Assign unused teams/player spots to non-gamer players. (SteamID == 0).
            foreach (var player in LobbyInfo.Players)
            {
                if (player.SteamID != 0) continue;
                player.GamePlayer = FirstKingdomAvailableTo(player);
                player.GameTeam = FirstTeamAvailableTo(player);
                player.HasPickedTeam = true;
            }

            SetLobbyName();
            BuildArgs();

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

            string map = SteamMatches.GetLobbyData("MapName");
            if (map != null && map.Length > 0)
            {
                SetMap(map);
            }

            SendLobbyData();
        }

        void OnLobbyChatUpdate()
        {
            Console.WriteLine("lobby chat updated");

            BuildLobbyInfo();
        }

        void OnLobbyChatMsg(string msg, UInt64 id, string name)
        {
            Console.WriteLine("chat msg = {0}", msg);

            if (!ProcessAsAction(msg, id, name))
            {
                GameClass.Game.AddChatMessage(name, msg);
            }
        }

        bool ProcessAsAction(string msg, UInt64 id, string name)
        {
            if (msg[0] != '%') return false; // Action message must start with a '%'
            if (msg.Length < 3) return false; // Action message must have at least 3 characters, eg '%p3'

            if (msg[1] == 'a')
            {
                try
                {
                    string remainder = msg.Substring(2);

                    if (remainder != null & remainder.Length > 0)
                    {
                        GameClass.Game.AddChatMessage("Game", remainder);
                        return true;
                    }
                }
                catch
                {
                    Console.WriteLine("bad chat command : {0}", msg);
                    return false;
                }

                return false;
            }
            else
            {
                if (!SteamMatches.IsLobbyOwner())
                {
                    // Only the lobby owner can act on action messages.
                    // Everyone else should ignore them, so return true,
                    // signalling this action was already processed.
                    return true;
                }

                // Get the info for the player sending the message.
                var player = LobbyInfo.Players.Where(_player => _player.SteamID == id).First();

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

                // Update the player's info.
                if (msg[1] == 'k')
                {
                    if (KingdomAvailableTo(value, player))
                    {
                        //GameClass.Game.AddChatMessage(name, "Has changed kingdoms!");
                        SendAnnouncement(name + " has changed kingdoms!");
                        player.GamePlayer = value;
                    }
                }
                else if (msg[1] == 't')
                {
                    if (TeamAvailableTo(value, player))
                    {
                        //GameClass.Game.AddChatMessage(name, "Has changed teams!");
                        SendAnnouncement(name + " has changed teams!");
                        player.GameTeam = value;
                    }
                }
                else
                {
                    return false;
                }
            }

            SetLobbyInfo();
            return true;
        }
    }
}
