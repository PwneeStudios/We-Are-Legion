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
    public static class Networking
    {
        public static ConcurrentQueue<Message>
            Inbox = new ConcurrentQueue<Message>();

        public static ConcurrentQueue<Tuple<int, Message>>
            Outbox = new ConcurrentQueue<Tuple<int, Message>>();

        static MessageStr _ = new MessageStr("");

        public static void ToServer(Message message)
        {
            message.Innermost.Immediate();

            //new Thread(() =>
            //{
            //    Thread.Sleep(100);

                Outbox.Enqueue(new Tuple<int, Message>(0, message));
            //}).Start();
        }

        public static void ToServer(MessageTail message)
        {
            ToServer(message.MakeFullMessage());
        }

        public static void ToClients(Message message)
        {
            //new Thread(() =>
            //{
            //    Thread.Sleep(100);

                if (Program.Server)
                {
                    foreach (var client in Server.Clients)
                    {
                        int index = client.Index;

                        if (Log.Outbox) Console.WriteLine("* Enqueued {0}", message);
                        Outbox.Enqueue(new Tuple<int, Message>(index, message));
                    }
                }
                else
                {
                    throw new Exception("Clients cannot send messages to clients.");
                }
            //}).Start();
        }

        public static void ToClients(MessageTail message)
        {
            ToClients(message.MakeFullMessage());
        }

        public static void Start()
        {
            if (Program.Server) new Server();
            if (Program.Client) new Client();
        }

        public static bool Send(this NetworkStream stream, string message)
        {
            try
            {
                var bytes = Encoding.ASCII.GetBytes(message + '|');
                stream.Write(bytes, 0, bytes.Length);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static List<string> Receive(this NetworkStream stream, byte[] scratch)
        {
            Int32 bytes_read = stream.Read(scratch, 0, 1 << 16);
            string data = Encoding.ASCII.GetString(scratch, 0, bytes_read);

            var messages = new List<string>(data.Split('|'));

            if (messages.Count <= 1) return messages;
            else return messages.GetRange(0, messages.Count - 1);
        }
    }
}
