using FragSharpFramework;

namespace GpuSim
{
    public partial class ActionAttackSquare : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Current, Field<data> TargetData, vec2 Destination_BL, vec2 Destination_Size, vec2 Selection_BL, vec2 Selection_Size)
        {
            data here = Current[Here];
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
        vec4 FragmentShader(VertexOut vertex, Field<data> Current, Field<data> TargetData, vec2 Destination)
        {
            data here  = Current[Here];
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
        extra FragmentShader(VertexOut vertex, Field<data> Data, Field<extra> Extra, vec2 Destination)
        {
            data  here       = Data[Here];
            extra extra_here = Extra[Here];

            if (selected(here))
            {
                float angle = atan(vertex.TexCoords.y - Destination.y * Data.DxDy.y, vertex.TexCoords.x - Destination.x * Data.DxDy.x);
                extra_here.target_angle = (angle + 3.14159f) / (2 * 3.14159f);
            }

            return extra_here;
        }
    }

    public partial class ActionSpawn_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<data> Select)
        {
            data here = Data[Here];
            data select = Select[Here];

            if (Something(select) && !Something(here))
            {
                if ((int)(vertex.TexCoords.x * Data.Size.x) % 2 == 0 &&
                    (int)(vertex.TexCoords.y * Data.Size.y) % 2 == 0)
                {
                    here.direction = Dir.Right;
                    here.action = UnitAction.Guard;
                }
            }

            return here;
        }
    }

    public partial class ActionSpawn_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, Field<data> Select, float player, float team)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            data select = Select[Here];

            if (Something(select) && !Something(data_here))
            {
                if ((int)(vertex.TexCoords.x * Units.Size.x) % 2 == 0 &&
                    (int)(vertex.TexCoords.y * Units.Size.y) % 2 == 0)
                {
                    unit_here.player = player;
                    unit_here.team = team;
                    unit_here.type = UnitType.Footman;
                }
            }

            return unit_here;
        }
    }

    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<unit> Select, bool Deselect, float action)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            unit select = Select[Here];

            // If the player unit here matches the 
            if (select.type > 0 && unit_here.player == select.player)
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
