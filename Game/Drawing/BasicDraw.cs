using FragSharpFramework;

namespace Game
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

    public partial class DrawColoredTexture : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, TextureSampler<Clamp, Point> Texture, color clr)
        {
            color output;

            output = Texture[vertex.TexCoords];
            output *= vertex.Color * clr;
            output.rgb *= vertex.Color.a;

            return output;
        }
    }

    public partial class DrawTextureSmooth : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, TextureSampler<Clamp, Linear> Texture)
        {
            color output;

            output = Texture[vertex.TexCoords];
            output *= vertex.Color;
            output.rgb *= vertex.Color.a;

            return output;
        }
    }
}
