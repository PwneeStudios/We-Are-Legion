// This file was auto-generated by FragSharp. It will be regenerated on the next compilation.
// Manual changes made will not persist and may cause incorrect behavior between compilations.

#define PIXEL_SHADER ps_3_0
#define VERTEX_SHADER vs_3_0

// Vertex shader data structure definition
struct VertexToPixel
{
    float4 Position   : POSITION0;
    float4 Color      : COLOR0;
    float2 TexCoords  : TEXCOORD0;
    float2 Position2D : TEXCOORD2;
};

// Fragment shader data structure definition
struct PixelToFrame
{
    float4 Color      : COLOR0;
};

// The following are variables used by the vertex shader (vertex parameters).

// The following are variables used by the fragment shader (fragment parameters).
// Texture Sampler for fs_param_s1, using register location 1
float2 fs_param_s1_size;
float2 fs_param_s1_dxdy;

Texture fs_param_s1_Texture;
sampler fs_param_s1 : register(s1) = sampler_state
{
    texture   = <fs_param_s1_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_s2, using register location 2
float2 fs_param_s2_size;
float2 fs_param_s2_dxdy;

Texture fs_param_s2_Texture;
sampler fs_param_s2 : register(s2) = sampler_state
{
    texture   = <fs_param_s2_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_s3, using register location 3
float2 fs_param_s3_size;
float2 fs_param_s3_dxdy;

Texture fs_param_s3_Texture;
sampler fs_param_s3 : register(s3) = sampler_state
{
    texture   = <fs_param_s3_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_s4, using register location 4
float2 fs_param_s4_size;
float2 fs_param_s4_dxdy;

Texture fs_param_s4_Texture;
sampler fs_param_s4 : register(s4) = sampler_state
{
    texture   = <fs_param_s4_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.

// Compiled vertex shader
VertexToPixel StandardVertexShader(float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)
{
    VertexToPixel Output = (VertexToPixel)0;
    Output.Position.w = 1;
    Output.Position.xy = inPos.xy;
    Output.TexCoords = inTexCoords;
    return Output;
}

// Compiled fragment shader
PixelToFrame FragmentShader(VertexToPixel psin)
{
    PixelToFrame __FinalOutput = (PixelToFrame)0;
    __FinalOutput.Color = (tex2D(fs_param_s1, psin.TexCoords + (float2(0, 0)) * fs_param_s1_dxdy) + tex2D(fs_param_s1, psin.TexCoords + (float2(1, 0)) * fs_param_s1_dxdy) + tex2D(fs_param_s1, psin.TexCoords + (float2(-(1), 0)) * fs_param_s1_dxdy) + tex2D(fs_param_s1, psin.TexCoords + (float2(0, 1)) * fs_param_s1_dxdy) + tex2D(fs_param_s1, psin.TexCoords + (float2(0, -(1))) * fs_param_s1_dxdy) + tex2D(fs_param_s2, psin.TexCoords + (float2(0, 0)) * fs_param_s2_dxdy) + tex2D(fs_param_s2, psin.TexCoords + (float2(1, 0)) * fs_param_s2_dxdy) + tex2D(fs_param_s2, psin.TexCoords + (float2(-(1), 0)) * fs_param_s2_dxdy) + tex2D(fs_param_s2, psin.TexCoords + (float2(0, 1)) * fs_param_s2_dxdy) + tex2D(fs_param_s2, psin.TexCoords + (float2(0, -(1))) * fs_param_s2_dxdy) + tex2D(fs_param_s3, psin.TexCoords + (float2(0, 0)) * fs_param_s3_dxdy) + tex2D(fs_param_s3, psin.TexCoords + (float2(1, 0)) * fs_param_s3_dxdy) + tex2D(fs_param_s3, psin.TexCoords + (float2(-(1), 0)) * fs_param_s3_dxdy) + tex2D(fs_param_s3, psin.TexCoords + (float2(0, 1)) * fs_param_s3_dxdy) + tex2D(fs_param_s3, psin.TexCoords + (float2(0, -(1))) * fs_param_s3_dxdy) + tex2D(fs_param_s4, psin.TexCoords + (float2(0, 0)) * fs_param_s4_dxdy) + tex2D(fs_param_s4, psin.TexCoords + (float2(1, 0)) * fs_param_s4_dxdy) + tex2D(fs_param_s4, psin.TexCoords + (float2(-(1), 0)) * fs_param_s4_dxdy) + tex2D(fs_param_s4, psin.TexCoords + (float2(0, 1)) * fs_param_s4_dxdy) + tex2D(fs_param_s4, psin.TexCoords + (float2(0, -(1))) * fs_param_s4_dxdy)) / 16.0;
    return __FinalOutput;
}

// Shader compilation
technique Simplest
{
    pass Pass0
    {
        VertexShader = compile VERTEX_SHADER StandardVertexShader();
        PixelShader = compile PIXEL_SHADER FragmentShader();
    }
}