using System;
using System.Linq;
using System.Collections.Generic;
using FragSharpFramework;

namespace Game
{
    public enum MessageType {
        DoneLoading, Start, LeaveGame, ServerLeft, RequestPause, RequestUnpause, Pause, Unpause,
        PlayerAction, PlayerActionAck, Bookend, StartingStep,
        NetworkDesync, GameState,
        Hash, StringHash,
    }

    public enum PlayerAction {
        ChatMessage,
        SelectAlongLine, SelectInBox, AttackMove, PlaceBuilding, CastSpell, DragonLordDeath,
    }

    public abstract class GenericMessage : SimShader
    {
        public MessageStr _ = new MessageStr("");

        public Connection _Source;
        public Connection Source
        {
            get
            {
                if (_Source == null && Outer != null) return Outer.Source;
                if (_Source != null) return _Source;
                return null;
            }

            set
            {
                _Source = value;
            }
        }

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
        public virtual void Immediate() { }
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

        protected static byte[] RestBytes(ref string s)
        {
            byte[] bytes = StringHelper.GetBytes(s);
            return bytes;
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
                case MessageType.PlayerActionAck : message.Inner = MessagePlayerActionAck.Parse(s); break;
                case MessageType.Bookend         : message.Inner = MessageBookend.Parse(s); break;
                case MessageType.GameState       : message.Inner = MessageGameState.Parse(s); break;
                case MessageType.StartingStep    : message.Inner = MessageStartingStep.Parse(s); break;
                case MessageType.Hash            : message.Inner = MessageHash.Parse(s); break;
                case MessageType.StringHash      : message.Inner = MessageStringHash.Parse(s); break;
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

        public override MessageStr EncodeHead() { return _ | SimStep; }
        public static MessageBookend Parse(string s) { return new MessageBookend(PopInt(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.Bookend, this); }

        public override void Do()
        {
            GameClass.World.ServerSimStep = SimStep;
            if (Log.DoUpdates) Console.WriteLine("   Do Bookend. Server is now at step {0}. We're at step {1}", GameClass.World.ServerSimStep, GameClass.World.SimStep);
        }
    }

    public class MessageStartingStep : MessageTail
    {
        public int SimStep;

        public MessageStartingStep(int SimStep)
        {
            this.SimStep = SimStep;
        }

        public override MessageStr EncodeHead() { return _ | SimStep; }
        public static MessageStartingStep Parse(string s) { return new MessageStartingStep(PopInt(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.StartingStep, this); }

        public override void Do()
        {
            if (Program.Server)
            {
                Source.SimStep = SimStep;

                GameClass.World.MinClientSimStep = Server.Clients
                    .Where(client => !client.HasLeft && (!client.Spectator || client.IsServer))
                    .Min(client => client.SimStep);

                if (Log.DoUpdates) Console.WriteLine("   Do StartingStep. Client {0} is now at step {1}. We're at step {2}:{3}. Min is {4}", Source.Index, SimStep, GameClass.World.SimStep, GameClass.World.ServerSimStep, GameClass.World.MinClientSimStep);
            }
            else
            {
                if (Log.Errors) Console.WriteLine("   WARNING!!!!! MessageStartingStep should never be received by a client.");
            }
        }
    }

    public class MessageHash : MessageTail
    {
        public int SimStep, Hash;

        public MessageHash(int SimStep, int Hash)
        {
            this.SimStep = SimStep;
            this.Hash = Hash;
        }

        public override MessageStr EncodeHead() { return _ | SimStep | Hash; }
        public static MessageHash Parse(string s) { return new MessageHash(PopInt(ref s), PopInt(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.Hash, this); }

        public override void Do()
        {
            if (Program.Server)
            {
                var h = GameClass.World.Hashes;

                if (!h.ContainsKey(SimStep))
                {
                    Console.WriteLine("   WARNING!!!!! Server recieved hash from simstep it has not finished yet!");
                }
                else
                {
                    if (h[SimStep] == Hash)
                    {
                        Console.WriteLine("Hash match {0} != {1}", Hash, h[SimStep]);
                    }
                    else
                    {
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("Hash mismatch! {0} != {1}", Hash, h[SimStep]);
                    }
                }
            }
            else
            {
                if (Log.Errors) Console.WriteLine("   WARNING!!!!! MessageHash should never be received by a client.");
            }
        }
    }

    public class MessageStringHash : MessageTail
    {
        public int SimStep;
        public string Hash;

        public MessageStringHash(int SimStep, string Hash)
        {
            this.SimStep = SimStep;
            this.Hash = Hash;
        }

        public override MessageStr EncodeHead() { return _ | SimStep | Hash; }
        public static MessageStringHash Parse(string s) { return new MessageStringHash(PopInt(ref s), Pop(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.StringHash, this); }

        public override void Do()
        {
            if (Program.Server)
            {
                var h = GameClass.World.StringHashes;

                if (!h.ContainsKey(SimStep))
                {
                    Console.WriteLine("   WARNING!!!!! Server recieved hash from simstep it has not finished yet!");
                }
                else
                {
                    if (h[SimStep] == Hash)
                    {
                        //Console.WriteLine("Hash match at step {0}", SimStep, Hash, h[SimStep]);
                    }
                    else
                    {
                        Console.WriteLine("Hash mismatch at step {0}! {1} != {2}", SimStep, Hash, h[SimStep]);
                        Console.WriteLine("Resynchronizing network.");
                        GameClass.World.SynchronizeNetwork();
                    }
                }
            }
            else
            {
                if (Log.Errors) Console.WriteLine("   WARNING!!!!! MessageStringHash should never be received by a client.");
            }
        }
    }

    public class MessagePlayerActionAck : MessageTail
    {
        public int ActivationSimStep = 0;

        public MessagePlayerActionAck(int ActivationSimStep, Message message)
        {
            this.ActivationSimStep = ActivationSimStep;
            this.Inner = message;
        }

        public override MessageStr EncodeHead() { return _ | ActivationSimStep; }
        public static MessagePlayerActionAck Parse(string s) { return new MessagePlayerActionAck(PopInt(ref s), Message.Parse(s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.PlayerActionAck, this); }

        public override void Do()
        {
            var q = GameClass.World.QueuedActions;

            if (!q.ContainsKey(ActivationSimStep))
            {
                q.Add(ActivationSimStep, new List<GenericMessage>());
            }

            q[ActivationSimStep].Add(this.Outer);
        }
    }

    public abstract class MessageTail : GenericMessage
    {
        public abstract Message MakeFullMessage();
    }

    public class MessagePlayerAction : GenericMessage
    {
        public int SimStep;
        public int PlayerNumber;
        public int TeamNumber;
        public PlayerAction Action;

        public float PlayerValue { get { return Player.Vals[PlayerNumber]; } }

        public MessagePlayerAction(int SimStep, int PlayerNumber, int TeamNumber, PlayerAction Action)
        {
            this.SimStep = SimStep;
            this.PlayerNumber = PlayerNumber;
            this.TeamNumber = TeamNumber;
            this.Action = Action;
        }

        public override MessageStr EncodeHead() { return _ | SimStep | PlayerNumber | TeamNumber | Action; }

        public static MessagePlayerAction Parse(string s)
        {
            var message = new MessagePlayerAction(PopInt(ref s), PopInt(ref s), PopInt(ref s), Pop<PlayerAction>(ref s));

            switch (message.Action)
            {
                case PlayerAction.SelectAlongLine: message.Inner = MessageSelectAlongLine.Parse(s); break;
                case PlayerAction.SelectInBox: message.Inner = MessageSelectInBox.Parse(s); break;
                case PlayerAction.AttackMove: message.Inner = MessageAttackMove.Parse(s); break;
                case PlayerAction.PlaceBuilding: message.Inner = MessagePlaceBuilding.Parse(s); break;
                case PlayerAction.CastSpell: message.Inner = MessageCastSpell.Parse(s); break;
                
                case PlayerAction.ChatMessage: message.Inner = MessageChat.Parse(s); break;
            }

            return message;
        }
    }

    public class MessageNetworkDesync : MessageTail
    {
        public int SimStep;

        public MessageNetworkDesync(int SimStep)
        {
            this.SimStep = SimStep;
        }

        public override MessageStr EncodeHead() { return _ | SimStep; }
        public static MessageNetworkDesync Parse(string s) { return new MessageNetworkDesync(PopInt(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.NetworkDesync, this); }

        public override void Do()
        {
            if (Log.DoUpdates) Console.WriteLine("   NetworkDesync identified at step {0} from server. We're at step {1}", SimStep, GameClass.World.SimStep);
        }
    }

    public class MessageGameState : MessageTail
    {
        public int SimStep;
        public byte[] Bytes;

        public MessageGameState(int SimStep, byte[] Bytes)
        {
            this.SimStep = SimStep;
            this.Bytes = Bytes;
        }

        public override MessageStr EncodeHead() { return _ | SimStep | Bytes; }
        public static MessageGameState Parse(string s) { return new MessageGameState(PopInt(ref s), RestBytes(ref s)); }
        public override Message MakeFullMessage() { return new Message(MessageType.GameState, this); }

        public override void Do()
        {
            if (Log.DoUpdates) Console.WriteLine("   GameState received from server at step {0} from server. We're at step {1}", SimStep, GameClass.World.SimStep);

            GameClass.World.Reload(SimStep, Bytes);
            GameClass.Game.Send("back");

            GameClass.World.DesyncPause = false;
            GameClass.World.PreventDragonLordMessageCount = 20;
            
            GameClass.Game.ToggleChatViaFlag(Toggle.Off);
        }
    }

    public class MessageStr
    {
        public string MyString = "";

        //public static char Seperator = ' ';
        public static char Seperator = (char)14;
        //public static char Seperator = '|';

        public static string s<T>(T v)
        {
            return v.ToString() + Seperator;
        }

        public static string s(vec2 v)
        {
            return v.x.ToString() + ':' + v.y.ToString() + Seperator;
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

            return new MessageStr(m.MyString + s(str));
        }

        public static MessageStr operator |(MessageStr m, byte[] bytes)
        {
            string str = StringHelper.GetString(bytes);
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

        public static MessageStr operator |(MessageStr m, float v)
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
