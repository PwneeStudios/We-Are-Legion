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
    public class PlayerLobbyInfo
    {
        public bool Spectator;

        public int LobbyIndex;
        public string Name;
        public UInt64 SteamID;
        public int GamePlayer;
        public int GameTeam;
        public bool Host;
        public string Args;

        public bool HasPickedTeam = false;
    }

    public class LobbyInfo
    {
        public List<PlayerLobbyInfo> Players = new List<PlayerLobbyInfo>(4);
        public List<PlayerLobbyInfo> Spectators = new List<PlayerLobbyInfo>();
        public GameParameters Params = new GameParameters();

        public LobbyInfo()
        {
        }

        private int Increment = 0;
        public void MarkAsChanged()
        {
            Increment++;
        }

        public LobbyInfo(int NumPlayers)
        {
            for (int i = 0; i < NumPlayers; i++)
            {
                Players.Add(new PlayerLobbyInfo());
            }
        }
    }
}
