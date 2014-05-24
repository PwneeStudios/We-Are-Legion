using FragSharpFramework;

namespace GpuSim
{
    public partial class Counting : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Units)
        {
            vec2 uv = vertex.TexCoords;

            data
                TL = (data)Units[Here],
                TR = (data)Units[RightOne],
                BL = (data)Units[UpOne],
                BR = (data)Units[UpRight];

            // Aggregate 4 cells into the containing supercell
            vec4 output = vec4.Zero;

            float count = 0;
            if (Something(TL)) count += 1;
            if (Something(TR)) count += 1;
            if (Something(BL)) count += 1;
            if (Something(BR)) count += 1;

            output.rg = pack_coord(count);

            count = 0;
            if (SomethingSelected(TL)) count += 1;
            if (SomethingSelected(TR)) count += 1;
            if (SomethingSelected(BL)) count += 1;
            if (SomethingSelected(BR)) count += 1;

            output.ba = pack_coord(count);

            return output;
        }
    }

    public partial class _Counting : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> PreviousLevel)
        {
            vec2 uv = vertex.TexCoords;

            vec4
                TL = (vec4)PreviousLevel[Here],
                TR = (vec4)PreviousLevel[RightOne],
                BL = (vec4)PreviousLevel[UpOne],
                BR = (vec4)PreviousLevel[UpRight];

            // Aggregate 4 cells into the containing supercell
            vec2 count = unpack_vec2(TL) + unpack_vec2(TR) + unpack_vec2(BL) + unpack_vec2(BR);

            vec4 output = vec4.Zero;
            output = pack_vec2(count);

            return output;
        }
    }
}
