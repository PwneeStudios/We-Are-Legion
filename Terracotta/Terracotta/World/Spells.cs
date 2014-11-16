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
            spell.DrawCursor = () => W.DrawCursor(Assets.SelectCircle, 30 * W.CellSize);
            spell.Execute = () => W.SpawnUnits(W.PlayerValue, W.TeamValue, UnitType.Skeleton, UnitDistribution.OnCorpses);

            Necromancer = spell = new Spell("Necromancer");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = NecroCursor;
            spell.Execute = () => W.PlaceUnit(UnitType.Necromancer);

            TerracottaArmy = spell = new Spell("Terracotta Army");
            spell.Selecting = () => W.SelectionUpdate(30 * W.CellSize, EffectSelection: false, LineSelect: false);
            spell.DrawCursor = () => W.DrawCursor(Assets.SelectCircle, 30 * W.CellSize);
            spell.Execute = () => W.SpawnUnits(W.PlayerValue, W.TeamValue, UnitType.ClaySoldier, UnitDistribution.EveryOther);
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
