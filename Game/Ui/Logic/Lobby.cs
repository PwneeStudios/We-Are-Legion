using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using FragSharpFramework;
using SteamWrapper;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        PlayerLobbyInfo ThisPlayer
        {
            get
            {
                try
                {
                    return LobbyInfo.Players.Where(match => match.SteamID == SteamCore.PlayerId()).First();
                }
                catch
                {
                    try
                    {
                        return LobbyInfo.Spectators.Where(match => match.SteamID == SteamCore.PlayerId()).First();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        bool _MapLoading = false;
        public void SetMapLoading()
        {
            var obj = new Dict();
            obj["LobbyMapLoading"] = MapLoading;

            Send("lobbyMap", obj);
        }

        public World BlankWorld;
        public void LobbyUiCreated()
        {
            World = BlankWorld;
            GameMapName = null;
            NewMap = null;
        }

        public bool DrawMapPreview = false;
        public vec2 MapPreviewPos = vec2.Zero;
        public vec2 MapPreviewSize = vec2.Zero;
        public void DrawMapPreviewAt(float x, float y, float width, float height)
        {
            MapPreviewPos = new vec2(x, y);
            MapPreviewSize = new vec2(width, height);

            DrawMapPreview = true;
        }

        public void HideMapPreview()
        {
            DrawMapPreview = false;
        }

        Thread SetMapThread, PrevMapThread;
        bool MapLoading = false;
        string GameMapName = null;

        public void SetMap(string map_name)
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
        public void SetMapThreadFunc(string map)
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

        public void StartGame()
        {
            _StartGame();
        }

        public LobbyInfo GetLobbyInfo()
        {
            LobbyInfo info = new LobbyInfo();

            var players = SteamMatches.GetLobbyData("Players");
            info.Players = (List<PlayerLobbyInfo>)JsonConvert.DeserializeObject(players, typeof(List<PlayerLobbyInfo>));

            var spectators = SteamMatches.GetLobbyData("Spectators");
            info.Spectators = (List<PlayerLobbyInfo>)JsonConvert.DeserializeObject(spectators, typeof(List<PlayerLobbyInfo>));

            var game_params = SteamMatches.GetLobbyData("Params");
            info.Params = (GameParameters)JsonConvert.DeserializeObject(game_params, typeof(GameParameters));

            info.CommonArgs = SteamMatches.GetLobbyData("CommonArgs");

            return info;
        }

        public void _StartGame()
        {
            //var lobby_data = SteamMatches.GetLobbyData("LobbyInfo");
            //var lobby = JsonConvert.DeserializeObject(lobby_data, typeof(LobbyInfo));
            //LobbyInfo = (LobbyInfo)lobby;

            LobbyInfo = GetLobbyInfo();

            string game_started = SteamMatches.GetLobbyData("GameStarted");
            if (game_started == "true")
            {
                var player = LobbyInfo.Players.Where(match => match.SteamID != 0).First();
                var args = player.Args + ' ' + LobbyInfo.CommonArgs;

                Program.ParseOptions(args);
                Program.Spectate = true;
                Program.Server = false;
                Program.Client = true;
                Program.StartupPlayerNumber = 0;
            }
            else
            {
                var args = ThisPlayer.Args + ' ' + LobbyInfo.CommonArgs;

                Program.ParseOptions(args);
            }

            SetScenarioToLoad(Program.StartupMap);
            Networking.Start();

            SteamMatches.SetLobbyData("GameStarted", "true");
        }

        public void ResetLobby()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            SteamMatches.SetLobbyData("CountDownStarted", "");
            SteamMatches.SetLobbyData("GameStarted", "");
        }

        public void StartGameCountdown()
        {
            // Only the lobby owner can start the match.
            if (!SteamMatches.IsLobbyOwner()) return;

            // We never set the lobby to unjoinable.
            //SteamMatches.SetLobbyJoinable(false);

            SteamMatches.SetLobbyData("CountDownStarted", "true");
            SteamMatches.SetLobbyData("GameStarted", "");
        }

        public void LeaveLobby()
        {
            Console.WriteLine("leaving lobby");
            SteamMatches.LeaveLobby();
        }

        public void SendChat(string msg)
        {
            SteamMatches.SendChatMsg(msg);
        }

        public void SendAnnouncement(string message)
        {
            string msg = string.Format("%a{0}", message);
            SteamMatches.SendChatMsg(msg);
        }

        public void SelectTeam(string team)
        {
            string msg = string.Format("%t{0}", team);

            SteamMatches.SendChatMsg(msg);
        }

        public void SelectKingdom(string kingdom)
        {
            string msg = string.Format("%k{0}", kingdom);

            SteamMatches.SendChatMsg(msg);
        }

        public void Join()
        {
            SteamMatches.SendChatMsg("%j1");
        }

        public void Spectate()
        {
            SteamMatches.SendChatMsg("%j0");
        }

        bool IsHost = false;
        public void OnJoinLobby(bool result)
        {
            LobbyInfo = new LobbyInfo(Program.MaxPlayers);

            if (result)
            {
                Console.WriteLine("Failure joining the lobby.");
                
                Send("joinFailed");
                SteamMatches.SetLobbyCallbacks(null, null, null, null);

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

            IsHost = SteamMatches.IsLobbyOwner();

            SendLobbyData();
            BuildLobbyInfo(joining_player_id:SteamCore.PlayerId());

            SteamP2P.SetOnP2PSessionRequest(OnP2PSessionRequest);
            SteamP2P.SetOnP2PSessionConnectFail(OnP2PSessionConnectFail);

            string game_started = SteamMatches.GetLobbyData("GameStarted");
            if (game_started == "true")
            {
                _StartGame();
            }
        }

        public void OnP2PSessionRequest(UInt64 Player)
        {
            Console.WriteLine("Accept session with {0}", Player);
            SteamP2P.AcceptP2PSessionWithPlayer(new SteamPlayer(Player));
            GetNames();
        }

        public void OnP2PSessionConnectFail(UInt64 Player)
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
        public void BuildMapList()
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

        public void SendLobbyData()
        {
            var obj = new Dict();
            obj["SteamID"] = SteamCore.PlayerId();
            obj["Maps"] = Maps;

            string lobby_name = SteamMatches.GetLobbyData("name");
            //string lobby_info = SteamMatches.GetLobbyData("LobbyInfo");
            string lobby_info = Jsonify(GetLobbyInfo());
            
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

        public string Wrap(string s)
        {
            return MessageStr.Seperator + s + MessageStr.Seperator;
        }

        public void BuildArgs()
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            //"--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --w 1280 --h 720"
            var teams = new StringBuilder("0000");
            var kingdoms = new StringBuilder("0000");
            int num_players = 0;
            int i = 0;
            foreach (var player in LobbyInfo.Players)
            {
                if (!player.Spectator)
                {
                    teams[player.GamePlayer - 1] = player.GameTeam.ToString()[0];
                    kingdoms[i] = player.GamePlayer.ToString()[0];
                    i++;
                }

                if (player.SteamID != 0)
                {
                    num_players++;
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

            string spectators = LobbyInfo.Spectators.Count.ToString();
            foreach (var player in LobbyInfo.Spectators)
            {
                if (player.Host)
                {
                    server = player.SteamID.ToString();
                }

                spectators += ' ' + player.SteamID.ToString();
            }

            foreach (var player in LobbyInfo.Players)
            {
                ConstructArgs(teams, kingdoms, num_players, server, users, spectators, player, spectator: false);
            }

            foreach (var player in LobbyInfo.Spectators)
            {
                player.GameTeam = 0;
                player.GamePlayer = 0;

                ConstructArgs(teams, kingdoms, num_players, server, users, spectators, player, spectator: true);
            }

            ConstructCommonArgs(teams, kingdoms, num_players, server, users, spectators);
        }

        public void ConstructCommonArgs(StringBuilder teams, StringBuilder kingdoms, int num_players, string server, string users, string spectators)
        {
            string options = InTrainingLobby ? "--keep-computer-dragonlords" : "--remove-computer-dragonlords";
            string networking = string.Format("--steam-networking --steam-server {0} --steam-users {1} --steam-spectators {2}", server, users, spectators);
            string game_params = Jsonify(LobbyInfo.Params);
            string spells = Jsonify(Spells.SpellInfoDict);

            LobbyInfo.CommonArgs = string.Format("{0} --k {1} --t {2} --n {3} --map {4} --params {5} --spells {6} {7}",
                networking, kingdoms, teams, num_players, Wrap(GameMapName), Wrap(game_params), Wrap(spells), options);
        }

        public void ConstructArgs(StringBuilder teams, StringBuilder kingdoms, int num_players, string server, string users, string spectators, PlayerLobbyInfo player, bool spectator)
        {
            string type = player.Host ? "--server" : "--client";

            player.Args = string.Format("{0} --p {1}", type, player.GamePlayer);
        }

        public void BuildLobbyInfo(ulong joining_player_id = 0)
        {
            if (!SteamMatches.IsLobbyOwner()) return;

            PlayerLobbyInfo joining_player = null;

            var PrevInfo = LobbyInfo;
            LobbyInfo = new LobbyInfo(Program.MaxPlayers);

            int members = SteamMatches.GetLobbyMemberCount();
            for (int i = 0; i < members; i++)
            {
                ulong SteamID = SteamMatches.GetMemberId(i);
                PlayerLobbyInfo player = new PlayerLobbyInfo();
                player.Spectator = true;

                int index = 0;
                foreach (var prev_player in PrevInfo.Players)
                {
                    if (prev_player.SteamID == SteamID)
                    {
                        player = LobbyInfo.Players[index] = prev_player;
                        player.Spectator = false;
                    }

                    index++;
                }

                player.Name = SteamMatches.GetMemberName(i);

                if (player.Spectator)
                {
                    player.SteamID = SteamMatches.GetMemberId(i);
                    LobbyInfo.Spectators.Add(player);
                }

                if (player.SteamID == joining_player_id)
                {
                    joining_player = player;
                }
            }

            // For every player that doesn't have a kingdom/team set,
            // choose an available initial value.
            foreach (var player in LobbyInfo.Players)
            {
                if (player.GamePlayer <= 0 || player.GamePlayer > Program.MaxPlayers)
                {
                    player.GamePlayer = FirstKingdomAvailableTo(player);
                }

                if (player.GameTeam <= 0 || player.GameTeam > Program.MaxTeams)
                {
                    player.GameTeam = FirstTeamAvailableTo(player);
                    player.HasPickedTeam = true;
                }
            }

            // Set the current player to be the host.
            LobbyInfo.Players.ForEach(player => player.Host = player.SteamID == SteamCore.PlayerId());
            LobbyInfo.Spectators.ForEach(player => player.Host = player.SteamID == SteamCore.PlayerId());

            // If there is a joinging player try to add them and then rebuild.
            if (joining_player_id != 0 && joining_player != null)
            {
                TryToJoin(joining_player.Name, joining_player);
                BuildLobbyInfo();
                return;
            }

            BuildArgs();
            SetLobbyInfo();
        }

        public int FirstTeamAvailableTo(PlayerLobbyInfo player)
        {
            for (int team = 1; team <= Program.MaxTeams; team++)
            {
                if (TeamAvailableTo(team, player))
                {
                    return team;
                }
            }

            return 0;
        }

        public int FirstKingdomAvailableTo(PlayerLobbyInfo player)
        {
            for (int kingdom = 1; kingdom <= Program.MaxPlayers; kingdom++)
            {
                if (KingdomAvailableTo(kingdom, player))
                {
                    return kingdom;
                }
            }

            return 0;
        }

        public bool TeamAvailableTo(int team, PlayerLobbyInfo player)
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

        public bool KingdomAvailableTo(int kingdom, PlayerLobbyInfo player)
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

        public void SetLobbyInfo()
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

            SteamMatches.SetLobbyData("Players", Jsonify(LobbyInfo.Players));
            SteamMatches.SetLobbyData("Spectators", Jsonify(LobbyInfo.Spectators));
            SteamMatches.SetLobbyData("Params", Jsonify(LobbyInfo.Params));
            SteamMatches.SetLobbyData("CommonArgs", LobbyInfo.CommonArgs);

            SteamMatches.SetLobbyData("NumPlayers", LobbyInfo.Players.Count(_player => _player.SteamID != 0).ToString());
            SteamMatches.SetLobbyData("NumSpectators", LobbyInfo.Spectators.Count.ToString());
            SteamMatches.SetLobbyData("MaxPlayers", Program.MaxPlayers.ToString());
        }

        public void OnLobbyChatEnter(string message)
        {
            if (Program.GameStarted) return;

            if (message != null && message.Length > 0)
            {
                Console.WriteLine("lobby chat message: " + message);
                SteamMatches.SendChatMsg(message);
            }
        }

        public void OnLobbyDataUpdate()
        {
            Console.WriteLine("lobby data updated");

            if (Program.GameStarted) return;

            string map = SteamMatches.GetLobbyData("MapName");
            if (map != null && map.Length > 0)
            {
                SetMap(map);
            }

            SendLobbyData();
        }

        public void OnLobbyChatUpdate(int StateChange, UInt64 id)
        {
            Console.WriteLine("lobby chat updated");

            if (Program.GameStarted)
            {
                // Game is already started so add player to the spectator list.
                if (Program.Server && StateChange == SteamMatches.ChatMember_Entered)
                {
                    Thread.Sleep(500);
                    Networking._Server.AddSpectator(id);
                    Console.WriteLine("Spectator joined, resynchronizing network.");
                    GameClass.World.SynchronizeNetwork();
                }
            }
            else
            {
                if (!IsHost && SteamMatches.IsLobbyOwner())
                {
                    Console.WriteLine("lobby owner left");
                    GameClass.Game.Send("setScreen", "disconnected-from-lobby", new { message = "The lobby host has left. Tell them they suck." });
                }
            }

            if (StateChange == SteamMatches.ChatMember_Entered)
            {
                BuildLobbyInfo(id);
            }
            else
            {
                BuildLobbyInfo();
            }
        }

        public void OnLobbyChatMsg(string msg, UInt64 id, string name)
        {
            Console.WriteLine("chat msg = {0}", msg);

            if (Program.GameStarted) return;

            if (!ProcessAsAction(msg, id, name))
            {
                GameClass.Game.AddChatMessage(name, msg);
            }
        }

        public bool ProcessAsAction(string msg, UInt64 id, string name)
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
                PlayerLobbyInfo player = null;
                try
                {
                    player = LobbyInfo.Players.Where(_player => _player.SteamID == id).First();
                    player.Spectator = false;
                }
                catch
                {
                    player = LobbyInfo.Spectators.Where(_player => _player.SteamID == id).First();
                    player.Spectator = true;
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

                // Join message.
                if (msg[1] == 'j')
                {
                    // The numeric value for joining/spectating is 1/0.
                    if (value != 0 && value != 1) return false;

                    if (value == 0) // Player wants to Spectate.
                    {
                        if (player.Spectator) return false;

                        for (int i = 0; i < LobbyInfo.Players.Count; i++)
                        {
                            if (LobbyInfo.Players[i].SteamID == player.SteamID)
                            {
                                LobbyInfo.Players[i] = new PlayerLobbyInfo();
                            }
                        }

                        player.Spectator = true;
                        LobbyInfo.Spectators.RemoveAll(_player => _player.SteamID == player.SteamID);
                        LobbyInfo.Spectators.Add(player);

                        SendAnnouncement(name + " likes to watch.");
                    }
                    else // Player wants to Join.
                    {
                        TryToJoin(name, player);
                    }
                }
                // Change kingdom message.
                else if (msg[1] == 'k' && !player.Spectator)
                {
                    // The numeric value for player (kingdom) must be one of 1, 2, 3, 4.
                    if (value <= 0 || value > Program.MaxPlayers) return false;

                    if (KingdomAvailableTo(value, player))
                    {
                        SendAnnouncement(name + " has changed kingdoms!");
                        player.GamePlayer = value;
                    }
                }
                // Change team message.
                else if (msg[1] == 't' && !player.Spectator)
                {
                    // The numeric value for team must be one of 1, 2, 3, 4.
                    if (value <= 0 || value > Program.MaxTeams) return false;

                    if (TeamAvailableTo(value, player))
                    {
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

        public bool TryToJoin(string name, PlayerLobbyInfo player)
        {
            if (!player.Spectator) return false;

            if (!LobbyInfo.Players.Exists(_player => _player.SteamID == 0)) return false;

            var kingdom = FirstKingdomAvailableTo(player);
            var team = FirstTeamAvailableTo(player);
            if (kingdom <= 0) return false;

            player.GamePlayer = kingdom;
            player.GameTeam = team;
            player.Spectator = false;

            bool found_a_spot = false;
            for (int i = 0; i < LobbyInfo.Players.Count; i++)
            {
                if (LobbyInfo.Players[i].SteamID == 0)
                {
                    LobbyInfo.Players[i] = player;
                    found_a_spot = true;
                    break;
                }
            }

            if (!found_a_spot) return false;

            LobbyInfo.Spectators.RemoveAll(_player => _player.SteamID == player.SteamID);

            SendAnnouncement(name + " has joined the melee!");
            return true;
        }
    }
}
