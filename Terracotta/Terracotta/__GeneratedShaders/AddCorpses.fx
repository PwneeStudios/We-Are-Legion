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
// Texture Sampler for fs_param_Unit, using register location 1
float2 fs_param_Unit_size;
float2 fs_param_Unit_dxdy;

Texture fs_param_Unit_Texture;
sampler fs_param_Unit : register(s1) = sampler_state
{
    texture   = <fs_param_Unit_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Data, using register location 2
float2 fs_param_Data_size;
float2 fs_param_Data_dxdy;

Texture fs_param_Data_Texture;
sampler fs_param_Data : register(s2) = sampler_state
{
    texture   = <fs_param_Data_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Corpses, using register location 3
float2 fs_param_Corpses_size;
float2 fs_param_Corpses_dxdy;

Texture fs_param_Corpses_Texture;
sampler fs_param_Corpses : register(s3) = sampler_state
{
    texture   = <fs_param_Corpses_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Magic, using register location 4
float2 fs_param_Magic_size;
float2 fs_param_Magic_dxdy;

Texture fs_param_Magic_Texture;
sampler fs_param_Magic : register(s4) = sampler_state
{
    texture   = <fs_param_Magic_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
bool Terracotta__SimShader__Something__Terracotta_data(float4 u)
{
    return u.r > 0 + .001;
}

bool Terracotta__SimShader__IsUnit__Terracotta_unit(float4 u)
{
    return u.r >= 0.003921569 - .001 && u.r < 0.02352941 - .001;
}

bool Terracotta__SimShader__LeavesCorpse__Terracotta_unit(float4 u)
{
    return Terracotta__SimShader__IsUnit__Terracotta_unit(u) && abs(u.r - 0.01568628) > .001;
}

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
    float4 unit_here = tex2D(fs_param_Unit, psin.TexCoords + (float2(0, 0)) * fs_param_Unit_dxdy);
    float4 data_here = tex2D(fs_param_Data, psin.TexCoords + (float2(0, 0)) * fs_param_Data_dxdy);
    float4 corpse_here = tex2D(fs_param_Corpses, psin.TexCoords + (float2(0, 0)) * fs_param_Corpses_dxdy);
    float4 magic_here = tex2D(fs_param_Magic, psin.TexCoords + (float2(0, 0)) * fs_param_Magic_dxdy);
    if (abs(magic_here.g - 0.0) > .001 && abs(unit_here.a - 0.2352941) < .001)
    {
        corpse_here = float4(0, 0, 0, 0);
    }
    if (Terracotta__SimShader__Something__Terracotta_data(data_here) && abs(unit_here.a - 0.07058824) < .001 && Terracotta__SimShader__LeavesCorpse__Terracotta_unit(unit_here))
    {
        corpse_here.r = data_here.r;
        corpse_here.g = unit_here.r;
        corpse_here.b = unit_here.g;
    }
    __FinalOutput.Color = corpse_here;
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