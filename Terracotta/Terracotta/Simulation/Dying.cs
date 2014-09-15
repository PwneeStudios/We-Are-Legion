using FragSharpFramework;

namespace Terracotta
{
    public partial class AddCorpses : SimShader
    {
        [FragmentShader]
        corpse FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data, Field<corpse> Corpses)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];
            corpse corpse_here = Corpses[Here];

            if (Something(data_here) && unit_here.anim == Anim.Die && IsUnit(unit_here))
            {
                corpse_here.direction = data_here.direction;
                corpse_here.type = unit_here.type;
                corpse_here.player = unit_here.player;
            }

            return corpse_here;
        }
    }
}
