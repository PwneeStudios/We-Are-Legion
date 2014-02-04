#include "RootEffect.fx"

PixelToFrame SimplePixelShader(VertexToPixel psin)
{
    PixelToFrame Output = (PixelToFrame)0;
    
	float4 result = lookup(psin, 0, 0);
	float4 prior = lookup2(psin, 0, 0);


	/*int dir = prior.r * 255;
	if (dir == 1 && lookup_intr(psin, 1, 0) == 1 ||
		dir == 2 && lookup_intr(psin, 0, 1) == 2 ||
		dir == 3 && lookup_intr(psin, -1, 0) == 3 ||
		dir == 4 && lookup_intr(psin, 0, -1) == 4 )
	{
		result = float4(0,0,0,0);
	}
	*/

	// Check four directions to see if we have the right to go somewhere
	// If we do have the right, then mark this pixel black (since we are moving out of it)
	float4 data1 = lookup(psin, 1, 0);
	if (data1.r == prior.r && data1.b == prior.b && data1.a == prior.a && prior.r > 0) result = float4(0,0,0,0);

	float4 data2 = lookup(psin, 0, 1);
	if (data2.r == prior.r && data2.b == prior.b && data2.a == prior.a && prior.r > 0) result = float4(0,0,0,0);
	
	float4 data3 = lookup(psin, -1, 0);
	if (data3.r == prior.r && data3.b == prior.b && data3.a == prior.a && prior.r > 0) result = float4(0,0,0,0);
	
	float4 data4 = lookup(psin, 0, -1);
	if (data4.r == prior.r && data4.b == prior.b && data4.a == prior.a && prior.r > 0) result = float4(0,0,0,0);


	// If unit hasn't moved, change direction
	if (result.a == prior.a && result.r > 0)
	{
		/* Turn left */
		result.r += 1/255.0;
		if (result.r > 4/255.0)
			result.r = 1/255.0;
			
		/* Reverse 
		result.r += 2/255.0;
		if (result.r > 4/255.0)
			result.r -= 4/255.0;
		*/
	}
	else
	{
		/* Return to original direction
		int val = result.g * 255;	
		if (val == 2)
			result.r = 3/255.0;
		else
		{
			if (result.r > 0 && result.a > 0)
				result.r = 1/255.0;
		}
		*/
	}
	
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