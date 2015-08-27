using System;
using System.IO;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using SteamWrapper;

namespace Game
{
    public class Server
    {
        TcpListener server = null;
        
        public static List<Connection> Clients;

        Thread ServerThread;
        bool ShouldStop = false;
        bool ShouldStopWhenEmpty = false;

        public void TemporaryJoin()
        {
            if (ServerThread == null) return;

            ServerThread.Join(600);
        }

        public void FinalSend()
        {
            if (ServerThread == null) return;

            ShouldStopWhenEmpty = true;
            ServerThread.Join();

            int count = 0;
            while (!ShouldStop)
            {
                count++;
                if (count > 1000) return;

                Thread.Sleep(1);
            }
        }

        void SendReceiveThread()
        {
            Tuple<int, Message> package = null;

            while (!ShouldStop)
            {
                lock (Clients)
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
                                //Console.WriteLine("----------------- (Server) Received: {0}", s);

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
                        int index = package.Item1;
                        var message = package.Item2;
                        var encoding = message.Encode();

                        Connection client;
                        if (index < 0)
                        {
                            client = Connection.Server;
                        }
                        else
                        {
                            client = Clients.Find(match => match.Index == index);
                            if (client == null) continue;
                        }

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
                    else
                    {
                        if (ShouldStopWhenEmpty)
                        {
                            ShouldStop = true;
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        public Server()
        {
            Clients = new List<Connection>();

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
                var msg = Networking.ReceiveString();

                foreach (var client in Clients)
                {
                    var c = client as ClientSteamConnection;
                    if (null == c || c.User.Id() != msg.Item1) continue;

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

        public void AcceptSteamPlayer(SteamPlayer player)
        {
            Networking.SendString(player, "Implicit connection acceptance.");
        }

        void StartSteamServer()
        {
            int count = 1;

            foreach (UInt64 user in Program.SteamUsers)
            {
                if (user == 0) continue;

                if (user == Program.SteamServer)
                {
                    Connection.Server.Index = count++;
                    Clients.Add(Connection.Server);
                    Console.WriteLine("Server connected to self!");

                    continue;
                }

                SteamPlayer player = new SteamPlayer(user);
                Clients.Add(new ClientSteamConnection(player, count++));
                AcceptSteamPlayer(player);

                Console.WriteLine("Connected!");
            }

            StartServerThread();
        }

        public void AddSpectator(ulong user)
        {
            lock (Clients)
            {
                // Remove identical users before adding user back in.
                Clients.RemoveAll(connection =>
                {
                    var steam_connection = connection as SteamConnection;
                    return null != steam_connection && steam_connection.User.Id() == user;
                });

                // Make sure the user's index.
                int max_index = Clients.Max(client => client.Index);
                if (max_index <= Program.MaxPlayers) max_index = Program.SpectatorIndex;

                // Add the new player.
                SteamPlayer player = new SteamPlayer(user);
                Clients.Add(new ClientSteamConnection(player, max_index));
                AcceptSteamPlayer(player);

                Console.WriteLine("Connected!");
            }
        }

        void StartTcpServer()
        {
            Clients.Add(Connection.Server);

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
