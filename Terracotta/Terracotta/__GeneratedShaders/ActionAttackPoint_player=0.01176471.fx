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
// Texture Sampler for fs_param_Data, using register location 1
float2 fs_param_Data_size;
float2 fs_param_Data_dxdy;

Texture fs_param_Data_Texture;
sampler fs_param_Data : register(s1) = sampler_state
{
    texture   = <fs_param_Data_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Unit, using register location 2
float2 fs_param_Unit_size;
float2 fs_param_Unit_dxdy;

Texture fs_param_Unit_Texture;
sampler fs_param_Unit : register(s2) = sampler_state
{
    texture   = <fs_param_Unit_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_TargetData, using register location 3
float2 fs_param_TargetData_size;
float2 fs_param_TargetData_dxdy;

Texture fs_param_TargetData_Texture;
sampler fs_param_TargetData : register(s3) = sampler_state
{
    texture   = <fs_param_TargetData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

float2 fs_param_Destination;


// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
bool Terracotta__SimShader__selected(float4 u)
{
    float val = u.b;
    return val >= 0.5019608 - .001;
}

float2 Terracotta__SimShader__pack_val_2byte(float x)
{
    float2 packed = float2(0, 0);
    packed.x = floor(x / 256.0);
    packed.y = x - packed.x * 256.0;
    return packed / 255.0;
}

float4 Terracotta__SimShader__pack_vec2(float2 v)
{
    float2 packed_x = Terracotta__SimShader__pack_val_2byte(v.x);
    float2 packed_y = Terracotta__SimShader__pack_val_2byte(v.y);
    return float4(packed_x.x, packed_x.y, packed_y.x, packed_y.y);
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
    float4 data_here = tex2D(fs_param_Data, psin.TexCoords + (float2(0, 0)) * fs_param_Data_dxdy);
    float4 unit_here = tex2D(fs_param_Unit, psin.TexCoords + (float2(0, 0)) * fs_param_Unit_dxdy);
    float4 target = float4(0, 0, 0, 0);
    if (abs(0.01176471 - unit_here.g) < .001 && Terracotta__SimShader__selected(data_here))
    {
        float2 dest = fs_param_Destination;
        target = Terracotta__SimShader__pack_vec2(dest);
    }
    else
    {
        target = tex2D(fs_param_TargetData, psin.TexCoords + (float2(0, 0)) * fs_param_TargetData_dxdy);
    }
    __FinalOutput.Color = target;
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