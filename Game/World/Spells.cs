using System;
using System.Collections.Generic;
using FragSharpFramework;

using Newtonsoft.Json;

namespace Game
{
    public class SpellInfo
    {
        public int JadeCost = 0, JadeCostIncrease = 0;
        public float TerritoryRange = float.MaxValue;        
    }

    public class Spell : SimShader
    {
        public readonly int id;
        public readonly string Name;

        [JsonIgnore] public Action DrawCursor;
        [JsonIgnore] public Func<bool> Execute;

        public delegate bool SpellExecution(int PlayerNumer, int TeamNumber, vec2 Pos);
        [JsonIgnore] public SpellExecution Apply;

        public Sound ExecutionSound;

        public SpellInfo Info
        {
            get
            {
                return Spells.SpellInfoDict[Name];
            }
        }

        static int next_id = 0;
        public Spell(string Name)
        {
            id = next_id;
            next_id++;

            this.Name = Name;

            Spells.Add(this);
        }
    }

    public class Spells : BaseShader
    {
        public static List<Spell> SpellList = new List<Spell>();
        public static Dictionary<string, Spell> SpellDict = new Dictionary<string, Spell>();
        public static Dictionary<string, SpellInfo> SpellInfoDict = new Dictionary<string, SpellInfo>();

        static World W { get { return GameClass.World; } }

        //enum Spell { None, Fireball, RaiseSkeletons, SummonNecromancer, RaiseTerracotta, Convert, Flamewall, Resurrect, CorpseExplode, }
        public static Spell Fireball, SkeletonArmy, TerracottaArmy, Necromancer;

        public const int FlameRadius = 15;
        public const int RaiseRadius = 47;
        public const int TerracottaRadius = 25;

        public static void Initialize()
        { 
            Spell spell;

            Fireball = spell = new Spell("Fireball");
            spell.DrawCursor = FlameCursor;
            spell.Execute = () => W.Fireball();
            spell.Apply = (p, t, v) => W.FireballApply(p, t, v);
            spell.Info.JadeCost = 750;
            spell.Info.JadeCostIncrease = 50;
            spell.ExecutionSound = SoundWad.Wad.FindByName("Spell_Fireball", FindStyle.NullIfNotFound);

            SkeletonArmy = spell = new Spell("Skeletons");
            spell.DrawCursor = SkeletonCursor;
            spell.Execute = () => W.RaiseSkeletons();
            spell.Apply = (p, t, v) => W.RaiseSkeletonsApply(p, t, v, RaiseRadius);
            spell.Info.JadeCost = 1000;
            spell.Info.JadeCostIncrease = 150;
            spell.ExecutionSound = SoundWad.Wad.FindByName("Spell_Skeletons", FindStyle.NullIfNotFound);

            Necromancer = spell = new Spell("Necromancer");
            spell.DrawCursor = () => NecroCursor(Necromancer.Info.TerritoryRange);
            spell.Execute = () => W.SummonNecromancer(Necromancer.Info.TerritoryRange);
            spell.Apply = (p, t, v) => W.SummonNecromancerApply(p, t, v);
            spell.Info.JadeCost = 1500;
            spell.Info.TerritoryRange = _64;
            spell.Info.JadeCostIncrease = 500;
            spell.ExecutionSound = SoundWad.Wad.FindByName("Spell_Necromancer", FindStyle.NullIfNotFound);

            TerracottaArmy = spell = new Spell("Terracotta");
            spell.DrawCursor = TerracottaCursor;
            spell.Execute = () => W.SummonTerracotta();
            spell.Apply = (p, t, v) => W.SummonTerracottaApply(p, t, v, TerracottaRadius);
            spell.Info.JadeCost = 3250;
            spell.Info.JadeCostIncrease = 1250;
            spell.ExecutionSound = SoundWad.Wad.FindByName("Spell_Terracotta", FindStyle.NullIfNotFound);
        }

        static void SkeletonCursor()
        {
            float size = 2 * RaiseRadius + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Skeleton, size * W.CellSize, angle);
        }

        static void TerracottaCursor()
        {
            float size_1 = 2 * TerracottaRadius + .5f * sin(2f * W.T);
            float angle_1 = 3.5f * W.T;
            W.DrawCursor(Assets.AoE_Terra, size_1 * W.CellSize, angle_1);

            float size_2 = 4.5f + 2 * TerracottaRadius + .5f * sin(-2f * W.T);
            float angle_2 = -3.5f * W.T;
            W.DrawCursor(Assets.AoE_Terra, size_2 * W.CellSize, angle_2);
        }

        static void NecroCursor(float TerritoryRange)
        {
            W.UpdateCellAvailability(TerritoryRange);

            W.DrawGridCell();
            W.DrawArrowCursor();
        }

        static void FlameCursor()
        {
            float size = 2 * FlameRadius + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Fire, size * W.CellSize, angle);
        }

        public static void Add(Spell spell)
        {
            SpellList.Add(spell);
            SpellDict.Add(spell.Name, spell);
            SpellInfoDict.Add(spell.Name, new SpellInfo());
        }
    }
}
