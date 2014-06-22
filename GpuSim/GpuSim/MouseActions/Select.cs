using FragSharpFramework;

namespace GpuSim
{
    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<unit> Select, bool Deselect, float action)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            unit select = Select[Here];

            // If the player unit here matches the 
            if (select.type > 0 && (select.player == Player.None || unit_here.player == select.player))
            {
                set_selected(ref data_here, true);
            }
            else
            {
                if (Deselect)
                    set_selected(ref data_here, false);
            }

            if (Something(data_here) && IsUnit(unit_here) && selected(data_here) && action < UnitAction.NoChange)
            {
                data_here.action = action;
            }

            return data_here;
        }
    }

    public partial class DataDrawMouse : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, PointSampler data_texture, float player)
        {
            unit d = unit.Nothing;

            if (data_texture[Here].a > 0)
            {
                d.type = UnitType.Footman;
                d.player = player;

                // Note: Unlike other data and simulation shaders, we do need to set the alpha component for this channel.
                // The reason is that we will be drawing multiple mouse datas onto the same render target, with potential overlapping.
                d.a = 1;
            }

            return d;
        }
    }
}
