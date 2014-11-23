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

        void ReceiveThread()
        {
            while (true)
            {
                if (stream.DataAvailable)
                {
                    var messages = stream.Receive(bytes);

                    foreach (var message in messages)
                    {
                        Networking.Inbox.Enqueue(message);
                        Console.WriteLine("(Client) Received: {0}", message);
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
                    stream.Send(message.Item2);
                    Console.WriteLine("(Client) Sent: {0}", message.Item2);
                }

                Thread.SpinWait(1);
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

                new Thread(ReceiveThread).Start();
                new Thread(SendThread).Start();
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
