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

    public partial class ActionSpawn : SimShader
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
                    here.direction = Dir.Right;
            }

            return here;
        }
    }

    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Select, bool Deselect, float action)
        {
            unit here = Current[Here];
            unit select = Select[Here];

            if (Something(select))
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
        color FragmentShader(VertexOut vertex, Sampler data_texture)
        {
            if (data_texture[Here].r > 0)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }
    }
}
