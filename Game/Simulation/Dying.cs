using FragSharpFramework;

namespace Game
{
    public partial class AddCorpses : SimShader
    {
        [FragmentShader]
        corpse FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data, Field<corpse> Corpses, Field<magic> Magic)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];
            corpse corpse_here = Corpses[Here];
            magic magic_here = Magic[Here];

            // Removed corpses that are being raised.
            if (magic_here.raising_player != Player.None && unit_here.anim == Anim.StartRaise)
            {
                corpse_here = corpse.Nothing;
            }

            if (Something(data_here) && unit_here.anim == Anim.Die && LeavesCorpse(unit_here))
            {
                corpse_here.direction = data_here.direction;
                corpse_here.type = unit_here.type;
                corpse_here.player = unit_here.player;
            }

            return corpse_here;
        }
    }
}
