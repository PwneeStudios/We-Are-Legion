using FragSharpFramework;

namespace GpuSim
{
    public partial class ActionAttackSquare : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, UnitField Current, UnitField TargetData, vec2 Destination_BL, vec2 Destination_Size, vec2 Selection_BL, vec2 Selection_Size)
        {
            unit here = Current[Here];
            vec4 target = vec4.Zero;

            if (selected(here))
            {
                vec2 pos = vertex.TexCoords * Current.Size;

                pos = (pos - Selection_BL) / Selection_Size;
                pos = pos * Destination_Size + Destination_BL;

                target = pack_vec2(pos);
            }
            else
            {
                target = (vec4)TargetData[Here];
            }

            return target;
        }
    }

    public partial class ActionAttackPoint : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, UnitField Current, UnitField TargetData, vec2 Destination)
        {
            unit here  = Current[Here];
            vec4 target = vec4.Zero;

            if (selected(here))
            {
                vec2 dest = Destination;

                target = pack_vec2(dest);
            }
            else
            {
                target = (vec4)TargetData[Here];
            }
            
            return target;
        }
    }

    public partial class ActionAttack2 : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, UnitField Current, DataField Data, vec2 Destination)
        {
            unit here = Current[Here];
            data data = Data[Here];

            if (selected(here))
            {
                float angle = atan(vertex.TexCoords.y - Destination.y * Current.DxDy.y, vertex.TexCoords.x - Destination.x * Current.DxDy.x);
                data.a = (angle + 3.14159f) / (2 * 3.14159f);
            }

            return data;
        }
    }

    public partial class ActionSpawn_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Select)
        {
            unit here = Current[Here];
            unit select = Select[Here];

            if (Something(select))
            {
                if ((int)(vertex.TexCoords.x * Current.Size.x) % 2 == 0 &&
                    (int)(vertex.TexCoords.y * Current.Size.y) % 2 == 0)
                {
                    here.direction = Dir.Right;
                }
            }

            return here;
        }
    }

    public partial class ActionSpawn_Extra : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, DataField CurData, UnitField Select, float player, float team)
        {
            data data = CurData[Here];
            unit select = Select[Here];

            if (Something(select))
            {
                if ((int)(vertex.TexCoords.x * CurData.Size.x) % 2 == 0 &&
                    (int)(vertex.TexCoords.y * CurData.Size.y) % 2 == 0)
                {
                    data.player = player;
                    data.team = team;
                }
            }

            return data;
        }
    }

    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, DataField CurData, DataField Select, bool Deselect, float action)
        {
            unit here = Current[Here];
            data data_here = CurData[Here];
            data select = Select[Here];

            // If the player unit here matches the 
            if (select.type > 0 && data_here.player == select.player)
            //if (select.type > 0 && data_here.player == Player.One)
            //if (select.type > 0 && data_here.player > _1 * .9  && select.player > _1 * .9
            //                    && data_here.player < _1 * 1.1 && select.player < _1 * 1.1)
            {
                set_selected(ref here, true);
            }
            else
            {
                if (Deselect)
                    set_selected(ref here, false);
            }

            if (Something(here) && selected(here) && action < UnitAction.NoChange)
            {
                here.action = action;
            }

            return here;
        }
    }

    public partial class DataDrawMouse : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Sampler data_texture, float player)
        {
            data d = data.Nothing;

            if (data_texture[Here].a > 0)
            {
                d.type = _1;
                d.player = player;
                
                // Note: Unlike other data and simulation shaders, we do need to set the alpha component for this channel.
                // The reason is that we will be drawing multiple mouse datas onto the same render target, with potential overlapping.
                d.a = 1;
            }

            return d;
        }
    }
}
