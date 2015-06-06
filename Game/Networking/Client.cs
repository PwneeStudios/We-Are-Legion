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
    public class TcpServerConnection : TcpConnection
    {
        public override void Connect()
        {
            //IPAddress server_addr = IPAddress.Parse("2604:2000:efc0:103:f878:6e49:e62d:74b0");
            IPAddress server_addr = IPAddress.Parse(Program.IpAddress);

            Client = new TcpClient();

            Client.Connect(server_addr, Program.Port);
            Console.WriteLine("Connected!");

            Stream = Client.GetStream();
        }
    }

    public class Client
    {
        Connection MyConnection = new TcpServerConnection();
        Thread ClientThread;
        bool ShouldStop = false;

        void SendReceiveThread()
        {
            while (!ShouldStop)
            {
                // Receive
                if (MyConnection.MessageAvailable())
                {
                    var messages = MyConnection.GetMessages();

                    foreach (var s in messages)
                    {
                        try
                        {
                            var message = Message.Parse(s);
                            message.Source = MyConnection;

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
                    MyConnection.Send(encoding);
                    if (Log.Send) Console.WriteLine("(Client) Sent: {0}", encoding);
                }

                Thread.Sleep(1);
            }
        }

        public Client()
        {
            Connect();
            
            Start();
        }

        void Start()
        {
            ClientThread = new Thread(SendReceiveThread);
            ClientThread.Start();
        }

        void Connect()
        {
            for (int i = 0; i < 50; i++)
            {
                try
                {
                    Console.Write("Waiting to connect... " + (i > 0 ? "(attempt {0})" : ""), i);
                    MyConnection.Connect();

                    break;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("ArgumentNullException: {0}", e);
                    MyConnection.Close();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("SocketException: {0}", e);
                    MyConnection.Close();
                }

                Thread.Sleep(100);
            }
        }

        public void Cleanup()
        {
            if (ClientThread != null)
            {
                ShouldStop = true;
                ClientThread.Join();
            }

            MyConnection.Close();
        }
    }
}
