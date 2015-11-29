using System;

using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;

using SteamWrapper;

namespace Game
{
    public static class Networking
    {
        public static ConcurrentQueue<Message>
            Inbox = new ConcurrentQueue<Message>();

        public static ConcurrentQueue<Tuple<int, Message>>
            Outbox = new ConcurrentQueue<Tuple<int, Message>>();

        static MessageStr _ = new MessageStr("");

        public static void SendString(SteamPlayer player, string s)
        {
            var bytes = StringHelper.GetBytes(s);
            SteamP2P.SendBytes(player, bytes);
        }

        public static Tuple<ulong, string> ReceiveString()
        {
            //return SteamP2P.ReadMessage();

            var tuple = SteamP2P.ReadBytes();
            var bytes = tuple.Item2;

            var s = StringHelper.GetString(bytes);

            return new Tuple<ulong, string>(tuple.Item1, s);
        }

        public static void ToServer(Message message)
        {
            message.Innermost.Immediate();

            Outbox.Enqueue(new Tuple<int, Message>(-1, message));
        }

        public static void ToServer(MessageTail message)
        {
            ToServer(message.MakeFullMessage());
        }

        public static void ToClient(Connection client, MessageTail message)
        {
            ToClient(client, message.MakeFullMessage());
        }

        public static void ToClient(Connection client, Message message)
        {
            if (Program.Server)
            {
                int index = client.Index;

                if (Log.Outbox) Console.WriteLine("* Enqueued {0} for {1}", message, index);
                Outbox.Enqueue(new Tuple<int, Message>(index, message));
            }
            else
            {
                throw new Exception("Clients cannot send messages to clients.");
            }
        }

        public static void ToClients(Message message)
        {
            if (Program.Server)
            {
                foreach (var client in Server.Clients)
                {
                    ToClient(client, message);
                }
            }
            else
            {
                throw new Exception("Clients cannot send messages to clients.");
            }
        }

        public static void ToClients(MessageTail message)
        {
            ToClients(message.MakeFullMessage());
        }

        public static Server _Server;
        public static Client _Client;

        public static void Start()
        {
            Cleanup();

            if (Program.Server) _Server = new Server();
            if (Program.Client) _Client = new Client();
        }

        public static void FinalSend()
        {
            if (_Server != null) { _Server.FinalSend(); }
            if (_Client != null) { _Client.FinalSend(); }
            GameClass.World.ProcessInbox();

            CreateBoxes();
        }

        public static void Cleanup()
        {
            if (_Server != null) { _Server.Cleanup(); _Server = null; }
            if (_Client != null) { _Client.Cleanup(); _Client = null; }

            CreateBoxes();
        }

        public static void CreateBoxes()
        {
            lock (Outbox) Outbox = new ConcurrentQueue<Tuple<int, Message>>();
            lock (Inbox) Inbox = new ConcurrentQueue<Message>();
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
