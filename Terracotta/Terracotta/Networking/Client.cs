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
    public class Client
    {
        TcpClient client = null;
        NetworkStream stream = null;
        byte[] bytes = new byte[1 << 16];

        void SendReceiveThread()
        {
            while (true)
            {
                // Receive
                if (stream.DataAvailable)
                {
                    var messages = stream.Receive(bytes);

                    foreach (var s in messages)
                    {
                        try
                        {
                            var message = Message.Parse(s);
                            message.Source = GameClient.Server;

                            Networking.Inbox.Enqueue(message);
                            Console.WriteLine("(Client) Received: {0}", message);
                        }
                        catch
                        {
                            Console.WriteLine("(Client) Received Malformed: {0}", s);
                        }
                    }
                }

                // Send
                Tuple<int, Message> outgoing = null;
                if (Networking.Outbox.TryDequeue(out outgoing))
                {
                    string encoding = outgoing.Item2.Encode();
                    stream.Send(encoding);
                    Console.WriteLine("(Client) Sent: {0}", encoding);
                }

                Thread.Sleep(1);
                //Thread.SpinWait(1);
            }
        }

        public Client()
        {
            try
            {
                Int32 port = 13000;
                IPAddress server_addr = IPAddress.Parse("127.0.0.1");

                client = new TcpClient();

                Console.Write("Waiting to connect... ");
                client.Connect(server_addr, port);
                Console.WriteLine("Connected!");

                stream = client.GetStream();

                new Thread(SendReceiveThread).Start();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                stream.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                stream.Close();
                client.Close();
            }
        }
    }
}
