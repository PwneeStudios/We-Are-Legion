using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawGrass : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, PointSampler Texture)
        {
            color output;

            color lookup1 = Texture[vertex.TexCoords];
            color lookup2 = Texture[vertex.TexCoords / 50];
            color lookup3 = Texture[vertex.TexCoords / 150];

            output = (lookup1 + rgba(1,1,1,1)) * (lookup2 + rgba(1, 1, 1, 1)) / 8 + lookup3 / 4;
            output *= vertex.Color;
            return output;
        }
    }
}
