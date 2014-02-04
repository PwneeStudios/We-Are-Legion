#define PIXEL_SHADER ps_3_0
#define VERTEX_SHADER vs_3_0

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

float4 lookup(VertexToPixel PSIn, int i, int j)
{
	return tex2D(TextureSampler, PSIn.TexCoords + float2((i+.5) * dx, (j+.5) * dy));
}

float4 lookup2(VertexToPixel PSIn, int i, int j)
{
	return tex2D(TextureSampler2, PSIn.TexCoords + float2((i+.5) * dx, (j+.5) * dy));
}

int lookup_intr(VertexToPixel PSIn, int i, int j)
{
	return (int)(tex2D(TextureSampler, PSIn.TexCoords + float2((i+.5) * dx, (j+.5) * dy)) * 255);
}

int lookup_intr_2(VertexToPixel PSIn, int i, int j)
{
	return (int)(tex2D(TextureSampler2, PSIn.TexCoords + float2((i+.5) * dx, (j+.5) * dy)) * 255);
}

float2 subcell_coord(VertexToPixel PSIn)
{
	float2 coords = PSIn.TexCoords.xy * float2(w, h);
	float i = floor(coords.x);
	float j = floor(coords.y);

	return coords - float2(i, j);
}

VertexToPixel SimplestVertexShader( float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)//, float inDepth : POSITION1)
{
    VertexToPixel Output = (VertexToPixel)0;    

    Output.Position.xy = inPos;
    Output.Position.w = 1;
    
    Output.Position.x = (inPos.x - xCameraPos.x) / xCameraAspect * xCameraPos.z;
    Output.Position.y = (inPos.y - xCameraPos.y) * xCameraPos.w;

	Output.Position2D = Output.Position.xy;

    Output.TexCoords = inTexCoords;
    Output.Color = inColor;
    
    return Output;
}