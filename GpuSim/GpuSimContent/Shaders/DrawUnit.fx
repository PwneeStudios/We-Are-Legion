#include "RootEffect.fx"

float4 Circle(float2 pos)
{
	float r = length(pos - float2(.5,.5));
	if (r < .3f)
		return float4(1,1,1,1);
		//return float4(pos.r,pos.g,1,1);
	else
		return float4(0,0,0,0);
}

float4 Sprite(float4 data, float2 pos, float cycle_offset)
{
	if (pos.x > 1) return float4(0,0,0,0);
	if (pos.y > 1) return float4(0,0,0,0);
	if (pos.x < 0) return float4(0,0,0,0);
	if (pos.y < 0) return float4(0,0,0,0);

	pos *= SpriteSize;
	pos.x += SpriteSize.x * (((int)(PercentSimStepComplete / SpriteSize.x) + (int)(cycle_offset*255)) % 5) * data.b;
	pos.y += (data.r * 255 - 1) * SpriteSize.y;

	//return Circle(pos);
	return tex2D(drawSampler, pos);
}


float2 DirLookup[] = { float2(0,0), float2(1,0), float2(0,1), float2(-1,0), float2(0,-1) };
float2 _DirLookup(int dir)
{
	//return DirLookup[dir];

/*
	if (dir == 0) return float2(0,0);
	else if (dir == 1) return float2(1,0);
	else if (dir == 2) return float2(0,1);
	else if (dir == 3) return float2(-1,0);
	else               return float2(0,-1);
	*/

	float angle = (dir-1) * (3.1415926 / 2.0);
	return dir == 0 ? float2(0,0) : float2(cos(angle), sin(angle));
}

PixelToFrame SimplePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    Output.Color = float4(0,0,0,0);

    float4 cur_lookup = tex2D(TextureSampler,  PSIn.TexCoords);
	float4 pre_lookup = tex2D(TextureSampler2, PSIn.TexCoords);
	
	int cur_dir = cur_lookup.r * 255;
	int pre_dir = pre_lookup.r * 255;

	float2 texCoord = subcell_coord(PSIn);

	if (cur_lookup.a == pre_lookup.a && cur_lookup.a != 0)
	{
		if (PercentSimStepComplete > .5) pre_lookup = cur_lookup;

		pre_lookup.b = 0;
		Output.Color += Sprite(pre_lookup, texCoord, cur_lookup.a);
	}
	else
	{
		if (cur_lookup.r > 0)
		{
			float2 vel = _DirLookup(cur_dir);

			cur_lookup.b = 1;
			Output.Color += Sprite(cur_lookup, texCoord + (1 - PercentSimStepComplete) * vel, cur_lookup.a);
		}

		if (pre_lookup.r > 0)
		{
			float2 vel = _DirLookup(pre_dir);

			pre_lookup.b = 1;
			Output.Color += Sprite(pre_lookup, texCoord - PercentSimStepComplete * vel, pre_lookup.a);
		}
	}

	return Output;

	// G component
	/*
    Output.Color = lookup;
	Output.Color.r = Output.Color.a;
	return Output;
	int val = lookup.g * 255;
	if (lookup.r > 0 && lookup.a > 0)
	{
		if (val == 0) Output.Color = float4(1,0,0,1);
		else if (val == 1) Output.Color = float4(0,1,0,1);
		else if (val == 2) Output.Color = float4(1,1,1,1);
		else Output.Color = float4(1,0,1,1);
	}
    return Output;
	*/

	// Standard    
    //Output.Color = lookup;
	//Output.Color *= PSIn.Color;
    //return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER SimplestVertexShader();
        PixelShader = compile PIXEL_SHADER SimplePixelShader();
    }
}