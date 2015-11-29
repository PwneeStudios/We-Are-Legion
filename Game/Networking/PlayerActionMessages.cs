using System;
using FragSharpFramework;

namespace Game
{
    public abstract class MessagePlayerActionTail : MessageTail
    {
        protected static vec2 PopVec2(ref string s)
        {
            string vecString = Pop(ref s);

            var parts = vecString.Split(':');
            return new vec2(CoreMath.ParseFloat(parts[0]), CoreMath.ParseFloat(parts[1]));
        }

        protected static float PopFloat(ref string s)
        {
            return CoreMath.ParseFloat(Pop(ref s));
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

    public class MessageChat : MessagePlayerActionTail
    {
        public bool Global;
        public string Name;
        public string ChatMessage;

        public MessageChat(bool Global, string Name, string ChatMessage)
        {
            this.Global = Global;
            this.Name = Name;
            this.ChatMessage = ChatMessage;
        }

        public override MessageStr EncodeHead() { return _ | Global | Name | ChatMessage; }
        public static MessageChat Parse(string s) { return new MessageChat(PopBool(ref s), Pop(ref s), Pop(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.ChatMessage); }

        public override void Immediate()
        {
        }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do message chat at {0} : {1}", GameClass.World.SimStep, this);
            GameClass.Game.AddChatMessage(Name, (Global ? "[All] " : "[Team] ") + ChatMessage);
        }
    }

    public class MessageAttackMove : MessagePlayerActionTail
    {
        public vec2
            Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size;
        public float
            Filter;

        public MessageAttackMove(vec2 Pos, vec2 Selected_BL, vec2 Selected_Size, vec2 Destination_BL, vec2 Destination_Size, float Filter)
        {
            this.Pos = Pos;
            this.Selected_BL = Selected_BL;
            this.Selected_Size = Selected_Size;
            this.Destination_BL = Destination_BL;
            this.Destination_Size = Destination_Size;
            this.Filter = Filter;
        }

        public override MessageStr EncodeHead() { return _ | Pos | Selected_BL | Selected_Size | Destination_BL | Destination_Size | Filter; }
        public static MessageAttackMove Parse(string s) { return new MessageAttackMove(PopVec2(ref s), PopVec2(ref s), PopVec2(ref s), PopVec2(ref s), PopVec2(ref s), PopFloat(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.AttackMove); }

        public override void Immediate()
        {
            Sounds.GiveOrder.MaybePlay();
        }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do attack move at {0} : {1}", GameClass.World.SimStep, this);
            GameClass.Data.AttackMoveApply(Player.Vals[Action.PlayerNumber], Pos, Selected_BL, Selected_Size, Destination_BL, Destination_Size, Filter);
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
            // Use this to test quick desyncing.
            //if (!Program.Server) return;

            if (Log.Do) Console.WriteLine("   Do place building at {0} : {1}", GameClass.World.SimStep, this);
            GameClass.World.PlaceBuildingApply(Action.PlayerNumber, Action.TeamNumber, Pos, Building);

            if (GameClass.World.GridPointInView(Pos))
                Sounds.PlaceBuilding.MaybePlay();
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

            bool executed = spell.Apply(Action.PlayerNumber, Action.TeamNumber, Pos);

            if (!executed) return;

            GameClass.PlayerInfo[Action.PlayerNumber].BuySpell(spell);

            if (GameClass.World.GridPointInView(Pos))
                spell.ExecutionSound.MaybePlay();
        }
    }

    public class MessageSelectAlongLine : MessagePlayerActionTail
    {
        public vec2 size, v1, v2;
        public bool deselect;

        public MessageSelectAlongLine(vec2 size, bool deselect, vec2 v1, vec2 v2)
        {
            this.size = size;
            this.deselect = deselect;
            this.v1 = v1;
            this.v2 = v2;
        }

        public override MessageStr EncodeHead() { return _ | size | deselect | v1 | v2; }
        public static MessageSelectAlongLine Parse(string s) { return new MessageSelectAlongLine(PopVec2(ref s), PopBool(ref s), PopVec2(ref s), PopVec2(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.SelectAlongLine); }

        public override void Immediate()
        {
            GameClass.Data.SelectAlongLine(v1, v2, size, deselect, true, Player.Vals[Action.PlayerNumber], true, Fake: true);
            GameClass.Data.Building_FakeSelectionSpread();
        }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do select at {0}      : {1}", GameClass.World.SimStep, this);
            GameClass.Data.SelectAlongLine(v1, v2, size, deselect, true, Player.Vals[Action.PlayerNumber], true);
            GameClass.Data.DoUnitSummary(Action.PlayerValue, true);
        }
    }

    public class MessageSelectInBox : MessagePlayerActionTail
    {
        public bool deselect;
        public vec2 bl, tr;

        public MessageSelectInBox(bool deselect, vec2 bl, vec2 tr)
        {
            this.deselect = deselect;
            this.bl = bl;
            this.tr = tr;
        }

        public override MessageStr EncodeHead() { return _ | deselect | bl | tr; }
        public static MessageSelectInBox Parse(string s) { return new MessageSelectInBox(PopBool(ref s), PopVec2(ref s), PopVec2(ref s)); }
        public override Message MakeFullMessage() { return MakeFullMessage(PlayerAction.SelectInBox); }

        public override void Immediate()
        {
            GameClass.Data.SelectInBox(bl, tr, deselect, Player.Vals[Action.PlayerNumber], Fake: true);
            GameClass.Data.Building_FakeSelectionSpread();
            GameClass.Data.DoUnitSummary(Action.PlayerValue, true);
        }

        public override void Do()
        {
            if (Log.Do) Console.WriteLine("   Do select in box at {0}      : {1}", GameClass.World.SimStep, this);
            GameClass.Data.SelectInBox(bl, tr, deselect, Player.Vals[Action.PlayerNumber]);
        }
    }
}
