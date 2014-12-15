using System;
using System.IO;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public abstract class MessagePlayerActionTail : MessageTail
    {
        protected static vec2 PopVec2(ref string s)
        {
            return vec2.Parse(Pop(ref s));
        }

        protected static float PopFloat(ref string s)
        {
            return float.Parse(Pop(ref s));
        }

        public MessagePlayerAction Action { get { return Outer as MessagePlayerAction; } }

        public Message MakeFullMessage(PlayerAction Action)
        {
            var Message = new Message(MessageType.PlayerAction);
            Message.Inner = new MessagePlayerAction(GameClass.World.SimStep, GameClass.World.MyPlayerNumber, GameClass.World.MyTeamNumber, Action);
            Message.Inner.Inner = this;

            return Message;
        }
    }

    public class MessageAttackMove : MessagePlayerActionTail
    {
        public vec2
            Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size;

        public MessageAttackMove(vec2 Pos, vec2 Selected_BL, vec2 Selected_Size, vec2 Destination_BL, vec2 Destination_Size)
        {
            this.Pos = Pos;
            this.Selected_BL = Selected_BL;
            this.Selected_Size = Selected_Size;
            this.Destination_BL = Destination_BL;
            this.Destination_Size = Destination_Size;
        }

        public override MessageStr EncodeHead() { return _ | Pos | Selected_BL | Selected_Size | Destination_BL | Destination_Size; }
        public static MessageAttackMove Parse(string s) { return new MessageAttackMove(PopVec2(ref s), PopVec2(ref s), PopVec2(ref s), PopVec2(ref s), PopVec2(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.AttackMove); }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do attack move at {0} : {1}", GameClass.World.SimStep, this);
            GameClass.Data.AttackMoveApply(Player.Vals[Action.PlayerNumber], Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size);
        }
    }

    public class MessagePlaceBuilding : MessagePlayerActionTail
    {
        public vec2
            Pos;

        public int
            Building;

        public MessagePlaceBuilding(vec2 Pos, int Building)
        {
            this.Pos = Pos;
            this.Building = Building;
        }

        public override MessageStr EncodeHead() { return _ | Pos | Building; }
        public static MessagePlaceBuilding Parse(string s) { return new MessagePlaceBuilding(PopVec2(ref s), PopInt(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.PlaceBuilding); }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do place building at {0} : {1}", GameClass.World.SimStep, this);
            GameClass.World.PlaceBuildingApply(Action.PlayerNumber, Action.TeamNumber, Pos, Building);
        }
    }

    public class MessageCastSpell : MessagePlayerActionTail
    {
        public int
            SpellIndex;

        public vec2
            Pos;

        public MessageCastSpell(int SpellIndex, vec2 Pos)
        {
            this.SpellIndex = SpellIndex;
            this.Pos = Pos;
        }

        public override MessageStr EncodeHead() { return _ | SpellIndex | Pos; }
        public static MessageCastSpell Parse(string s) { return new MessageCastSpell(PopInt(ref s), PopVec2(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.CastSpell); }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do cast spell at {0} for {2}/{3} : {1}", GameClass.World.SimStep, this, Action.PlayerNumber, Action.TeamNumber);

            var spell = Spells.SpellList[SpellIndex];

            spell.Apply(Action.PlayerNumber, Action.TeamNumber, Pos);
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
        public static MessageSelect Parse(string s) { return new MessageSelect(PopVec2(ref s), PopBool(ref s), PopVec2(ref s), PopVec2(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.Select); }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do select at {0}      : {1}", GameClass.World.SimStep, this);
            GameClass.Data.SelectAlongLine(v1, v2, size, deselect, true, Player.Vals[Action.PlayerNumber], true);
        }
    }
}
