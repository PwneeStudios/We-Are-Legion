#include "root.fx"

PixelToFrame SimplePixelShader(VertexToPixel psin)
{
    PixelToFrame Output = (PixelToFrame)0;

	float4 lookup = tex2D(TextureSampler,  psin.TexCoords);
	float4 lookup2 = tex2D(TextureSampler,  psin.TexCoords / 50);
	float4 lookup3 = tex2D(TextureSampler,  psin.TexCoords / 150);

    Output.Color = (lookup + float4(1,1,1,1)) * (lookup2 + float4(1,1,1,1)) / 8 + lookup3 / 4;
	Output.Color *= psin.Color;
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