using System;
using System.IO;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Game
{
    public class GameClient
    {
        public static GameClient Server = new GameClient(IsServer: true);

        public TcpClient Client = null;
        public NetworkStream Stream = null;
        public int Index = -1;
        public int SimStep = 0;

        public bool IsServer = false;

        public bool HasLoaded = false;

        public GameClient(bool IsServer = false)
        {
            this.IsServer = IsServer;

            if (IsServer)
            {
                Index = 0;
            }
        }

        public GameClient(TcpClient Client, int Index)
        {
            this.Client = Client;
            this.Index = Index;

            this.Stream = this.Client.GetStream();
        }
    }

    public class Server
    {
        TcpListener server = null;
        byte[] bytes = new byte[1 << 16];
        
        public static List<GameClient> Clients;

        Thread ServerThread;
        bool ShouldStop = false;

        void SendReceiveThread()
        {
            Tuple<int, Message> package = null;

            while (!ShouldStop)
            {
                // Receive
                foreach (var client in Clients)
                {
                    if (client.IsServer) continue;

                    if (client.Stream.DataAvailable)
                    {
                        var messages = client.Stream.Receive(bytes);

                        foreach (var s in messages)
                        {
                            try
                            {
                                var message = Message.Parse(s);
                                message.Source = client;

                                Networking.Inbox.Enqueue(message);
                                if (Log.Receive) Console.WriteLine("(Server) Received: {0}", message);
                            }
                            catch
                            {
                                if (Log.Errors) Console.WriteLine("(Server) Received Malformed: {0}", s);
                            }
                        }
                    }
                }

                // Send
                if (Networking.Outbox.TryDequeue(out package))
                {
                    int index    = package.Item1;
                    var message  = package.Item2;
                    var encoding = message.Encode();

                    if (index < Clients.Count)
                    {
                        var client = Clients[index];

                        if (client.IsServer)
                        {
                            message.Source = GameClient.Server;
                            Networking.Inbox.Enqueue(message);
                        }
                        else
                        {                            
                            client.Stream.Send(encoding);
                        }

                        if (Log.Send) Console.WriteLine("(Server) Sent to {0}: {1}", index, encoding);
                    }
                }

                //Thread.Sleep(200);
                Thread.Sleep(1);
                //Thread.SpinWait(1);
            }
        }

        public Server()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, Program.Port);
                server.Start();

                //new Thread(ConnectThread).Start();
                Clients = new List<GameClient>();
                Clients.Add(GameClient.Server);

                for (int i = 1; i < Program.NumPlayers; i++)
                {
                    Console.Write("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    Clients.Add(new GameClient(client, i));
                    Console.WriteLine("Connected!");
                }

                Console.WriteLine("All players connected!");

                ServerThread = new Thread(SendReceiveThread);
                ServerThread.Start();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                server.Stop();

                CloseAll();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();

                CloseAll();
            }
        }

        void CloseAll()
        {
            foreach (var client in Clients)
            {
                if (client.Stream != null) client.Stream.Close();
                if (client.Client != null) client.Client.Close();
            }
        }

        public void Cleanup()
        {
            if (ServerThread != null)
            {
                ShouldStop = true;
                ServerThread.Join();
            }
            CloseAll();
        }
    }
}
