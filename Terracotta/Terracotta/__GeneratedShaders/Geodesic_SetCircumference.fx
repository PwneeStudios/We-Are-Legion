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
// Texture Sampler for fs_param_Geo, using register location 1
float2 fs_param_Geo_size;
float2 fs_param_Geo_dxdy;

Texture fs_param_Geo_Texture;
sampler fs_param_Geo : register(s1) = sampler_state
{
    texture   = <fs_param_Geo_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Info, using register location 2
float2 fs_param_Info_size;
float2 fs_param_Info_dxdy;

Texture fs_param_Info_Texture;
sampler fs_param_Info : register(s2) = sampler_state
{
    texture   = <fs_param_Info_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
float FragSharpFramework__FragSharpStd__fint_floor(float v)
{
    v += 0.0005;
    return floor(255 * v) * 0.003921569;
}

float GpuSim__SimShader__unpack_val(float2 packed)
{
    float coord = 0;
    packed = floor(255.0 * packed + float2(0.5, 0.5));
    coord = 256 * packed.x + packed.y;
    return coord;
}

float2 GpuSim__SimShader__unpack_vec2_3byte(float3 packed)
{
    float extra_bits = packed.z;
    float extra_y = FragSharpFramework__FragSharpStd__fint_floor(extra_bits / 16);
    float extra_x = FragSharpFramework__FragSharpStd__fint_floor(extra_bits - 16 * extra_y);
    float2 v = float2(0, 0);
    v.x = GpuSim__SimShader__unpack_val(float2(extra_x, packed.x));
    v.y = GpuSim__SimShader__unpack_val(float2(extra_y, packed.y));
    return v;
}

float2 GpuSim__SimShader__geo_pos_id(float4 g)
{
    return GpuSim__SimShader__unpack_vec2_3byte(g.gba);
}

float GpuSim__SimShader__polar_dist(float4 info)
{
    return GpuSim__SimShader__unpack_val(info.rg);
}

float2 GpuSim__SimShader__pack_val_2byte(float x)
{
    float2 packed = float2(0, 0);
    packed.x = floor(x / 256.0);
    packed.y = x - packed.x * 256.0;
    return packed / 255.0;
}

void GpuSim__SimShader__set_circumference(inout float4 info, float circumference)
{
    info.ba = GpuSim__SimShader__pack_val_2byte(circumference);
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
    float4 info_here = tex2D(fs_param_Info, psin.TexCoords + (float2(0, 0)) * fs_param_Info_dxdy);
    float4 here = tex2D(fs_param_Geo, psin.TexCoords + (float2(0, 0)) * fs_param_Geo_dxdy);
    if (abs(here.r - 0.0) < .001)
    {
        __FinalOutput.Color = float4(0, 0, 0, 0);
        return __FinalOutput;
    }
    float2 pos_here = psin.TexCoords * fs_param_Geo_size;
    float2 start_pos = GpuSim__SimShader__geo_pos_id(here);
    float2 GeoStart = (start_pos - pos_here);
    float4 right = tex2D(fs_param_Geo, psin.TexCoords + (GeoStart + float2(1, 0)) * fs_param_Geo_dxdy), up = tex2D(fs_param_Geo, psin.TexCoords + (GeoStart + float2(0, 1)) * fs_param_Geo_dxdy), left = tex2D(fs_param_Geo, psin.TexCoords + (GeoStart + float2(-(1), 0)) * fs_param_Geo_dxdy), down = tex2D(fs_param_Geo, psin.TexCoords + (GeoStart + float2(0, -(1))) * fs_param_Geo_dxdy);
    float circum = 0;
    if (all(abs(right.gba - here.gba) < .001))
    {
        circum = max(circum, GpuSim__SimShader__polar_dist(tex2D(fs_param_Info, psin.TexCoords + (GeoStart + float2(1, 0)) * fs_param_Info_dxdy)));
    }
    if (all(abs(up.gba - here.gba) < .001))
    {
        circum = max(circum, GpuSim__SimShader__polar_dist(tex2D(fs_param_Info, psin.TexCoords + (GeoStart + float2(0, 1)) * fs_param_Info_dxdy)));
    }
    if (all(abs(left.gba - here.gba) < .001))
    {
        circum = max(circum, GpuSim__SimShader__polar_dist(tex2D(fs_param_Info, psin.TexCoords + (GeoStart + float2(-(1), 0)) * fs_param_Info_dxdy)));
    }
    if (all(abs(down.gba - here.gba) < .001))
    {
        circum = max(circum, GpuSim__SimShader__polar_dist(tex2D(fs_param_Info, psin.TexCoords + (GeoStart + float2(0, -(1))) * fs_param_Info_dxdy)));
    }
    GpuSim__SimShader__set_circumference(info_here, circum);
    __FinalOutput.Color = info_here;
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