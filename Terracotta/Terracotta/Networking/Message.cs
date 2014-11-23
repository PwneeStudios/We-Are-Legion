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
    public enum MessageType { PlayerAction, PlayerActionAck, EndOfStep }
    public enum PlayerAction { Select, Attack }

    public abstract class GenericMessage
    {
        public MessageStr _ = new MessageStr("");

        public GenericMessage SubMessage = null;
        public abstract MessageStr EncodeHead();

        public override string ToString()
        {
            return Encode();
        }

        public string Encode()
        {
            if (SubMessage == null) return EncodeHead();
            else return EncodeHead() | SubMessage.Encode();
        }

        protected static T ToEnum<T>(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        protected static void HeadTail(string s, out string head, out string tail)
        {
            int i = s.IndexOf(MessageStr.Seperator);

            head = s.Substring(0, i);
            tail = s.Substring(i + 1, s.Length - (i + 1));
        }

        protected static void HeadTail(string s, out string head1, out string head2, out string tail)
        {
            HeadTail(s, out head1, out tail);
            HeadTail(tail, out head2, out tail);
        }

        protected static void HeadTail(string s, out string head1, out string head2, out string head3, out string tail)
        {
            HeadTail(s, out head1, out tail);
            HeadTail(tail, out head2, out tail);
            HeadTail(tail, out head3, out tail);
        }
    }

    public class Message : GenericMessage
    {
        public MessageType Type;
        
        public Message(MessageType Type)
        {
            this.Type = Type;
        }

        public Message(MessageType Type, Message SubMessage)
        {
            this.Type = Type;
            this.SubMessage = SubMessage;
        }

        public static Message Parse(string s)
        {
            string _Type, tail;
            HeadTail(s, out _Type, out tail);

            var Type = ToEnum<MessageType>(_Type);
            var msg = new Message(Type);

            switch (msg.Type)
            {
                case MessageType.PlayerAction: msg.SubMessage = MessagePlayerAction.Parse(tail); break;
                case MessageType.PlayerActionAck: msg.SubMessage = Message.Parse(tail); break;
            }

            return msg;
        }

        public override MessageStr EncodeHead()
        {
 	        return _ | Type;
        }
    }

    public class MessagePlayerAction : GenericMessage
    {
        public int SimStep;
        public int PlayerNumber;
        public PlayerAction Action;

        public MessagePlayerAction(int SimStep, int PlayerNumber, PlayerAction Action)
        {
            this.SimStep = SimStep;
            this.PlayerNumber = PlayerNumber;
            this.Action = Action;
        }

        public override MessageStr EncodeHead()
        {
 	        return _ | SimStep | PlayerNumber | Action;
        }

        public static MessagePlayerAction Parse(string s)
        {
            string _SimStep, _PlayerNumber, _Action, tail;
            HeadTail(s, out _SimStep, out _PlayerNumber, out _Action, out tail);

            int SimStep      = int.Parse(_SimStep);
            int PlayerNumber = int.Parse(_PlayerNumber);
            var Action       = ToEnum<PlayerAction>(_Action);

            var msg = new MessagePlayerAction(SimStep, PlayerNumber, Action);

            switch (msg.Action)
            {
                case PlayerAction.Select: msg.SubMessage = MessageSelect.Parse(tail); break;
            }

            return msg;
        }
    }

    public abstract class MessagePlayerActionTail : GenericMessage
    {
        public abstract Message MakeFullMessage();

        public Message MakeFullMessage(PlayerAction Action)
        {
            var Message = new Message(MessageType.PlayerAction);
            Message.SubMessage = new MessagePlayerAction(GameClass.World.SimStep, GameClass.World.PlayerNumber, Action);
            Message.SubMessage.SubMessage = this;

            return Message;
        }
    }

    public class MessageSelect : MessagePlayerActionTail
    {
        public vec2 v1, v2;

        public MessageSelect(vec2 v1, vec2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public override MessageStr EncodeHead() { return _ | v1 | v2; }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.Select); }

        public static MessageSelect Parse(string s)
        {
            string _v1, _v2, tail;
            HeadTail(s, out _v1, out _v2, out tail);

            vec2 v1 = vec2.Parse(_v1);
            vec2 v2 = vec2.Parse(_v2);

            var msg = new MessageSelect(v1, v2);

            return msg;
        }
    }

    public class MessageStr
    {
        public string MyString = "";

        public static char Seperator = ' ';
        public static string s<T>(T v)
        {
            return v.ToString() + Seperator;
        }

        public MessageStr(string str)
        {
            MyString = str;
        }

        public static MessageStr operator |(MessageStr m, MessageType t)
        {
            return new MessageStr(m.MyString + s(t));
        }

        public static MessageStr operator |(MessageStr m, PlayerAction t)
        {
            return new MessageStr(m.MyString + s(t));
        }

        public static MessageStr operator |(MessageStr m, string str)
        {
            if (str == null) return m;

            return new MessageStr(m.MyString + str);
        }

        public static MessageStr operator |(MessageStr m, vec2 v)
        {
            return new MessageStr(m.MyString + s(v));
        }

        public static MessageStr operator |(MessageStr m, int v)
        {
            return new MessageStr(m.MyString + s(v));
        }

        public static implicit operator string(MessageStr m)
        {
            return m.MyString;
        }
    }
}
