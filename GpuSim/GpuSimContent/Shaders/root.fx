#define PIXEL_SHADER ps_3_0
#define VERTEX_SHADER vs_3_0

#define PROCEDURE PixelToFrame SimplePixelShader(VertexToPixel psin) { PixelToFrame Output = (PixelToFrame)0;
#define END return Output; } technique Simplest { pass Pass0 { VertexShader = compile VERTEX_SHADER SimplestVertexShader(); PixelShader = compile PIXEL_SHADER SimplePixelShader(); } }

#define direction r

#define change g
#define MOVED  1
#define STAYED 0
#define HAS_MOVED .change == MOVED

#define UNIT float4
#define NOTHING float4(0,0,0,0)

#define VEC float2
#define VEC_ZERO float2(0,0)

#define PUT_HERE Output.Color = 
#define OUTPUT   Output.Color

#define IS_VALID_DIRECTION > 0

#define HERE      psin
#define RIGHT_ONE psin,  1,  0
#define UP_ONE    psin,  0,  1
#define LEFT_ONE  psin, -1,  0
#define DOWN_ONE  psin,  0, -1

const float RIGHT = 1 / 255.0;
const float UP    = 2 / 255.0;
const float LEFT  = 3 / 255.0;
const float DOWN  = 4 / 255.0;

const float TURN = 1 / 255.0;





const float w =		   1024.0, h =		  1024.0;
const float dx = 1.0 / 1024.0, dy = 1.0 / 1024.0;

float4 xCameraPos;
float xCameraAspect;

float PercentSimStepComplete;

float2 SpriteSize		= float2(1.0/5.0, 1.0/4.0);

Texture xTexture;
sampler TextureSampler : register(s1) = sampler_state
{
	texture = <xTexture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture xTexture2;
sampler TextureSampler2 : register(s2) = sampler_state
{
	texture = <xTexture2>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture drawTexture;
sampler drawSampler : register(s3) = sampler_state
{
	texture = <drawTexture>;
    MipFilter = Linear;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexToPixel
{
	float4 Position     : POSITION0;
    float4 Color		: COLOR0;
    float2 TexCoords    : TEXCOORD0;
    float2 Position2D   : TEXCOORD2;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

float4 lookup(VertexToPixel psin)
{
	return tex2D(TextureSampler, psin.TexCoords);
}

float4 lookup(VertexToPixel psin, int i, int j)
{
	return tex2D(TextureSampler, psin.TexCoords + float2((i+.5) * dx, (j+.5) * dy));
}

float4 lookup2(VertexToPixel psin)
{
	return tex2D(TextureSampler2, psin.TexCoords);
}

UNIT lookup2(VertexToPixel psin, int i, int j)
{
	return tex2D(TextureSampler2, psin.TexCoords + float2((i+.5) * dx, (j+.5) * dy));
}

VEC get_subcell_pos(VertexToPixel psin)
{
	float2 coords = psin.TexCoords.xy * VEC(w, h);
	float i = floor(coords.x);
	float j = floor(coords.y);

	return coords - VEC(i, j);
}

VEC direction_to_vec(float direction)
{
	float angle = (direction * 255 - 1) * (3.1415926 / 2.0);
	return direction IS_VALID_DIRECTION ? float2(cos(angle), sin(angle)) : VEC_ZERO;
}

UNIT lookup(VertexToPixel psin, float direction)
{
	float angle = (direction * 255 - 1) * (3.1415926 / 2.0);

	return lookup(HERE, direction IS_VALID_DIRECTION ? cos(angle) : VEC_ZERO, direction IS_VALID_DIRECTION ? sin(angle) : VEC_ZERO);
}

VertexToPixel SimplestVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)
{
    VertexToPixel Output = (VertexToPixel)0;    

    Output.Position.w = 1;
    
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;

	Output.Position2D = Output.Position.xy;

    Output.TexCoords = inTexCoords;
    Output.Color = inColor;
    
    return Output;
}