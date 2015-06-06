using System;
using System.IO;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

        byte[] bytes = new byte[1 << 16];
        public override List<string> GetMessages()
        {
            var message = SteamP2P.ReadMessage();
            
            var list = new List<string>();
            list.Add(message);

            return list;
        }

        public override void Send(string message)
        {
            SteamP2P.SendMessage(User, message);
        }

        public override void Close()
        {
        }
    }
}
