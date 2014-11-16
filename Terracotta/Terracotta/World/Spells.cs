using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public class Spell : SimShader
    {
        public readonly int id;
        public readonly string Name;

        public Action Selecting;
        public Action DrawCursor;
        public Action Execute;

        static int next_id = 0;
        public Spell(string Name)
        {
            id = next_id;
            next_id++;

            Spells.Add(this);
        }
    }

    public class Spells : BaseShader
    {
        static List<Spell> SpellList = new List<Spell>();
        static World W { get { return GameClass.World; } }

        //enum Spell { None, Fireball, RaiseSkeletons, SummonNecromancer, RaiseTerracotta, Convert, Flamewall, Resurrect, CorpseExplode, }
        public static Spell Flamefield, SkeletonArmy, TerracottaArmy, Necromancer;

        public static void Initialize()
        { 
            Spell spell;

            Flamefield = spell = new Spell("Flamefield");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = FlameCursor;
            spell.Execute = () => W.Fireball();

            SkeletonArmy = spell = new Spell("Skeleton Army");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = SkeletonCursor;
            spell.Execute = () => W.RaiseSkeletons(vec(30, 30));

            Necromancer = spell = new Spell("Necromancer");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = NecroCursor;
            spell.Execute = () => W.SummonNecromancer();

            TerracottaArmy = spell = new Spell("Terracotta Army");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = TerracottaCursor;
            spell.Execute = () => W.SummonTerracotta(vec(30, 30));
        }

        static void SkeletonCursor()
        {
            float size = 30 + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Skeleton, size * W.CellSize, angle);
        }

        static void TerracottaCursor()
        {
            float size_1 = 30 + .5f * sin(2f * W.T);
            float angle_1 = 3.5f * W.T;
            W.DrawCursor(Assets.AoE_Terra, size_1 * W.CellSize, angle_1);

            float size_2 = 4.5f + 30 + .5f * sin(-2f * W.T);
            float angle_2 = -3.5f * W.T;
            W.DrawCursor(Assets.AoE_Terra, size_2 * W.CellSize, angle_2);
        }

        static void NecroCursor()
        {
            W.UpdateCellAvailability();

            W.DrawGridCell();
            W.DrawArrowCursor();
        }

        static void FlameCursor()
        {
            float size = 30 + .5f * cos(2f * W.T);
            float angle = 0;
            W.DrawCursor(Assets.AoE_Fire, size * W.CellSize, angle);
        }

        public static void Add(Spell spell)
        {
            SpellList.Add(spell);
        }
    }
}
