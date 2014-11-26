using FragSharpFramework;

namespace Terracotta
{
    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<unit> Select,
            [Player.Vals] float player,
            bool deselect)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];

            if (unit_here.player != player)
            {
                return data_here;
            }

            unit select = Select[Here];

            // If the player unit here matches the 
            if (select.type > 0 && (select.player == Player.None || unit_here.player == select.player) && !BlockingTileHere(unit_here))
            {
                set_selected(ref data_here, true);
            }
            else
            {
                if (deselect)
                    set_selected(ref data_here, false);
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
