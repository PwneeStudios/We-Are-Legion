using FragSharpFramework;

namespace GpuSim
{
    public partial class ActionAttack : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Extra1, vec2 Destination)
        {
            unit here  = Current[Here];
            unit extra1 = Extra1[Here];

            if (selected(here))
            {
                vec2 dest = Destination;

                extra1.rg = pack_coord(dest.x);
                extra1.ba = pack_coord(dest.y);
            }
            
            return extra1;
        }
    }

    public partial class ActionAttack2 : SimShader
    {
        [FragmentShader]
        extra2 FragmentShader(VertexOut vertex, UnitField Current, Extra2Field Extra2, vec2 Destination)
        {
            unit here = Current[Here];
            extra2 extra2 = Extra2[Here];

            if (selected(here))
            {
                float angle = atan(vertex.TexCoords.y - Destination.y * Current.DxDy.y, vertex.TexCoords.x - Destination.x * Current.DxDy.x);
                extra2.a = (angle + 3.14159f) / (2 * 3.14159f);
            }

            return extra2;
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

    public partial class DrawMouse : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Sampler Texture)
        {
            color output;

            output = Texture[vertex.TexCoords];
            output *= vertex.Color;

            return output;
        }
    }
}
