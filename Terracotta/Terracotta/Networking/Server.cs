using System;
using System.IO;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public class GameClient
    {
        public static GameClient Server = new GameClient(IsServer: true);

        public TcpClient Client = null;
        public NetworkStream Stream = null;
        public int Index = -1;
        public int SimStep = 0;

        public bool IsServer = false;

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

        void SendReceiveThread()
        {
            Tuple<int, Message> package = null;

            while (true)
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
                                Console.WriteLine("(Server) Received: {0}", message);
                            }
                            catch
                            {
                                Console.WriteLine("(Server) Received Malformed: {0}", s);
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

                        Console.WriteLine("(Server) Sent to {0}: {1}", index, encoding);
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
                Int32 port = 13000;
                IPAddress local_addr = IPAddress.Parse("127.0.0.1");
                //IPAddress local_addr = IPAddress.Parse("72.229.112.45");

                server = new TcpListener(local_addr, port);
                server.Start();

                //new Thread(ConnectThread).Start();
                Clients = new List<GameClient>();
                Clients.Add(GameClient.Server);

                Console.Write("Waiting for a connection... ");
                var client = server.AcceptTcpClient();
                Clients.Add(new GameClient(client, 1));
                Console.WriteLine("Connected!");

                new Thread(SendReceiveThread).Start();
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
                client.Stream.Close();
                client.Client.Close();
            }
        }
    }
}
