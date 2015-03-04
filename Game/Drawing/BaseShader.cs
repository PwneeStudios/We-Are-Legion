using FragSharpFramework;

namespace Game
{
    public partial class BaseShader : SimShader
    {
        [VertexShader]
        VertexOut SimpleVertexShader(Vertex data, vec4 cameraPos, float cameraAspect)
        {
            VertexOut Output = VertexOut.Zero;

            Output.Position.w = 1;

            Output.Position.x = (data.Position.x - cameraPos.x) / cameraAspect * cameraPos.z;
            Output.Position.y = (data.Position.y - cameraPos.y) * cameraPos.w;

            Output.TexCoords = data.TextureCoordinate;
            Output.Color = data.Color;

            return Output;
        }
    }
}
