using FragSharpFramework;

namespace Game
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

    public partial class DataHash : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, PeriodicField<vec4> Noise)
        {
            data d = Data[Here];
            if (selected(d)) set_select_state(ref d, SelectState.Selected_Show);
            else set_select_state(ref d, SelectState.NotSelected_NoShow);
            //d.prior_direction_and_select = 0;
            //d = data.Nothing;

            vec4 n = Noise[Here];

            return Noise[Noise[d.xy + n.xy].xy + d.zw];
        }
    }
}
