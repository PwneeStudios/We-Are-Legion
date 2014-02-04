#include "RootEffect.fx"

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;

	float4 lookup = tex2D(TextureSampler,  PSIn.TexCoords);
	float4 lookup2 = tex2D(TextureSampler,  PSIn.TexCoords / 50);
	float4 lookup3 = tex2D(TextureSampler,  PSIn.TexCoords / 150);

    Output.Color = (lookup + float4(1,1,1,1)) * (lookup2 + float4(1,1,1,1)) / 8 + lookup3 / 4;
	Output.Color *= PSIn.Color;
    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}