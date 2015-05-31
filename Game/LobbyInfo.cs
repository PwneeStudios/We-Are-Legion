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
        public int LobbyIndex;
        public string Name;
        public uint SteamID;
        public int GamePlayer;
        public int GameTeam;
    }

    public class LobbyInfo
    {
        public List<PlayerLobbyInfo> Players = new List<PlayerLobbyInfo>(4);
        public string MapName = "Beset";

        public LobbyInfo()
        {
            for (int i = 0; i < 4; i++)
            {
                Players.Add(new PlayerLobbyInfo());
            }
        }
    }
}
