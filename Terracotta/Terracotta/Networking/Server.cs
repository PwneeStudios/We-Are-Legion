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
    public class Server
    {
        TcpListener server = null;
        TcpClient client = null;
        NetworkStream stream = null;
        byte[] bytes = new byte[1 << 16];

        void ReceiveThread()
        {
            while (true)
            {
                //Thread.Sleep(1000);

                if (stream.DataAvailable)
                {
                    var messages = stream.Receive(bytes);

                    foreach (var message in messages)
                    {
                        Console.WriteLine("(Server) Received: {0}", message);

                        try
                        {
                            var msg = Message.Parse(message);
                            //Console.WriteLine("Parsed {0}", msg);

                            if (msg.Type == MessageType.PlayerAction)
                            {
                                Networking.ToClients(new Message(MessageType.PlayerActionAck, msg));
                            }

                            Networking.Inbox.Enqueue(message);
                        }
                        catch
                        {
                            Console.WriteLine("(Server) Received Malformed: {0}", message);
                        }
                    }
                }
            }
        }

        void SendThread()
        {
            Tuple<int, string> message = null;

            while (true)
            {
                if (Networking.Outbox.TryDequeue(out message))
                {
                    if (message.Item1 == 0)
                    {
                        Networking.Inbox.Enqueue(message.Item2);
                    }
                    else
                    {
                        stream.Send(message.Item2);
                    }

                    Console.WriteLine("(Server) Sent to {1}: {0}", message.Item2, message.Item1);
                }

                Thread.SpinWait(1);
            }
        }

        public Server()
        {
            try
            {
                Int32 port = 13000;
                IPAddress local_addr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(local_addr, port);
                server.Start();

                Console.Write("Waiting for a connection... ");
                client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                stream = client.GetStream();

                new Thread(ReceiveThread).Start();
                new Thread(SendThread).Start();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                server.Stop();
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
                stream.Close();
                client.Close();
            }
        }
    }
}
