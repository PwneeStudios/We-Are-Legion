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
        void BindMethods_FindLobby()
        {
            xnaObj.Bind("CreateLobby", CreateLobby);

            xnaObj.Bind("FindLobbies", FindLobbies);
            xnaObj.Bind("JoinLobby", JoinLobby);
        }

        JSValue CreateLobby(object sender, JavascriptMethodEventArgs e)
        {
            SteamMatches.CreateLobby(OnCreateLobby, SteamMatches.LobbyType_Public);
            return JSValue.Null;
        }

        JSValue FindLobbies(object sender, JavascriptMethodEventArgs e)
        {
            SteamMatches.FindLobbies(OnFindLobbies);
            return JSValue.Null;
        }

        JSValue JoinLobby(object sender, JavascriptMethodEventArgs e)
        {
            int lobby = (int)e.Arguments[0];
            SteamMatches.JoinLobby(lobby, OnJoinLobby, OnLobbyChatUpdate, OnLobbyChatMsg, OnLobbyDataUpdate);

            return JSValue.Null;
        }

        void OnCreateLobby(bool result)
        {
            if (result)
            {
                Console.WriteLine("Failure during lobby creation.");
                return;
            }

            SetLobbyName();

            Console.WriteLine("Trying to join the created lobby.");
            SteamMatches.JoinCreatedLobby(OnJoinLobby, OnLobbyChatUpdate, OnLobbyChatMsg, OnLobbyDataUpdate);
        }

        private static void SetLobbyName()
        {
            string player_name = SteamCore.PlayerName();
            string lobby_name = string.Format("{0}'s lobby", player_name);
            SteamMatches.SetLobbyData("name", lobby_name);
        }

        void OnFindLobbies(bool result)
        {
            if (result)
            {
                Console.WriteLine("Failure during lobby search.");
                return;
            }

            var obj = new Dict();

            int n = SteamMatches.NumLobbies();
            Console.WriteLine("Found {0} lobbies", n);
            obj["NumLobbies"] = n;

            var lobby_list = new List<Dict>(n);
            for (int i = 0; i < n; i++)
            {
                string lobby_name = SteamMatches.GetLobbyData(i, "name");
                int member_count = SteamMatches.GetLobbyMemberCount(i);
                int capacity = SteamMatches.GetLobbyCapacity(i);
                
                Console.WriteLine("lobby {0} name: {1} members: {2}/{3}", i, lobby_name, member_count, capacity);

                var lobby = new Dict();
                lobby["Name"] = lobby_name;
                lobby["Index"] = i;
                lobby["MemberCount"] = member_count;
                lobby["Capacity"] = capacity;

                lobby_list.Add(lobby);
            }

            obj["Lobbies"] = lobby_list;

            SendDict("lobbies", obj);
        }
    }
}
