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

// Texture Sampler for fs_param_CurrentData, using register location 2
float2 fs_param_CurrentData_size;
float2 fs_param_CurrentData_dxdy;

Texture fs_param_CurrentData_Texture;
sampler fs_param_CurrentData : register(s2) = sampler_state
{
    texture   = <fs_param_CurrentData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_PreviousData, using register location 3
float2 fs_param_PreviousData_size;
float2 fs_param_PreviousData_dxdy;

Texture fs_param_PreviousData_Texture;
sampler fs_param_PreviousData : register(s3) = sampler_state
{
    texture   = <fs_param_PreviousData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Random, using register location 4
float2 fs_param_Random_size;
float2 fs_param_Random_dxdy;

Texture fs_param_Random_Texture;
sampler fs_param_Random : register(s4) = sampler_state
{
    texture   = <fs_param_Random_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.

// The following methods are included because they are referenced by the fragment shader.
bool GpuSim__SimShader__Something(float4 u)
{
    return u.r > 0 + .001;
}

bool GpuSim__SimShader__IsValid(float direction)
{
    return direction > 0 + .001;
}

float FragSharpFramework__FragSharpStd__fint_round(float v)
{
    return floor(255 * v + 0.5) * 0.003921569;
}

float GpuSim__SimShader__prior_direction(float4 u)
{
    float val = u.b;
    if (val >= 0.5019608 - .001)
    {
        val -= 0.5019608;
    }
    val = FragSharpFramework__FragSharpStd__fint_round(val);
    return val;
}

void GpuSim__SimShader__set_selected(inout float4 u, bool selected)
{
    u.b = GpuSim__SimShader__prior_direction(u) + (selected ? 0.5019608 : 0.0);
}

bool GpuSim__SimShader__selected(float4 u)
{
    float val = u.b;
    return val >= 0.5019608 - .001;
}

void GpuSim__SimShader__set_prior_direction(inout float4 u, float dir)
{
    u.b = dir + (GpuSim__SimShader__selected(u) ? 0.5019608 : 0.0);
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
    float4 cur_data = tex2D(fs_param_CurrentData, psin.TexCoords + (float2(0, 0)) * fs_param_CurrentData_dxdy), prev_data = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(0, 0)) * fs_param_PreviousData_dxdy);
    float4 rnd = tex2D(fs_param_Random, psin.TexCoords + (float2(0, 0)) * fs_param_Random_dxdy);
    if (rnd.x > 0.93 + .001 && !(GpuSim__SimShader__Something(cur_data)) && !(GpuSim__SimShader__Something(prev_data)))
    {
        float4 unit_right = tex2D(fs_param_Unit, psin.TexCoords + (float2(1, 0)) * fs_param_Unit_dxdy), unit_up = tex2D(fs_param_Unit, psin.TexCoords + (float2(0, 1)) * fs_param_Unit_dxdy), unit_left = tex2D(fs_param_Unit, psin.TexCoords + (float2(-(1), 0)) * fs_param_Unit_dxdy), unit_down = tex2D(fs_param_Unit, psin.TexCoords + (float2(0, -(1))) * fs_param_Unit_dxdy);
        float4 data_right = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(1, 0)) * fs_param_PreviousData_dxdy), data_up = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(0, 1)) * fs_param_PreviousData_dxdy), data_left = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(-(1), 0)) * fs_param_PreviousData_dxdy), data_down = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(0, -(1))) * fs_param_PreviousData_dxdy);
        float spawn_dir = 0.0;
        if (abs(unit_left.r - 0.007843138) < .001)
        {
            spawn_dir = 0.003921569;
        }
        if (abs(unit_right.r - 0.007843138) < .001)
        {
            spawn_dir = 0.01176471;
        }
        if (abs(unit_up.r - 0.007843138) < .001)
        {
            spawn_dir = 0.01568628;
        }
        if (abs(unit_down.r - 0.007843138) < .001)
        {
            spawn_dir = 0.007843138;
        }
        if (GpuSim__SimShader__IsValid(spawn_dir))
        {
            cur_data.r = spawn_dir;
            cur_data.a = 0.01568628;
            cur_data.g = 0.003921569;
            GpuSim__SimShader__set_selected(cur_data, false);
            GpuSim__SimShader__set_prior_direction(cur_data, cur_data.r);
        }
    }
    __FinalOutput.Color = cur_data;
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