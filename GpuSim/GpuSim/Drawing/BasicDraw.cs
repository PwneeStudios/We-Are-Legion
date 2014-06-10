using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawSolid : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, color clr)
        {
            return clr;
        }
    }

    public partial class DrawTexture : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, PointSampler Texture)
        {
            color output;

            output = Texture[vertex.TexCoords];
            output *= vertex.Color;
            output.rgb *= vertex.Color.a;

            return output;
        }
    }
}
