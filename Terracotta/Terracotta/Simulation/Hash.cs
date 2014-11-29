using FragSharpFramework;

namespace Terracotta
{
    public partial class HashReduce : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel, PeriodicField<vec4> Noise)
        {
            vec4
                TL = PreviousLevel[Here],
                TR = PreviousLevel[RightOne],
                BL = PreviousLevel[UpOne],
                BR = PreviousLevel[UpRight];

            // Aggregate 4 cells into the containing supercell
            return Noise[Noise[Noise[Noise[TL.xy].xy + TR.xy].xy + BL.xy].xy + BR.xy];
        }
    }

    public partial class Hash : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> F, PeriodicField<vec4> Noise)
        {
            vec4 f = F[Here];
            vec4 n = Noise[Here];

            return Noise[Noise[f.xy + n.xy].xy + f.zw];
        }
    }
}
