using FragSharpFramework;

namespace Game
{
    public partial class DoUnitSummary_1 : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, [Player.Vals] float player, [Vals.Bool] bool only_selected)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            if (player != Player.None && unit_here.player != player || only_selected && !fake_selected(data_here)) return vec4.Zero;

            return vec(
                unit_here.type == UnitType.Footman ? 1f : 0,
                unit_here.type == UnitType.DragonLord ? 1f : 0,
                unit_here.type == UnitType.Necromancer ? 1f : 0,
                unit_here.type == UnitType.Skeleton ? 1f : 0
                );
        }
    }

    public partial class DoUnitSummary_2 : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, [Player.Vals] float player, [Vals.Bool] bool only_selected)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            if (player != Player.None && unit_here.player != player || only_selected && !fake_selected(data_here)) return vec4.Zero;

            return vec(
                unit_here.type == UnitType.ClaySoldier ? 1f : 0,
                unit_here.type == UnitType.Barracks ? 1f : 0,
                unit_here.type == UnitType.GoldMine ? 1f : 0,
                unit_here.type == UnitType.JadeMine ? 1f : 0
                );
        }
    }

    public partial class CountUnitTypeForAllPlayers : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, [UnitType.Vals] float type)
        {
            data data_here = Data[Here];

            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];

                if (unit_here.type == type && !(IsUnit(type) && unit_here.anim == Anim.Die) && !(IsBuilding(type) && !IsCenter((building)(vec4)data_here)))
                {
                    if (unit_here.player == Player.One) output.x = _1;
                    if (unit_here.player == Player.Two) output.y = _1;
                    if (unit_here.player == Player.Three) output.z = _1;
                    if (unit_here.player == Player.Four) output.w = _1;
                }
            }

            return output;
        }
    }

    public partial class CountReduce_4x1byte : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec4
                TL = PreviousLevel[Here],
                TR = PreviousLevel[RightOne],
                BL = PreviousLevel[UpOne],
                BR = PreviousLevel[UpRight];

            // Aggregate 4 cells into the containing supercell
            return TL + TR + BL + BR;
        }
    }

    /// <summary>
    /// Warning: If only counting selected unit this result is /not/ network consistent.
    /// The counting of selected units depends on client-side only /fake/ selection state.
    /// Any action taken with this number must have this number sent with it in the network message.
    /// Note: Counting units without regard to selection /is/ network consistent.
    /// </summary>
    public partial class CountUnits : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, [Player.Vals] float player, bool only_selected)
        {
            data data_here = Data[Here];
            
            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];
                
                bool valid = (player == Player.None || unit_here.player == player) && (!only_selected || fake_selected(data_here));

                if (IsUnit(unit_here) && valid)
                    output.xyz = pack_coord_3byte(1);

                if (unit_here.type == UnitType.Barracks && IsCenter((building)(vec4)data_here) && valid)
                    output.w = _1;
            }

            return output;
        }
    }

    public partial class CountAllUnits : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, [Player.Vals] float player, bool only_selected)
        {
            data data_here = Data[Here];

            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];

                bool valid = (player == Player.None || unit_here.player == player) && (!only_selected || fake_selected(data_here));

                if ((IsUnit(unit_here) || IsBuilding(unit_here) && IsCenter(to_building(data_here))) && valid)
                    output.xyz = pack_coord_3byte(1);
            }

            return output;
        }
    }

    public partial class CountMovingAttackingDyingExploding : SimShader
    {
        [FragmentShader]
        ActionCount FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            ActionCount output = ActionCount.Nothing;
            if (Something(data_here) && IsUnit(unit_here))
            {
                output.UnitsMoving = data_here.change == Change.Moved ? _1 : _0;
                output.UnitsAttacking = unit_here.anim == Anim.Attack ? _1 : _0;
                output.UnitsDying = unit_here.anim == Anim.Die ? _1 : _0;
            }

            if (Something(data_here) && IsBuilding(unit_here))
            {
                output.BuildingsExploding = data_here.direction == Dir.StationaryDying ? _1 : _0;
            }

            return output;
        }
    }

    public partial class CountReduce_3byte1byte : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec4
                TL = PreviousLevel[Here],
                TR = PreviousLevel[RightOne],
                BL = PreviousLevel[UpOne],
                BR = PreviousLevel[UpRight];

            // Aggregate 4 cells into the containing supercell
            float count_3byte = unpack_val(TL.xyz) + unpack_val(TR.xyz) + unpack_val(BL.xyz) + unpack_val(BR.xyz);
            float count_1byte = TL.w + TR.w + BL.w + BR.w;

            vec4 output = vec4.Zero;
            output.xyz = pack_coord_3byte(count_3byte);
            output.w = count_1byte;

            return output;
        }
    }
}
