#include "root.fx"

float GoL_calc(float val, float current)
{
	if (abs(val - 3) < .05)
		return 1;
	else if (abs(val - 2) < .05)
		return current;
	else
		return 0;
}

PixelToFrame SimplePixelShader(VertexToPixel psin)
{
    PixelToFrame Output = (PixelToFrame)0;
    
	// Translate
	//float4 val = lookup(psin, 1, 1);
	//Output.Color = val;


	/*
	// Game of Life
	float val = 
		lookup(psin,  1,  1).r +
		lookup(psin,  1,  0).r +
		lookup(psin,  1, -1).r +
		lookup(psin, -1,  1).r +
		lookup(psin, -1,  0).r +
		lookup(psin, -1, -1).r +
		lookup(psin,  0,  1).r +
		lookup(psin,  0, -1).r;

	if (abs(val - 3) < .05)
		Output.Color = float4(1,1,1,1);
	else if (abs(val - 2) < .05)
		Output.Color = lookup(psin, 0, 0);
	else
		Output.Color = float4(0,0,0,0);
	*/

	// 3 x Game of Life
	float4 lkup1 = lookup(psin,  1,  1);
	float4 lkup2 = lookup(psin,  1,  0);
	float4 lkup3 = lookup(psin,  1, -1);
	float4 lkup4 = lookup(psin, -1,  1);
	float4 lkup5 = lookup(psin, -1,  0);
	float4 lkup6 = lookup(psin, -1, -1);
	float4 lkup7 = lookup(psin,  0,  1);
	float4 lkup8 = lookup(psin,  0, -1);
	float4 lkup9 = lookup(psin,  2, 0);

	float4 lkup = lookup(psin, 0, 0);

	float valr = lkup1.r + lkup2.r + lkup3.r + lkup4.r + lkup5.r + lkup6.r + lkup7.r + lkup8.b + lkup9.r;
	float valg = lkup1.g + lkup2.g + lkup3.g + lkup4.g + lkup5.g + lkup6.g + lkup7.g + lkup8.r;// + lkup9.g;
	float valb = lkup1.b + lkup2.b + lkup3.b + lkup4.b + lkup5.b + lkup6.b + lkup7.b + lkup8.g;// + lkup9.b;

	Output.Color.r = GoL_calc(valr, lkup.r);
	Output.Color.g = GoL_calc(valg, lkup.g);
	Output.Color.b = GoL_calc(valb, lkup.b);

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