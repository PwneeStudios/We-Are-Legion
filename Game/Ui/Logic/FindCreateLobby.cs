using System;
using System.Collections.Generic;

using SteamWrapper;

namespace Game
{
    using Dict = Dictionary<string, object>;

    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        static bool InTrainingLobby = false;
        public void CreateLobby(string lobbyType, bool trainingLobby)
        {
            InTrainingLobby = trainingLobby;
            Program.GameStarted = false;

            if (!SteamCore.SteamIsConnected())
            {
                SteamCore.SetOfflineMode(true);
            }

            if (SteamMatches.InLobby())

            try { SteamMatches.LeaveLobby(); } catch { };
            SteamMatches.CreateLobby(OnCreateLobby, StringToLobbyType(lobbyType));
        }

        public void SetLobbyType(int lobbyType)
        {
            SteamMatches.SetLobbyType(lobbyType);
        }

        public static int StringToLobbyType(string _lobbyType)
        {
            switch (_lobbyType)
            {
                case "public": return SteamMatches.LobbyType_Public;
                case "friends": return SteamMatches.LobbyType_FriendsOnly;
                case "private": return SteamMatches.LobbyType_Private;
            }
            
            return SteamMatches.LobbyType_Public;
        }

        public void FindLobbies()
        {
            InTrainingLobby = false;

            if (!SteamCore.SteamIsConnected()) Offline();

            SteamMatches.FindLobbies(OnFindLobbies);
        }

        public void FindFriendLobbies()
        {
            InTrainingLobby = false;

            if (!SteamCore.SteamIsConnected()) Offline();

            SteamMatches.SetLobbyCallbacks(null, null, null, () => OnFindLobbies(false));
            SteamMatches.FindFriendLobbies(OnFindLobbies);
        }

        public void JoinLobby(int lobby)
        {
            InTrainingLobby = false;
            Program.GameStarted = false;

            if (SteamMatches.InLobby())
            
            try { SteamMatches.LeaveLobby(); } catch { };

            SteamMatches.JoinLobby(lobby, OnJoinLobby, OnLobbyChatUpdate, OnLobbyChatMsg, OnLobbyDataUpdate);
        }

        public void WatchGame(int lobby)
        {
            JoinLobby(lobby);
        }

        public void OnCreateLobby(bool result)
        {
            if (result)
            {
                Console.WriteLine("Failure during lobby creation.");
                return;
            }

            Program.GameStarted = false;

            SteamMatches.SetLobbyMemberLimit(64);
            SetLobbyName();
            SteamMatches.SetLobbyData("NumPlayers", "1");
            SteamMatches.SetLobbyData("NumSpectators", "0");
            SteamMatches.SetLobbyData("MaxPlayers", "4");

            if (InTrainingLobby) SteamMatches.SetLobbyJoinable(false);

            Console.WriteLine("Trying to join the created lobby.");
            SteamMatches.JoinCreatedLobby(OnJoinLobby, OnLobbyChatUpdate, OnLobbyChatMsg, OnLobbyDataUpdate);                
        }

        public static void SetLobbyName()
        {
            string lobby_name = "";

            if (SteamCore.InOfflineMode())
            {
                lobby_name = "Local lobby. Offline.";
            }
            else
            {
                lobby_name = string.Format("{0}'s lobby", SteamCore.PlayerName());
            }

            SteamMatches.SetLobbyData("name", lobby_name);
        }

        public void Offline()
        {
            var obj = new
            {
                Lobbies = 0,
                Online = false,
            };

            Send("lobbies", obj);
        }

        public void OnFindLobbies(bool result)
        {
            if (result)
            {
                Console.WriteLine("Failure during lobby search.");
                return;
            }

            var obj = new Dict();

            int num_lobbies = SteamMatches.NumLobbies();
            Console.WriteLine("Found {0} lobbies", num_lobbies);
            obj["NumLobbies"] = num_lobbies;

            var lobby_list = new List<Dict>(num_lobbies);
            var lobby_names = new List<String>(num_lobbies);
            for (int i = 0; i < num_lobbies; i++)
            {
                string lobby_name = SteamMatches.GetLobbyData(i, "name");
                string game_started = SteamMatches.GetLobbyData(i, "GameStarted");
                string countdown_started = SteamMatches.GetLobbyData(i, "CountDownStarted");
                if (!lobby_names.Contains(lobby_name) && !(countdown_started == "true" && game_started != "true"))
                {
                    int member_count = SteamMatches.GetLobbyMemberCount(i);
                    int capacity = SteamMatches.GetLobbyCapacity(i);

                    // Console.WriteLine("lobby {0} name: {1} members: {2}/{3}", i, lobby_name, member_count, capacity);

                    if (capacity <= 0) continue;

                    var lobby = new Dict();
                    lobby["Name"] = lobby_name;
                    lobby["Index"] = i;
                    lobby["MemberCount"] = member_count;
                    lobby["Capacity"] = capacity;
                    lobby["GameStarted"] = game_started;

                    lobby["NumPlayers"] = SteamMatches.GetLobbyData(i, "NumPlayers").MaybeInt();
                    lobby["NumSpectators"] = SteamMatches.GetLobbyData(i, "NumSpectators").MaybeInt();
                    lobby["MaxPlayers"] = SteamMatches.GetLobbyData(i, "MaxPlayers").MaybeInt();

                    lobby_names.Add(lobby_name);
                    lobby_list.Add(lobby);
                }
            }

            obj["Lobbies"] = lobby_list;
            obj["Online"] = true;

            Send("lobbies", obj);
        }
    }
}
