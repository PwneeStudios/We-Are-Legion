using FragSharpFramework;

namespace Game
{
    public partial class UpdateFakeSelect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data)
        {
            data data_here = Data[Here];

            float state = select_state(data_here);

            if      (state == SelectState.NotSelected_Show2) state = SelectState.NotSelected_Show1;
            else if (state == SelectState.NotSelected_Show1) state = SelectState.NotSelected_NoShow;
            else if (state == SelectState.Selected_NoShow2) state = SelectState.Selected_NoShow1;
            else if (state == SelectState.Selected_NoShow1) state = SelectState.Selected_Show;

            set_select_state(ref data_here, state);

            return data_here;
        }
    }

    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<unit> Select,
            [Player.Vals] float player,
            bool deselect,
            [Vals.Bool] bool fake)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];

            if (unit_here.player != player)
            {
                return data_here;
            }

            unit select = Select[Here];

            // If the player unit here matches the specified player.
            if (select.type > 0 && (select.player == Player.None || unit_here.player == select.player) && !BlockingTileHere(unit_here))
            {
                if (fake) set_selected_fake(ref data_here, true);
                else      set_selected     (ref data_here, true);
            }
            else
            {
                if (deselect)
                {
                    if (fake) set_selected_fake(ref data_here, false);
                    else      set_selected     (ref data_here, false);
                }
            }

            return data_here;
        }
    }

    public partial class ActionSelectInBox : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit,
            vec2 bl, vec2 tr,
            [Player.Vals] float player,
            bool deselect,
            [Vals.Bool] bool fake)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];

            if (unit_here.player != player || BlockingTileHere(unit_here))
            {
                return data_here;
            }

            vec2 pos = vertex.TexCoords * Data.Size;
            bool select = bl < pos && pos < tr;

            // If the player unit here matches the specified player.
            if (select)
            {
                if (fake) set_selected_fake(ref data_here, true);
                else set_selected(ref data_here, true);
            }
            else
            {
                if (deselect)
                {
                    if (fake) set_selected_fake(ref data_here, false);
                    else set_selected(ref data_here, false);
                }
            }

            return data_here;
        }
    }

    public partial class DataDrawMouse : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, PointSampler data_texture, [Player.Vals] float player)
        {
            unit d = unit.Nothing;

            if (data_texture[Here].a > 0)
            {
                d.type = UnitType.Footman;
                d.player = player;

                // Note: Unlike other data and simulation shaders, we do need to set the alpha component for this channel.
                // The reason is that we will be drawing multiple mouse datas onto the same render target, with potential overlap.
                d.a = 1;
            }

            return (color)d;
        }
    }

    public partial class DataDrawMouseCircle : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, vec2 pos, float r2, [Player.Vals] float player, Field<data> Data)
        {
            unit d = unit.Nothing;

            vec2 pos_here = vertex.TexCoords * Data.Size;
            vec2 diff = pos_here - pos;

            float distance = diff.x * diff.x + diff.y * diff.y;
            bool in_range = distance < r2;

            if (in_range)
            {
                d.type = UnitType.Footman;
                d.player = player;

                // Note: Unlike other data and simulation shaders, we do need to set the alpha component for this channel.
                // The reason is that we will be drawing multiple mouse datas onto the same render target, with potential overlap.
                d.a = 1;
            }

            return (color)d;
        }
    }
}
