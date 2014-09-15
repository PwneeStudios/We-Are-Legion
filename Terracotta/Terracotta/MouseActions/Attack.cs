using FragSharpFramework;

namespace Terracotta
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
                extra_here = extra.Nothing;
            }

            return extra_here;
        }
    }
}
