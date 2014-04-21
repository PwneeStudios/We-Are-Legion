using FragSharpFramework;

namespace GpuSim
{
    public partial class ActionSelect : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Select, bool Deselect)
        {
            unit here = Current[Here];
            unit select = Select[Here];

            if (Something(select))
            {
                here.a = _1;
            }
            else
            {
                if (Deselect)
                    here.a = _0;
            }

            return here;
        }
    }

    public partial class DataDrawMouse : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex)
        {
            return rgba(1, 1, 1, 1);
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
