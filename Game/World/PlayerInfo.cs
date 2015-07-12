using System;
using System.IO;
using System.Collections.Generic;

using FragSharpFramework;

namespace Game
{
    public class PlayerUnitInfo
    {
        public int
            Count = 0, Bought = 0;

        public void Write(BinaryWriter writer)
        {
            writer.Write(Count);
            writer.Write(Bought);
        }

        public void Read(BinaryReader reader)
        {
            Count = reader.ReadInt32();
            Bought = reader.ReadInt32();
        }
    }

    public class TeamInfo : SimShader
    {
        public int DragonLordCount = 0;
        public bool Defeated = false;
    }

    public class PlayerInfo : SimShader
    {
        public string Name;
        public int Number;
        public bool Defeated = false;

        public PlayerUnitInfo
            GoldMine = new PlayerUnitInfo(),
            JadeMine = new PlayerUnitInfo(),
            Barracks = new PlayerUnitInfo();

        public int
            Gold = 0,
            Jade = 0,
            Units = 0,
            DragonLords = -1;

        public Dictionary<string, int>
            SpellCasts = new Dictionary<string, int>(),
            SpellCosts = new Dictionary<string, int>();

        public bool DragonLordAlive = false;
        public bool CreateDragonLordDeathEffect = false;
        public vec2 DragonLordDeathPos;
        public int DefeatedFrameStamp = 0;
        public bool DestroyedAllBarracks = false;

        public GameParameters Params;
        public PlayerInfo(int Number, GameParameters Params)
        {
            this.Name = "Player " + Number.ToString();
            this.Number = Number;

            this.Params = Params;

            Gold = Params.StartGold;
            Jade = Params.StartJade;

            foreach (var spell in Spells.SpellList)
            {
                SpellCasts.Add(spell.Name, 0);
                SpellCosts.Add(spell.Name, 0);
            }
        }

        public bool CanAffordSpell(Spell spell)
        {
            if (GameClass.World.MapEditorActive) return true;

            var cost = SpellCost(spell);

            return cost <= Jade;
        }

        public void BuySpell(Spell spell)
        {
            if (GameClass.World.MapEditorActive) return;

            Jade -= SpellCost(spell);
            SpellCasts[spell.Name]++;
        }

        public int SpellCost(Spell spell)
        {
            return spell.Info.JadeCost + spell.Info.JadeCostIncrease * SpellCasts[spell.Name];
        }

        public bool CanAffordBuilding(int type) { return CanAffordBuilding(_[type]); }
        public bool CanAffordBuilding(float type)
        {
            if (GameClass.World.MapEditorActive) return true;

            var cost = BuildingCost(type);

            return cost <= Gold;
        }

        public void BuyBuilding(int type) { BuyBuilding(_[type]); }
        public void BuyBuilding(float type)
        {
            if (GameClass.World.MapEditorActive) return;

            Gold -= BuildingCost(type);
            this[type].Bought++;
        }

        public void Update()
        {
            Params.Barracks.CurrentGoldCost = BuildingCost(UnitType.Barracks);
            Params.GoldMine.CurrentGoldCost = BuildingCost(UnitType.GoldMine);
            Params.JadeMine.CurrentGoldCost = BuildingCost(UnitType.JadeMine);

            foreach (var spell in Spells.SpellList)
            {
                SpellCosts[spell.Name] = SpellCost(spell);
            }
        }

        public int BuildingCost(int type) { return BuildingCost(_[type]); }
        public int BuildingCost(float type)
        {
            return Params[type].GoldCost + Params[type].CostIncrease * this[type].Bought;
        }

        public void GoldUpdate()
        {
            Gold += this[UnitType.GoldMine].Count * Params[UnitType.GoldMine].GoldPerTick +
                    this[UnitType.JadeMine].Count * Params[UnitType.JadeMine].GoldPerTick +
                    this[UnitType.Barracks].Count * Params[UnitType.Barracks].GoldPerTick;

            if (Gold < 0)
            {
                Gold = 0;
            }
        }

        public void JadeUpdate()
        {
            Jade += this[UnitType.JadeMine].Count * Params[UnitType.JadeMine].JadePerTick +
                    this[UnitType.JadeMine].Count * Params[UnitType.JadeMine].JadePerTick +
                    this[UnitType.Barracks].Count * Params[UnitType.Barracks].JadePerTick;

            if (Jade < 0)
            {
                Jade = 0;
            }
        }

        public PlayerUnitInfo this[float type]
        {
            get
            {
                if (type == UnitType.Barracks) return Barracks;
                if (type == UnitType.GoldMine) return GoldMine;
                if (type == UnitType.JadeMine) return JadeMine;
                throw new Exception("Invalid building type.");
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Gold);
            writer.Write(Jade);
            writer.Write(Units);
            writer.Write(DragonLords);
            writer.Write(DragonLordAlive);

            GoldMine.Write(writer);
            JadeMine.Write(writer);
            Barracks.Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Gold = reader.ReadInt32();
            Jade = reader.ReadInt32();
            Units = reader.ReadInt32();
            DragonLords = reader.ReadInt32();
            DragonLordAlive = reader.ReadBoolean();

            GoldMine.Read(reader);
            JadeMine.Read(reader);
            Barracks.Read(reader);            
        }

        public override string ToString()
        {
            return string.Format("Gold {0}({1}:{2}), Jade {3}({4}:{5}), Units {6}({7}:{8}) DragonLords {9}",
                Gold, this[UnitType.GoldMine].Count, this[UnitType.GoldMine].Bought,
                Jade, this[UnitType.JadeMine].Count, this[UnitType.JadeMine].Bought,
                Units, this[UnitType.Barracks].Count, this[UnitType.Barracks].Bought,
                DragonLords);
        }
    }
}
