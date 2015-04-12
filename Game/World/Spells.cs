using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Game
{
    public class Spell : SimShader
    {
        public readonly int id;
        public readonly string Name;

        public Action DrawCursor;
        public Func<bool> Execute;

        public delegate void SpellExecution(int PlayerNumer, int TeamNumber, vec2 Pos);
        public SpellExecution Apply;

        public int JadeCost = 0;
        public float TerritoryRange = float.MaxValue;

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
        public static Dictionary<string, Spell> Lookup = new Dictionary<string, Spell>();

        static World W { get { return GameClass.World; } }

        //enum Spell { None, Fireball, RaiseSkeletons, SummonNecromancer, RaiseTerracotta, Convert, Flamewall, Resurrect, CorpseExplode, }
        public static Spell Fireball, SkeletonArmy, TerracottaArmy, Necromancer;

        public const int FlameRadius = 30;
        public static vec2 FlameR = vec(FlameRadius, FlameRadius);

        public const int RaiseRadius = 85;
        public static vec2 RaiseR = vec(RaiseRadius, RaiseRadius);

        public const int TerracottaRadius = 50;
        public static vec2 TerracottaR = vec(TerracottaRadius, TerracottaRadius);

        public static void Initialize()
        { 
            Spell spell;

            Fireball = spell = new Spell("Fireball");
            spell.DrawCursor = FlameCursor;
            spell.Execute = () => W.Fireball();
            spell.Apply = (p, t, v) => W.FireballApply(p, t, v);
            spell.JadeCost = 1000;

            SkeletonArmy = spell = new Spell("Skeletons");
            spell.DrawCursor = SkeletonCursor;
            spell.Execute = () => W.RaiseSkeletons(RaiseR);
            spell.Apply = (p, t, v) => W.RaiseSkeletonsApply(p, t, v, RaiseR);
            spell.JadeCost = 1000;

            Necromancer = spell = new Spell("Necromancer");
            spell.DrawCursor = () => NecroCursor(Necromancer.TerritoryRange);
            spell.Execute = () => W.SummonNecromancer(Necromancer.TerritoryRange);
            spell.Apply = (p, t, v) => W.SummonNecromancerApply(p, t, v);
            spell.JadeCost = 1000;
            spell.TerritoryRange = _64;

            TerracottaArmy = spell = new Spell("Terracotta");
            spell.DrawCursor = TerracottaCursor;
            spell.Execute = () => W.SummonTerracotta(TerracottaR);
            spell.Apply = (p, t, v) => W.SummonTerracottaApply(p, t, v, TerracottaR);
            spell.JadeCost = 1000;
        }

        static void SkeletonCursor()
        {
            float size = RaiseRadius + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Skeleton, size * W.CellSize, angle);
        }

        static void TerracottaCursor()
        {
            float size_1 = TerracottaRadius + .5f * sin(2f * W.T);
            float angle_1 = 3.5f * W.T;
            W.DrawCursor(Assets.AoE_Terra, size_1 * W.CellSize, angle_1);

            float size_2 = 4.5f + TerracottaRadius + .5f * sin(-2f * W.T);
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
            float size = FlameRadius + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Fire, size * W.CellSize, angle);
        }

        public static void Add(Spell spell)
        {
            SpellList.Add(spell);
            Lookup.Add(spell.Name, spell);
        }
    }
}
