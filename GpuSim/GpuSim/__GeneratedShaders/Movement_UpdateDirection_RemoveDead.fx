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
// Texture Sampler for fs_param_TargetData, using register location 1
float2 fs_param_TargetData_size;
float2 fs_param_TargetData_dxdy;

Texture fs_param_TargetData_Texture;
sampler fs_param_TargetData : register(s1) = sampler_state
{
    texture   = <fs_param_TargetData_Texture>;
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

// Texture Sampler for fs_param_Extra, using register location 3
float2 fs_param_Extra_size;
float2 fs_param_Extra_dxdy;

Texture fs_param_Extra_Texture;
sampler fs_param_Extra : register(s3) = sampler_state
{
    texture   = <fs_param_Extra_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Data, using register location 4
float2 fs_param_Data_size;
float2 fs_param_Data_dxdy;

Texture fs_param_Data_Texture;
sampler fs_param_Data : register(s4) = sampler_state
{
    texture   = <fs_param_Data_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_PathToOtherTeams, using register location 5
float2 fs_param_PathToOtherTeams_size;
float2 fs_param_PathToOtherTeams_dxdy;

Texture fs_param_PathToOtherTeams_Texture;
sampler fs_param_PathToOtherTeams : register(s5) = sampler_state
{
    texture   = <fs_param_PathToOtherTeams_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following methods are included because they are referenced by the fragment shader.
bool GpuSim__SimShader__Something(float4 u)
{
    return u.r > 0 + .001;
}

bool GpuSim__SimShader__IsBuilding(float4 u)
{
    return abs(u.r - 0.007843138) < .001;
}

bool GpuSim__SimShader__IsCenter(float4 b)
{
    return abs(b.g - 0.003921569) < .001 && abs(b.a - 0.003921569) < .001;
}

bool GpuSim__SimShader__selected(float4 u)
{
    float val = u.b;
    return val >= 0.03137255 - .001;
}

void GpuSim__SimShader__set_prior_direction(inout float4 u, float dir)
{
    u.b = dir + (GpuSim__SimShader__selected(u) ? 0.03137255 : 0.0);
}

float GpuSim__SimShader__unpack_coord(float2 packed)
{
    float coord = 0;
    coord = (255 * packed.x + packed.y) * 255;
    return coord;
}

float2 GpuSim__SimShader__unpack_vec2(float4 packed)
{
    float2 v = float2(0, 0);
    v.x = GpuSim__SimShader__unpack_coord(packed.rg);
    v.y = GpuSim__SimShader__unpack_coord(packed.ba);
    return v;
}

float GpuSim__Movement_UpdateDirection_RemoveDead__BuildingDirection(VertexToPixel psin, VertexToPixel vertex, sampler TargetData, float2 TargetData_size, float2 TargetData_dxdy, float4 here)
{
    float dir = 0.003921569;
    float4 target = tex2D(TargetData, psin.TexCoords + (float2(0, 0)) * TargetData_dxdy);
    float2 CurPos = vertex.TexCoords * TargetData_size;
    float2 Destination = GpuSim__SimShader__unpack_vec2(target);
    float2 diff = Destination - CurPos;
    float2 mag = abs(diff);
    if (mag.x > mag.y + .001 && diff.x > 0 + .001)
    {
        dir = 0.003921569;
    }
    if (mag.x > mag.y + .001 && diff.x < 0 - .001)
    {
        dir = 0.01176471;
    }
    if (mag.y > mag.x + .001 && diff.y > 0 + .001)
    {
        dir = 0.007843138;
    }
    if (mag.y > mag.x + .001 && diff.y < 0 - .001)
    {
        dir = 0.01568628;
    }
    return dir;
}

bool GpuSim__SimShader__IsValid(float direction)
{
    return direction > 0 + .001;
}

void GpuSim__Movement_UpdateDirection_RemoveDead__NaivePathfind(VertexToPixel psin, VertexToPixel vertex, sampler Current, float2 Current_size, float2 Current_dxdy, sampler TargetData, float2 TargetData_size, float2 TargetData_dxdy, float4 data, inout float4 here, inout float4 extra_here)
{
    float dir = 0;
    float4 target = tex2D(TargetData, psin.TexCoords + (float2(0, 0)) * TargetData_dxdy);
    float2 CurPos = vertex.TexCoords * TargetData_size;
    float2 Destination = GpuSim__SimShader__unpack_vec2(target);
    float4 right = tex2D(Current, psin.TexCoords + (float2(1, 0)) * Current_dxdy), up = tex2D(Current, psin.TexCoords + (float2(0, 1)) * Current_dxdy), left = tex2D(Current, psin.TexCoords + (float2(-(1), 0)) * Current_dxdy), down = tex2D(Current, psin.TexCoords + (float2(0, -(1))) * Current_dxdy);
    if (Destination.x > CurPos.x + 0.75 + .001)
    {
        dir = 0.003921569;
    }
    if (Destination.x < CurPos.x - 0.75 - .001)
    {
        dir = 0.01176471;
    }
    if (Destination.y > CurPos.y + 0.75 + .001)
    {
        dir = 0.007843138;
    }
    if (Destination.y < CurPos.y - 0.75 - .001)
    {
        dir = 0.01568628;
    }
    float2 diff = Destination - CurPos;
    float2 mag = abs(diff);
    if ((mag.x > mag.y + .001 || diff.y > 0 + .001 && GpuSim__SimShader__Something(up) || diff.y < 0 - .001 && GpuSim__SimShader__Something(down)) && Destination.x > CurPos.x + 1 + .001 && !(GpuSim__SimShader__Something(right)))
    {
        dir = 0.003921569;
    }
    if ((mag.y > mag.x + .001 || diff.x > 0 + .001 && GpuSim__SimShader__Something(right) || diff.x < 0 - .001 && GpuSim__SimShader__Something(left)) && Destination.y > CurPos.y + 1 + .001 && !(GpuSim__SimShader__Something(up)))
    {
        dir = 0.007843138;
    }
    if ((mag.x > mag.y + .001 || diff.y > 0 + .001 && GpuSim__SimShader__Something(up) || diff.y < 0 - .001 && GpuSim__SimShader__Something(down)) && Destination.x < CurPos.x - 1 - .001 && !(GpuSim__SimShader__Something(left)))
    {
        dir = 0.01176471;
    }
    if ((mag.y > mag.x + .001 || diff.x > 0 + .001 && GpuSim__SimShader__Something(right) || diff.x < 0 - .001 && GpuSim__SimShader__Something(left)) && Destination.y < CurPos.y - 1 - .001 && !(GpuSim__SimShader__Something(down)))
    {
        dir = 0.01568628;
    }
    if (GpuSim__SimShader__IsValid(dir))
    {
        here.r = dir;
    }
    else
    {
        if (abs(here.a - 0.007843138) < .001)
        {
            here.a = 0.01176471;
        }
    }
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
    if (GpuSim__SimShader__Something(data_here))
    {
        float4 path = float4(0, 0, 0, 0);
        float4 here = tex2D(fs_param_Unit, psin.TexCoords + (float2(0, 0)) * fs_param_Unit_dxdy);
        float4 extra_here = tex2D(fs_param_Extra, psin.TexCoords + (float2(0, 0)) * fs_param_Extra_dxdy);
        if (abs(here.a - 0.03921569) < .001)
        {
            __FinalOutput.Color = float4(0, 0, 0, 0);
            return __FinalOutput;
        }
        if (GpuSim__SimShader__IsBuilding(here))
        {
            if (GpuSim__SimShader__IsCenter(data_here))
            {
                GpuSim__SimShader__set_prior_direction(data_here, GpuSim__Movement_UpdateDirection_RemoveDead__BuildingDirection(psin, psin, fs_param_TargetData, fs_param_TargetData_size, fs_param_TargetData_dxdy, data_here));
            }
            __FinalOutput.Color = data_here;
            return __FinalOutput;
        }
        float4 _value_right = tex2D(fs_param_PathToOtherTeams, psin.TexCoords + (float2(1, 0)) * fs_param_PathToOtherTeams_dxdy), _value_up = tex2D(fs_param_PathToOtherTeams, psin.TexCoords + (float2(0, 1)) * fs_param_PathToOtherTeams_dxdy), _value_left = tex2D(fs_param_PathToOtherTeams, psin.TexCoords + (float2(-(1), 0)) * fs_param_PathToOtherTeams_dxdy), _value_down = tex2D(fs_param_PathToOtherTeams, psin.TexCoords + (float2(0, -(1))) * fs_param_PathToOtherTeams_dxdy);
        float value_right = 1, value_left = 1, value_up = 1, value_down = 1;
        if (abs(here.b - 0.003921569) < .001)
        {
            value_right = _value_right.x;
            value_left = _value_left.x;
            value_up = _value_up.x;
            value_down = _value_down.x;
        }
        else
        {
            if (abs(here.b - 0.007843138) < .001)
            {
                value_right = _value_right.y;
                value_left = _value_left.y;
                value_up = _value_up.y;
                value_down = _value_down.y;
            }
        }
        float auto_attack_cutoff = 0.04705882;
        float min = 256;
        float hold_dir = data_here.r;
        if (abs(data_here.a - 0.007843138) < .001 || abs(data_here.a - 0.01176471) < .001)
        {
            if (value_right < min - .001)
            {
                data_here.r = 0.003921569;
                min = value_right;
            }
            if (value_up < min - .001)
            {
                data_here.r = 0.007843138;
                min = value_up;
            }
            if (value_left < min - .001)
            {
                data_here.r = 0.01176471;
                min = value_left;
            }
            if (value_down < min - .001)
            {
                data_here.r = 0.01568628;
                min = value_down;
            }
        }
        if (min > auto_attack_cutoff + .001)
        {
            data_here.r = hold_dir;
        }
        if (min < auto_attack_cutoff - .001 && abs(data_here.a - 0.01176471) < .001)
        {
            data_here.a = 0.007843138;
        }
        if (min > auto_attack_cutoff + .001 && abs(data_here.a - 0.007843138) < .001 || abs(data_here.a - 0.003921569) < .001)
        {
            GpuSim__Movement_UpdateDirection_RemoveDead__NaivePathfind(psin, psin, fs_param_Data, fs_param_Data_size, fs_param_Data_dxdy, fs_param_TargetData, fs_param_TargetData_size, fs_param_TargetData_dxdy, here, data_here, extra_here);
        }
    }
    __FinalOutput.Color = data_here;
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