using System;
using System.IO;
using SteamWrapper;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void Test_OnCreateLobby(bool result)
        {
            Console.WriteLine(result);

            string player_name = SteamCore.PlayerName();
            string lobby_name = string.Format("{0}'s lobby", player_name);
            SteamMatches.SetLobbyData("name", lobby_name);

            SteamMatches.FindLobbies(Test_OnFindLobbies);
        }

        void Test_OnFindLobbies(bool result)
        {
            Console.WriteLine(result);

            if (result)
            {
                Console.WriteLine("Failure during lobby search.");
                return;
            }

            int n = SteamMatches.NumLobbies();
            Console.WriteLine("Found {0} lobbies", n);

            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(SteamMatches.GetLobbyData(i, "name"));
            }

            SteamMatches.JoinCreatedLobby(Test_OnJoinLobby, Test_OnLobbyChatUpdate, Test_OnLobbyChatMsg, Test_OnLobbyDataUpdate);
        }

        void Test_OnJoinLobby(bool result)
        {
            Console.WriteLine(result);

            Console.WriteLine(SteamMatches.GetLobbyData("name"));
        }

        void Test_OnLobbyDataUpdate()
        {
            Console.WriteLine("data updated");
        }

        void Test_OnLobbyChatUpdate(int StateChange, UInt64 id)
        {
            Console.WriteLine("chat updated");
        }

        void Test_OnLobbyChatMsg(string msg, UInt64 id, string name)
        {
            Console.WriteLine("chat msg = {0}", msg);
        }

        void Test_CreateLobby()
        {
            SteamMatches.CreateLobby(Test_OnCreateLobby, SteamMatches.LobbyType_Public);
        }

        void Test_P2P()
        {
            SteamP2P.SetOnP2PSessionRequest(OnP2PSessionRequest);
            SteamP2P.SetOnP2PSessionConnectFail(OnP2PSessionConnectFail);

            while (true)
            {
                //Networking.SendString(new SteamPlayer(SteamCore.PlayerId()), "Hello");
                Networking.SendString(new SteamPlayer(76561198060676433), "Hello to 76561198060676433");
                Networking.SendString(new SteamPlayer(76561198201081585), "Hello to 76561198201081585");

                var bytes = File.ReadAllBytes("Content/Maps/Beset.m3n");
                var s = new MessageGameState(0, bytes).MakeFullMessage().ToString();
                s = s.Substring(0, 100);
                Networking.SendString(new SteamPlayer(76561198060676433), s);

                while (SteamP2P.MessageAvailable())
                {
                    var msg = Networking.ReceiveString();
                    Console.WriteLine("got message! {0}", msg);
                }
            }
        }
    }
}
