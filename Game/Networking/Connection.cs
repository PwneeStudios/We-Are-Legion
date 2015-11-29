using System.Net.Sockets;
using System.Collections.Generic;

using SteamWrapper;

namespace Game
{
    public class Connection
    {
        public static Connection Server = new Connection(IsServer: true);

        public int Index = -1;
        public int SimStep = 0;

        public bool IsServer = false;
        public bool HasLoaded = false;

        public bool Spectator = false;
        public bool HasLeft = false;
        public bool RequestingPause = false;

        public int Team
        {
            get
            {
                if (Index < 0 || Index > 4) return -1;

                return GameClass.World.PlayerTeams[Index];
            }
        }

        public Connection(bool IsServer = false)
        {
            this.IsServer = IsServer;

            if (IsServer)
            {
                Index = 0;
            }
        }

        public virtual bool MessageAvailable() { return false; }
        public virtual List<string> GetMessages() { return null; }
        public virtual void Send(string message) { }
        public virtual void Close() { }
        public virtual void Connect() { }
    }

    public class TcpConnection : Connection
    {
        protected TcpClient Client = null;
        protected NetworkStream Stream = null;

        public TcpConnection()
        {
            
        }

        public TcpConnection(TcpClient Client, int Index)
        {
            this.Client = Client;
            this.Index = Index;

            this.Stream = this.Client.GetStream();
        }

        public override bool MessageAvailable()
        {
            return Stream.DataAvailable;
        }

        byte[] bytes = new byte[1 << 16];
        public override List<string> GetMessages()
        {
            return Stream.Receive(bytes);   
        }

        public override void Send(string message)
        {
            Stream.Send(message);
        }

        public override void Close()
        {
            if (Stream != null) Stream.Close();
            if (Client != null) Client.Close();
        }
    }

    public class SteamConnection : Connection
    {
        public SteamPlayer User;

        public SteamConnection(SteamPlayer User, int Index = -1)
        {
            this.User = User;
            this.Index = Index;
        }

        public override bool MessageAvailable()
        {
            return SteamP2P.MessageAvailable();
        }

        public override List<string> GetMessages()
        {
            var message = Networking.ReceiveString();
            
            var list = new List<string>();
            list.Add(message.Item2);

            return list;
        }

        public override void Send(string message)
        {
            Networking.SendString(User, message);
        }

        public override void Close()
        {
        }
    }

    public class ClientSteamConnection : Connection
    {
        public SteamPlayer User;
        public List<string> Messages = new List<string>();

        public ClientSteamConnection(SteamPlayer User, int Index = -1)
        {
            this.User = User;
            this.Index = Index;
        }

        public override bool MessageAvailable()
        {
            return Messages.Count > 0;
        }

        public override List<string> GetMessages()
        {
            var messages = Messages;
            Messages = new List<string>();

            return messages;
        }

        public override void Send(string message)
        {
            Networking.SendString(User, message);
        }

        public override void Close()
        {
        }
    }
}
