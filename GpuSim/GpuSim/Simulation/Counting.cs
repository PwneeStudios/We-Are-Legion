using FragSharpFramework;

namespace GpuSim
{
    public partial class CountGoldMines : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<building> Data, Field<unit> Units)
        {
            building data_here = Data[Here];

            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];

                if (unit_here.type == UnitType.GoldMine && IsCenter(data_here))
                {
                    if (unit_here.player == Player.One)   output.x = _1;
                    if (unit_here.player == Player.Two)   output.y = _1;
                    if (unit_here.player == Player.Three) output.z = _1;
                    if (unit_here.player == Player.Four)  output.w = _1;
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

    public partial class CountUnits : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, float player, bool only_selected)
        {
            data data_here = Data[Here];
            
            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];
                
                if (IsUnit(unit_here) && unit_here.player == player && (!only_selected || selected(data_here)))
                    output.xyz = pack_coord_3byte(1);

                if (unit_here.type == UnitType.Barracks && IsCenter((building)(vec4)data_here) && unit_here.player == player && (!only_selected || selected(data_here)))
                    output.w = _1;
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
            float count_3byte = unpack_coord(TL.xyz) + unpack_coord(TR.xyz) + unpack_coord(BL.xyz) + unpack_coord(BR.xyz);
            float count_1byte = TL.w + TR.w + BL.w + BR.w;

            vec4 output = vec4.Zero;
            output.xyz = pack_coord_3byte(count_3byte);
            output.w = count_1byte;

            return output;
        }
    }
}
