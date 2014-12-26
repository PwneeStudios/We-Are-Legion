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
// Texture Sampler for fs_param_Tiles, using register location 1
float2 fs_param_Tiles_size;
float2 fs_param_Tiles_dxdy;

Texture fs_param_Tiles_Texture;
sampler fs_param_Tiles : register(s1) = sampler_state
{
    texture   = <fs_param_Tiles_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Texture, using register location 2
float2 fs_param_Texture_size;
float2 fs_param_Texture_dxdy;

Texture fs_param_Texture_Texture;
sampler fs_param_Texture : register(s2) = sampler_state
{
    texture   = <fs_param_Texture_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

float fs_param_solid_blend;

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.
// Texture Sampler for fs_param_FarColor, using register location 3
float2 fs_param_FarColor_size;
float2 fs_param_FarColor_dxdy;

Texture fs_param_FarColor_Texture;
sampler fs_param_FarColor : register(s3) = sampler_state
{
    texture   = <fs_param_FarColor_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following methods are included because they are referenced by the fragment shader.
float2 Terracotta__SimShader__get_subcell_pos__FragSharpFramework_VertexOut__FragSharpFramework_vec2(VertexToPixel vertex, float2 grid_size)
{
    float2 coords = vertex.TexCoords * grid_size;
    float i = floor(coords.x);
    float j = floor(coords.y);
    return coords - float2(i, j);
}

float FragSharpFramework__FragSharpStd__Float__float(float v)
{
    return floor(255 * v + 0.5);
}

int FragSharpFramework__FragSharpStd__Int__float(float v)
{
    return (int)floor(255 * v + 0.5);
}

float4 Terracotta__DrawTiles__Sprite__Terracotta_tile__FragSharpFramework_vec2__FragSharpFramework_PointSampler__bool__float(VertexToPixel psin, float4 c, float2 pos, sampler Texture, float2 Texture_size, float2 Texture_dxdy, bool solid_blend_flag, float solid_blend)
{
    float4 clr = float4(0.0, 0.0, 0.0, 0.0);
    if (pos.x > 1 + .001 || pos.y > 1 + .001 || pos.x < 0 - .001 || pos.y < 0 - .001)
    {
        return clr;
    }
    pos = pos * 0.98 + float2(0.01, 0.01);
    pos.x += FragSharpFramework__FragSharpStd__Float__float(c.g);
    pos.y += FragSharpFramework__FragSharpStd__Float__float(c.b);
    pos *= float2(1.0 / 32, 1.0 / 32);
    clr = tex2D(Texture, pos);
    if (solid_blend_flag)
    {
        float4 solid_clr = tex2D(fs_param_FarColor, float2(FragSharpFramework__FragSharpStd__Int__float(c.r)+.5,.5+ 6 + (int)(c.r)) * fs_param_FarColor_dxdy);
        clr = solid_blend * clr + (1 - solid_blend) * solid_clr;
    }
    return clr;
}

float4 Terracotta__DrawTiles__GridLines__FragSharpFramework_vec2(float2 pos)
{
    if (pos.x > 1 + .001 || pos.y > 1 + .001 || pos.x < 0 - .001 || pos.y < 0 - .001)
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
    if (pos.x < 0.025 - .001 || pos.x > 0.975 + .001 || pos.y < 0.025 - .001 || pos.y > 0.975 + .001)
    {
        return float4(1, 1, 1, 1) * 0.2;
    }
    return float4(0.0, 0.0, 0.0, 0.0);
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
    float4 here = tex2D(fs_param_Tiles, psin.TexCoords + (float2(0, 0)) * fs_param_Tiles_dxdy);
    float2 subcell_pos = Terracotta__SimShader__get_subcell_pos__FragSharpFramework_VertexOut__FragSharpFramework_vec2(psin, fs_param_Tiles_size);
    if (here.r > 0.0 + .001)
    {
        output += Terracotta__DrawTiles__Sprite__Terracotta_tile__FragSharpFramework_vec2__FragSharpFramework_PointSampler__bool__float(psin, here, subcell_pos, fs_param_Texture, fs_param_Texture_size, fs_param_Texture_dxdy, false, fs_param_solid_blend);
        if (false)
        {
            output += Terracotta__DrawTiles__GridLines__FragSharpFramework_vec2(subcell_pos);
        }
    }
    __FinalOutput.Color = output;
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