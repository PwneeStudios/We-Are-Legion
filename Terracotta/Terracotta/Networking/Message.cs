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
    public enum MessageType { PlayerAction, PlayerActionAck, Bookend }
    public enum PlayerAction { Select, Attack }

    public abstract class GenericMessage : SimShader
    {
        public MessageStr _ = new MessageStr("");

        GenericMessage _Inner = null, _Outer = null;

        public GenericMessage Inner
        {
            get { return _Inner; }
            set { _Inner = value; Inner.Outer = this; }
        }

        public GenericMessage Outer
        {
            get { return _Outer; }
            set { _Outer = value; }
        }

        public GenericMessage Innermost
        {
            get
            {
                if (Inner == null) return this;
                else return Inner.Innermost;
            }
        }

        public abstract MessageStr EncodeHead();
        public virtual void Do() { }

        public override string ToString()
        {
            return Encode();
        }

        public string Encode()
        {
            if (Inner == null) return EncodeHead();
            else return EncodeHead() | Inner.Encode();
        }

        protected static T ToEnum<T>(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        protected static string Pop(ref string s)
        {
            string head;
            HeadTail(s, out head, out s);
            return head;
        }

        protected static int PopInt(ref string s)
        { 
            return int.Parse(Pop(ref s));
        }

        protected static bool PopBool(ref string s)
        {
            return bool.Parse(Pop(ref s));
        }

        protected static vec2 PopVec2(ref string s)
        {
            return vec2.Parse(Pop(ref s));
        }

        protected static T Pop<T>(ref string s)
        {
            return ToEnum<T>(Pop(ref s));
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

        public Message(MessageType Type, GenericMessage SubMessage)
        {
            this.Type = Type;
            this.Inner = SubMessage;
        }

        public static Message Parse(string s)
        {
            var Type = Pop<MessageType>(ref s);
            var message = new Message(Type);

            switch (message.Type)
            {
                case MessageType.PlayerAction    : message.Inner = MessagePlayerAction.Parse(s); break;
                case MessageType.PlayerActionAck : message.Inner = Message.Parse(s); break;
                case MessageType.Bookend         : message.Inner = MessageBookend.Parse(s); break;
            }

            return message;
        }

        public override MessageStr EncodeHead()
        {
 	        return _ | Type;
        }
    }

    public class MessageBookend : MessageTail
    {
        public int SimStep;

        public MessageBookend(int SimStep)
        {
            this.SimStep = SimStep;
        }

        public override MessageStr EncodeHead()
        {
            return _ | SimStep;
        }

        public static MessageBookend Parse(string s)
        {
            int SimStep = PopInt(ref s);

            var message = new MessageBookend(SimStep);

            return message;
        }

        public override Message MakeFullMessage() { return new Message(MessageType.Bookend, this); }

        public override void Do()
        {
            GameClass.World.ServerSimStep = SimStep;
            Console.WriteLine("   Do Bookend. Server is now at step {0}. We're at step {1}", GameClass.World.ServerSimStep, GameClass.World.SimStep);
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
            int SimStep      = PopInt(ref s);
            int PlayerNumber = PopInt(ref s);
            var Action       = Pop<PlayerAction>(ref s);

            var message = new MessagePlayerAction(SimStep, PlayerNumber, Action);

            switch (message.Action)
            {
                case PlayerAction.Select: message.Inner = MessageSelect.Parse(s); break;
            }

            return message;
        }
    }

    public abstract class MessageTail : GenericMessage
    {
        public abstract Message MakeFullMessage();
    }

    public abstract class MessagePlayerActionTail : MessageTail
    {
        public MessagePlayerAction Action { get { return Outer as MessagePlayerAction; } }

        public Message MakeFullMessage(PlayerAction Action)
        {
            var Message = new Message(MessageType.PlayerAction);
            Message.Inner = new MessagePlayerAction(GameClass.World.SimStep, GameClass.World.PlayerNumber, Action);
            Message.Inner.Inner = this;

            return Message;
        }
    }

    public class MessageSelect : MessagePlayerActionTail
    {
        public vec2 size, v1, v2;
        public bool deselect;

        public MessageSelect(vec2 size, bool deselect, vec2 v1, vec2 v2)
        {
            this.size = size;
            this.deselect = deselect;
            this.v1 = v1;
            this.v2 = v2;
        }

        public override MessageStr EncodeHead() { return _ | size | deselect | v1 | v2; }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.Select); }

        public static MessageSelect Parse(string s)
        {
            vec2 size = PopVec2(ref s);
            bool deselect = PopBool(ref s);
            vec2 v1 = PopVec2(ref s);
            vec2 v2 = PopVec2(ref s);

            var message = new MessageSelect(size, deselect, v1, v2);

            return message;
        }

        public override void Do()
        {
            Console.WriteLine("   Do select");
            GameClass.Data.SelectAlongLine(v1, v2, size, deselect, true, Player.Vals[Action.PlayerNumber], true);
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

        public static MessageStr operator |(MessageStr m, bool v)
        {
            return new MessageStr(m.MyString + s(v));
        }

        public static implicit operator string(MessageStr m)
        {
            return m.MyString;
        }
    }
}
