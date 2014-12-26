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
float4 vs_param_cameraPos;
float vs_param_cameraAspect;

// The following are variables used by the fragment shader (fragment parameters).
// Texture Sampler for fs_param_Path, using register location 1
float2 fs_param_Path_size;
float2 fs_param_Path_dxdy;

Texture fs_param_Path_Texture;
sampler fs_param_Path : register(s1) = sampler_state
{
    texture   = <fs_param_Path_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};


// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
float Terracotta__SimShader__Get__Terracotta_PlayerTuple__float(float4 tuple, float player)
{
    if (abs(player - 0.003921569) < .001)
    {
        return tuple.r;
    }
    if (abs(player - 0.007843138) < .001)
    {
        return tuple.g;
    }
    if (abs(player - 0.01176471) < .001)
    {
        return tuple.b;
    }
    if (abs(player - 0.01568628) < .001)
    {
        return tuple.a;
    }
    return 0;
}

// Compiled vertex shader
VertexToPixel StandardVertexShader(float2 inPos : POSITION0, float2 inTexCoords : TEXCOORD0, float4 inColor : COLOR0)
{
    VertexToPixel Output = (VertexToPixel)0;
    Output.Position.w = 1;
    Output.Position.x = (inPos.x - vs_param_cameraPos.x) / vs_param_cameraAspect * vs_param_cameraPos.z;
    Output.Position.y = (inPos.y - vs_param_cameraPos.y) * vs_param_cameraPos.w;
    Output.TexCoords = inTexCoords;
    Output.Color = inColor;
    return Output;
}

// Compiled fragment shader
PixelToFrame FragmentShader(VertexToPixel psin)
{
    PixelToFrame __FinalOutput = (PixelToFrame)0;
    float4 output = float4(0.0, 0.0, 0.0, 0.0);
    float4 here = tex2D(fs_param_Path, psin.TexCoords + (float2(0, 0)) * fs_param_Path_dxdy);
    float dist = Terracotta__SimShader__Get__Terracotta_PlayerTuple__float(here, 0.007843138);
    bool controlled = dist < 0.0627451 - .001;
    float4 clr = controlled ? float4(0.0, 0.0, 0.0, 0.0) : float4(0.0, 0.0, 0.0, 0.65);
    clr.rgb *= clr.a;
    __FinalOutput.Color = clr;
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