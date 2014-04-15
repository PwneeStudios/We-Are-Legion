#include "root.fx"

float4 Circle(VEC pos)
{
	float r = length(pos - VEC(.5,.5));
	if (r < .3f)
		return float4(1,1,1,1);
		//return float4(pos.r,pos.g,1,1);
	else
		return float4(0,0,0,0);
} 

float4 Sprite(UNIT data, VEC pos, float cycle_offset)
{
	if (pos.x > 1) return float4(0,0,0,0);
	if (pos.y > 1) return float4(0,0,0,0);
	if (pos.x < 0) return float4(0,0,0,0);
	if (pos.y < 0) return float4(0,0,0,0);

	pos *= SpriteSize;
	pos.x += SpriteSize.x * (((int)(PercentSimStepComplete / SpriteSize.x) + (int)(cycle_offset*255)) % 5) * data.b;
	pos.y += (data.direction * 255 - 1) * SpriteSize.y;

	//return float4(1,1,1,1);
	//return Circle(pos);
	return tex2D(drawSampler, pos);
	//return tex2D(TextureSampler, pos);
}


PROCEDURE

    UNIT cur = lookup (HERE);
	UNIT pre = lookup2(HERE);
	
	VEC subcell_pos = get_subcell_pos(HERE);

	if (cur.a == pre.a && cur.a != 0)
	{
		if (PercentSimStepComplete > .5) pre = cur;

		pre.b = 0;
		OUTPUT += Sprite(pre, subcell_pos, cur.a);
	}
	else
	{
		if (cur.direction IS_VALID_DIRECTION)
		{
			VEC vel = direction_to_vec(cur.direction);

			cur.b = 1;
			OUTPUT += Sprite(cur, subcell_pos + (1 - PercentSimStepComplete) * vel, cur.a);
		}

		if (pre.r > 0)
		{
			VEC vel = direction_to_vec(pre.direction);

			pre.b = 1;
			OUTPUT += Sprite(pre, subcell_pos - PercentSimStepComplete * vel, pre.a);
		}
	}

END