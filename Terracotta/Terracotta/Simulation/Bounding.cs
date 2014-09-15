using FragSharpFramework;

namespace Terracotta
{
    public partial class BoundingTr : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Units)
        {
            vec2 uv = vertex.TexCoords * Units.Size;

            return SomethingSelected(Units[Here]) ? pack_vec2(uv) : vec(0, 0, 0, 0);
        }
    }

    public partial class BoundingBl : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Units)
        {
            vec2 uv = vertex.TexCoords * Units.Size;

            return SomethingSelected(Units[Here]) ? pack_vec2(uv) : vec(1, 1, 1, 1);
        }
    }

    public partial class _BoundingTr : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec2
                TL = unpack_vec2(PreviousLevel[Here]),
                TR = unpack_vec2(PreviousLevel[RightOne]),
                BL = unpack_vec2(PreviousLevel[UpOne]),
                BR = unpack_vec2(PreviousLevel[UpRight]);

            return pack_vec2( max(TL, TR, BL, BR) );
        }
    }

    public partial class _BoundingBl : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec2
                TL = unpack_vec2(PreviousLevel[Here]),
                TR = unpack_vec2(PreviousLevel[RightOne]),
                BL = unpack_vec2(PreviousLevel[UpOne]),
                BR = unpack_vec2(PreviousLevel[UpRight]);

            return pack_vec2(min(TL, TR, BL, BR));
        }
    }
}
