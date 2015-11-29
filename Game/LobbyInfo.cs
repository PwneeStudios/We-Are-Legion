using System;
using System.Collections.Generic;

namespace Game
{
    public class PlayerLobbyInfo
    {
        public bool Spectator;

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
        public string CommonArgs = "";

        public LobbyInfo()
        {
        }

        public int Increment = 0;
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
