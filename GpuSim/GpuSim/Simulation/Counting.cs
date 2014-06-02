using FragSharpFramework;

namespace GpuSim
{
    public partial class Counting : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, float player, bool only_selected)
        {
            data data_here = Data[Here];
            
            vec4 output = vec4.Zero;
            if (Something(data_here))
            {
                unit unit_here = Units[Here];
                
                if (unit_here.player == player && (!only_selected || selected(data_here)))
                    output.xyz = pack_coord_3byte(1);
            }

            return output;
        }
    }

    public partial class _Counting : SimShader
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
            float count = unpack_coord(TL.xyz) + unpack_coord(TR.xyz) + unpack_coord(BL.xyz) + unpack_coord(BR.xyz);

            vec4 output = vec4.Zero;
            output.xyz = pack_coord_3byte(count);

            return output;
        }
    }
}
