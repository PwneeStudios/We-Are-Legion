using FragSharpFramework;

namespace Terracotta
{
    public partial class UpdateMagic : SimShader
    {
        [FragmentShader]
        magic FragmentShader(VertexOut vertex, Field<magic> Magic)
        {
            magic here = Magic[Here];

            here.kill = _false;

            return here;
        }
    }

    public partial class Kill : SimShader
    {
        [FragmentShader]
        magic FragmentShader(VertexOut vertex, Field<data> Select, Field<magic> Magic)
        {
            magic here  = Magic[Here];
            data select = Select[Here];

            if (Something(select))
            {
                here.kill = _true;
            }

            return here;
        }
    }
}
