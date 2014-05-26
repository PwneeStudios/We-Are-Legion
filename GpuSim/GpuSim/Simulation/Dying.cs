using FragSharpFramework;

namespace GpuSim
{
    public partial class AddCorpses : SimShader
    {
        [FragmentShader]
        corpse FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data, Field<corpse> Corpses)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];
            corpse corpse_here = Corpses[Here];

            if (Something(data_here) && unit_here.anim == Anim.Dead)
            {
                corpse_here.direction = data_here.direction;
                corpse_here.type = unit_here.type;
                corpse_here.player = unit_here.player;
            }

            return corpse_here;
        }
    }
}
