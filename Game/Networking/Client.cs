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
    public class Client
    {
        TcpClient client = null;
        NetworkStream stream = null;
        byte[] bytes = new byte[1 << 16];

        Thread ClientThread;
        bool ShouldStop = false;

        void SendReceiveThread()
        {
            while (!ShouldStop)
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
                            if (Log.Receive) Console.WriteLine("(Client) Received: {0}", message);
                        }
                        catch
                        {
                            if (Log.Errors) Console.WriteLine("(Client) Received Malformed: {0}", s);
                        }
                    }
                }

                // Send
                Tuple<int, Message> outgoing = null;
                if (Networking.Outbox.TryDequeue(out outgoing))
                {
                    string encoding = outgoing.Item2.Encode();
                    stream.Send(encoding);
                    if (Log.Send) Console.WriteLine("(Client) Sent: {0}", encoding);
                }

                Thread.Sleep(1);
                //Thread.SpinWait(1);
            }
        }

        public Client()
        {
            Connect();
            
            Start();
        }

        void Start()
        {
            new Thread(SendReceiveThread).Start();
        }

        void Connect()
        {
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    //Program.IpAddress = "2604:2000:efc0:103:f878:6e49:e62d:74b0";
                    IPAddress server_addr = IPAddress.Parse(Program.IpAddress);
                    //client = new TcpClient(server_addr.AddressFamily);
                    client = new TcpClient();

                    Console.Write("Waiting to connect... " + (i > 0 ? "(attempt {0})" : ""), i);
                    client.Connect(server_addr, Program.Port);
                    Console.WriteLine("Connected!");

                    stream = client.GetStream();

                    break;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                    CloseAll();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                    CloseAll();
                }

                Thread.Sleep(100);
            }
        }

        void CloseAll()
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
        }

        public void Cleanup()
        {
            if (ClientThread != null)
            {
                ShouldStop = true;
                ClientThread.Join();
            }
            CloseAll();
        }
    }
}
