using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawMouse : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, PointSampler Texture)
        {
            color output;

            output = Texture[vertex.TexCoords];
            output *= vertex.Color;

            return output;
        }
    }
}
