#include "RootEffect.fx"

PixelToFrame SimplePixelShader(VertexToPixel psin)
{
    PixelToFrame Output = (PixelToFrame)0;
    
	float4 result = float4(0,0,0,0);

	// Check four directions to see if something is incoming
	float4 data1 = lookup(psin, 1, 0);
	int dir1 = data1.r * 255;
	if (dir1 == 3) result = data1;

	float4 data2 = lookup(psin, 0, 1);
	int dir2 = data2.r * 255;
	if (dir2 == 4) result = data2;

	float4 data3 = lookup(psin, -1, 0);
	int dir3 = data3.r * 255;
	if (dir3 == 1) result = data3;

	float4 data4 = lookup(psin, 0, -1);
	int dir4 = data4.r * 255;
	if (dir4 == 2) result = data4;

	// If something is here already, they have the right to stay here
	float4 data = lookup(psin, 0, 0);
	if (data.r > 0) result = data;

	// Finish
	Output.Color = result;
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