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

// Texture Sampler for fs_param_Units, using register location 3
float2 fs_param_Units_size;
float2 fs_param_Units_dxdy;

Texture fs_param_Units_Texture;
sampler fs_param_Units : register(s3) = sampler_state
{
    texture   = <fs_param_Units_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
float4 FragSharpFramework__FragSharpStd__min__FragSharpFramework_vec4__FragSharpFramework_vec4__FragSharpFramework_vec4__FragSharpFramework_vec4(float4 a, float4 b, float4 c, float4 d)
{
    return min(min(a, b), min(c, d));
}

bool Terracotta__SimShader__Something__Terracotta_data(float4 u)
{
    return u.r > 0 + .001;
}

bool Terracotta__SimShader__BlockingTileHere__Terracotta_unit(float4 u)
{
    return u.r >= 0.07843138 - .001;
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
    float4 unit_here = tex2D(fs_param_Units, psin.TexCoords + (float2(0, 0)) * fs_param_Units_dxdy);
    float4 right = tex2D(fs_param_Path, psin.TexCoords + (float2(1, 0)) * fs_param_Path_dxdy), up = tex2D(fs_param_Path, psin.TexCoords + (float2(0, 1)) * fs_param_Path_dxdy), left = tex2D(fs_param_Path, psin.TexCoords + (float2(-(1), 0)) * fs_param_Path_dxdy), down = tex2D(fs_param_Path, psin.TexCoords + (float2(0, -(1))) * fs_param_Path_dxdy);
    float4 dist_to_enemy_of = FragSharpFramework__FragSharpStd__min__FragSharpFramework_vec4__FragSharpFramework_vec4__FragSharpFramework_vec4__FragSharpFramework_vec4(right, up, left, down) + float4(0.003921569, 0.003921569, 0.003921569, 0.003921569);
    if (Terracotta__SimShader__Something__Terracotta_data(data_here))
    {
        if (Terracotta__SimShader__BlockingTileHere__Terracotta_unit(unit_here) || abs(unit_here.g - 0.0) < .001)
        {
            dist_to_enemy_of += 100 * float4(0.003921569, 0.003921569, 0.003921569, 0.003921569);
        }
        else
        {
            dist_to_enemy_of += 3 * float4(0.003921569, 0.003921569, 0.003921569, 0.003921569);
            if (abs(unit_here.b - 0.003921569) > .001)
            {
                dist_to_enemy_of.r = 0.0;
            }
            if (abs(unit_here.b - 0.007843138) > .001)
            {
                dist_to_enemy_of.g = 0.0;
            }
            if (abs(unit_here.b - 0.01176471) > .001)
            {
                dist_to_enemy_of.b = 0.0;
            }
            if (abs(unit_here.b - 0.01568628) > .001)
            {
                dist_to_enemy_of.a = 0.0;
            }
        }
    }
    __FinalOutput.Color = dist_to_enemy_of;
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