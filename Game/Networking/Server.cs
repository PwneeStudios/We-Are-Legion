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
    public class Server
    {
        TcpListener server = null;
        
        public static List<Connection> Clients;

        Thread ServerThread;
        bool ShouldStop = false;

        void SendReceiveThread()
        {
            Tuple<int, Message> package = null;

            while (!ShouldStop)
            {
                PreprocessMessages();

                // Receive
                foreach (var client in Clients)
                {
                    if (client.IsServer) continue;

                    if (client.MessageAvailable())
                    {
                        var messages = client.GetMessages();

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
                            message.Source = Connection.Server;
                            Networking.Inbox.Enqueue(message);
                        }
                        else
                        {                            
                            client.Send(encoding);
                        }

                        if (Log.Send) Console.WriteLine("(Server) Sent to {0}: {1}", index, encoding);
                    }
                }

                Thread.Sleep(1);
            }
        }

        public Server()
        {
            Clients = new List<Connection>();
            Clients.Add(Connection.Server);

            if (Program.SteamNetworking)
            {
                StartSteamServer();
            }
            else
            {
                StartTcpServer();
            }
        }

        void PreprocessMessages()
        {
            if (Program.SteamNetworking)
            {
                SteamPreprocessMessages();
            }
            else
            {
                TcpPreprocessMessages();
            }
        }

        void SteamPreprocessMessages()
        {
            while (SteamP2P.MessageAvailable())
            {
                var msg = SteamP2P.ReadMessage();

                foreach (var client in Clients)
                {
                    var c = client as ClientSteamConnection;
                    if (c.User.Id() != msg.Item1) continue;

                    c.Messages.Add(msg.Item2);
                }
            }
        }

        void TcpPreprocessMessages()
        { 
            
        }

        void StartServerThread()
        {
            ServerThread = new Thread(SendReceiveThread);
            ServerThread.Start();
        }

        void StartSteamServer()
        {
            int count = 1;
            foreach (UInt64 user in Program.SteamUsers)
            {
                if (user == 0 || user == Program.SteamServer) continue;

                SteamPlayer player = new SteamPlayer(user);
                Clients.Add(new ClientSteamConnection(player, count++));

                SteamP2P.SendMessage(player, "Implicit connection acceptance.");

                Console.WriteLine("Connected!");
            }

            StartServerThread();
        }

        void StartTcpServer()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, Program.Port);
                server.Start();

                for (int i = 1; i < Program.NumPlayers; i++)
                {
                    Console.Write("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    Clients.Add(new TcpConnection(client, i));
                    Console.WriteLine("Connected!");
                }

                Console.WriteLine("All players connected!");

                StartServerThread();
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
                client.Close();
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
