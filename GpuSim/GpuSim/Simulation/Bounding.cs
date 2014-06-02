using FragSharpFramework;

namespace GpuSim
{
    public partial class Bounding : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Units)
        {
            vec2 uv = vertex.TexCoords;

            data
                TL = Units[Here],
                TR = Units[RightOne],
                BL = Units[UpOne],
                BR = Units[UpRight];

            if (SomethingSelected(TL) || SomethingSelected(TR) || SomethingSelected(BL) || SomethingSelected(BR))
                return vec(uv.x, uv.y, uv.x, uv.y);
            else
                return vec(0, 0, 1, 1);
        }
    }

    public partial class _Bounding : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec4
                TL = PreviousLevel[Here],
                TR = PreviousLevel[RightOne],
                BL = PreviousLevel[UpOne],
                BR = PreviousLevel[UpRight];

            vec4 output = vec4.Zero;

            output.r = max(TL.r, TR.r, BL.r, BR.r);
            output.g = max(TL.g, TR.g, BL.g, BR.g);

            output.b = min(TL.b, TR.b, BL.b, BR.b);
            output.a = min(TL.a, TR.a, BL.a, BR.a);

            return output;
        }
    }

}
