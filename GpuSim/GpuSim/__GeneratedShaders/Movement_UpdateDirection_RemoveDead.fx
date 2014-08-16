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

// Texture Sampler for fs_param_PrevData, using register location 5
float2 fs_param_PrevData_size;
float2 fs_param_PrevData_dxdy;

Texture fs_param_PrevData_Texture;
sampler fs_param_PrevData : register(s5) = sampler_state
{
    texture   = <fs_param_PrevData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_PathToOtherTeams, using register location 6
float2 fs_param_PathToOtherTeams_size;
float2 fs_param_PathToOtherTeams_dxdy;

Texture fs_param_PathToOtherTeams_Texture;
sampler fs_param_PathToOtherTeams : register(s6) = sampler_state
{
    texture   = <fs_param_PathToOtherTeams_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Geo, using register location 7
float2 fs_param_Geo_size;
float2 fs_param_Geo_dxdy;

Texture fs_param_Geo_Texture;
sampler fs_param_Geo : register(s7) = sampler_state
{
    texture   = <fs_param_Geo_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_DirwardRight, using register location 8
float2 fs_param_DirwardRight_size;
float2 fs_param_DirwardRight_dxdy;

Texture fs_param_DirwardRight_Texture;
sampler fs_param_DirwardRight : register(s8) = sampler_state
{
    texture   = <fs_param_DirwardRight_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_DirwardLeft, using register location 9
float2 fs_param_DirwardLeft_size;
float2 fs_param_DirwardLeft_dxdy;

Texture fs_param_DirwardLeft_Texture;
sampler fs_param_DirwardLeft : register(s9) = sampler_state
{
    texture   = <fs_param_DirwardLeft_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_DirwardUp, using register location 10
float2 fs_param_DirwardUp_size;
float2 fs_param_DirwardUp_dxdy;

Texture fs_param_DirwardUp_Texture;
sampler fs_param_DirwardUp : register(s10) = sampler_state
{
    texture   = <fs_param_DirwardUp_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_DirwardDown, using register location 11
float2 fs_param_DirwardDown_size;
float2 fs_param_DirwardDown_dxdy;

Texture fs_param_DirwardDown_Texture;
sampler fs_param_DirwardDown : register(s11) = sampler_state
{
    texture   = <fs_param_DirwardDown_Texture>;
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

bool GpuSim__SimShader__IsUnit(float4 u)
{
    return abs(u.r - 0.003921569) < .001;
}

bool GpuSim__SimShader__IsBuilding(float4 u)
{
    return u.r >= 0.007843138 - .001 && u.r < 0.01960784 - .001;
}

float GpuSim__ExplosionSpriteSheet__ExplosionFrame(float s, float4 building_here)
{
    return (s + 255 * (building_here.r - 0.02745098)) * 6;
}

bool GpuSim__SimShader__IsCenter(float4 b)
{
    return abs(b.g - 0.003921569) < .001 && abs(b.a - 0.003921569) < .001;
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

float GpuSim__SimShader__unpack_val(float2 packed)
{
    float coord = 0;
    packed = floor(255.0 * packed + float2(0.5, 0.5));
    coord = 256 * packed.x + packed.y;
    return coord;
}

float2 GpuSim__SimShader__unpack_vec2(float4 packed)
{
    float2 v = float2(0, 0);
    v.x = GpuSim__SimShader__unpack_val(packed.rg);
    v.y = GpuSim__SimShader__unpack_val(packed.ba);
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

float GpuSim__SimShader__wall_pos(float4 d)
{
    return GpuSim__SimShader__unpack_val(d.ba);
}

bool GpuSim__SimShader__ValidDirward(float4 d)
{
    return any(abs(d - float4(0, 0, 0, 0)) > .001);
}

bool GpuSim__SimShader__IsValid(float direction)
{
    return direction > 0 + .001;
}

void GpuSim__Movement_UpdateDirection_RemoveDead__NaivePathfind(VertexToPixel psin, VertexToPixel vertex, sampler Current, float2 Current_size, float2 Current_dxdy, sampler Previous, float2 Previous_size, float2 Previous_dxdy, sampler TargetData, float2 TargetData_size, float2 TargetData_dxdy, sampler Geo, float2 Geo_size, float2 Geo_dxdy, sampler DirwardRight, float2 DirwardRight_size, float2 DirwardRight_dxdy, sampler DirwardLeft, float2 DirwardLeft_size, float2 DirwardLeft_dxdy, sampler DirwardUp, float2 DirwardUp_size, float2 DirwardUp_dxdy, sampler DirwardDown, float2 DirwardDown_size, float2 DirwardDown_dxdy, float4 data, inout float4 here, inout float4 extra_here)
{
    float dir = 0;
    float4 target = tex2D(TargetData, psin.TexCoords + (float2(0, 0)) * TargetData_dxdy);
    float2 CurPos = floor((vertex.TexCoords * TargetData_size + float2(0.5, 0.5)));
    float2 Destination = floor(GpuSim__SimShader__unpack_vec2(target));
    float4 right = tex2D(Current, psin.TexCoords + (float2(1, 0)) * Current_dxdy), up = tex2D(Current, psin.TexCoords + (float2(0, 1)) * Current_dxdy), left = tex2D(Current, psin.TexCoords + (float2(-(1), 0)) * Current_dxdy), down = tex2D(Current, psin.TexCoords + (float2(0, -(1))) * Current_dxdy);
    float4 prev_right = tex2D(Previous, psin.TexCoords + (float2(1, 0)) * Previous_dxdy), prev_up = tex2D(Previous, psin.TexCoords + (float2(0, 1)) * Previous_dxdy), prev_left = tex2D(Previous, psin.TexCoords + (float2(-(1), 0)) * Previous_dxdy), prev_down = tex2D(Previous, psin.TexCoords + (float2(0, -(1))) * Previous_dxdy);
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
    float dir2 = 0.0;
    bool blocked = false;
    if (mag.x > mag.y + .001 && Destination.x > CurPos.x + 1 + .001)
    {
        dir = 0.003921569;
        blocked = GpuSim__SimShader__Something(right) || GpuSim__SimShader__Something(prev_right);
    }
    if (mag.y > mag.x + .001 && Destination.y > CurPos.y + 1 + .001)
    {
        dir = 0.007843138;
        blocked = GpuSim__SimShader__Something(up) || GpuSim__SimShader__Something(prev_up);
    }
    if (mag.x > mag.y + .001 && Destination.x < CurPos.x - 1 - .001)
    {
        dir = 0.01176471;
        blocked = GpuSim__SimShader__Something(left) || GpuSim__SimShader__Something(prev_left);
    }
    if (mag.y > mag.x + .001 && Destination.y < CurPos.y - 1 - .001)
    {
        dir = 0.01568628;
        blocked = GpuSim__SimShader__Something(down) || GpuSim__SimShader__Something(prev_down);
    }
    bool blocked2 = false;
    if (abs(dir - 0.003921569) < .001 || abs(dir - 0.01176471) < .001)
    {
        if (Destination.y > CurPos.y + 0 + .001)
        {
            dir2 = 0.007843138;
            blocked2 = GpuSim__SimShader__Something(up) || GpuSim__SimShader__Something(prev_up);
        }
        else
        {
            if (Destination.y < CurPos.y - 0 - .001)
            {
                dir2 = 0.01568628;
                blocked2 = GpuSim__SimShader__Something(down) || GpuSim__SimShader__Something(prev_down);
            }
        }
    }
    if (abs(dir - 0.007843138) < .001 || abs(dir - 0.01568628) < .001)
    {
        if (Destination.x > CurPos.x + 0 + .001)
        {
            dir2 = 0.003921569;
            blocked2 = GpuSim__SimShader__Something(right) || GpuSim__SimShader__Something(prev_right);
        }
        else
        {
            if (Destination.x < CurPos.x - 0 - .001)
            {
                dir2 = 0.01176471;
                blocked2 = GpuSim__SimShader__Something(left) || GpuSim__SimShader__Something(prev_left);
            }
        }
    }
    float4 geo_here = tex2D(Geo, psin.TexCoords + (float2(0, 0)) * Geo_dxdy);
    float4 dirward_here = float4(0, 0, 0, 0);
    bool other_side = false;
    if (abs(dir - 0.003921569) < .001)
    {
        dirward_here = tex2D(DirwardRight, psin.TexCoords + (float2(0, 0)) * DirwardRight_dxdy);
        other_side = Destination.x > GpuSim__SimShader__wall_pos(dirward_here) + .001;
    }
    else
    {
        if (abs(dir - 0.01176471) < .001)
        {
            dirward_here = tex2D(DirwardLeft, psin.TexCoords + (float2(0, 0)) * DirwardLeft_dxdy);
            other_side = Destination.x < GpuSim__SimShader__wall_pos(dirward_here) - .001;
        }
        else
        {
            if (abs(dir - 0.007843138) < .001)
            {
                dirward_here = tex2D(DirwardUp, psin.TexCoords + (float2(0, 0)) * DirwardUp_dxdy);
                other_side = Destination.y > GpuSim__SimShader__wall_pos(dirward_here) + .001;
            }
            else
            {
                if (abs(dir - 0.01568628) < .001)
                {
                    dirward_here = tex2D(DirwardDown, psin.TexCoords + (float2(0, 0)) * DirwardDown_dxdy);
                    other_side = Destination.y < GpuSim__SimShader__wall_pos(dirward_here) - .001;
                }
            }
        }
    }
    float4 dirward_here2 = float4(0, 0, 0, 0);
    bool other_side2 = false;
    if (abs(dir2 - 0.003921569) < .001)
    {
        dirward_here2 = tex2D(DirwardRight, psin.TexCoords + (float2(0, 0)) * DirwardRight_dxdy);
        other_side2 = Destination.x > GpuSim__SimShader__wall_pos(dirward_here2) + .001;
    }
    else
    {
        if (abs(dir2 - 0.01176471) < .001)
        {
            dirward_here2 = tex2D(DirwardLeft, psin.TexCoords + (float2(0, 0)) * DirwardLeft_dxdy);
            other_side2 = Destination.x < GpuSim__SimShader__wall_pos(dirward_here2) - .001;
        }
        else
        {
            if (abs(dir2 - 0.007843138) < .001)
            {
                dirward_here2 = tex2D(DirwardUp, psin.TexCoords + (float2(0, 0)) * DirwardUp_dxdy);
                other_side2 = Destination.y > GpuSim__SimShader__wall_pos(dirward_here2) + .001;
            }
            else
            {
                if (abs(dir2 - 0.01568628) < .001)
                {
                    dirward_here2 = tex2D(DirwardDown, psin.TexCoords + (float2(0, 0)) * DirwardDown_dxdy);
                    other_side2 = Destination.y < GpuSim__SimShader__wall_pos(dirward_here2) - .001;
                }
            }
        }
    }
    float2 geo_id = geo_here.ba;
    if (geo_here.r > 0 + .001 && (abs(geo_here.g - 0.0) < .001 || blocked && other_side || blocked2 && other_side2) && (GpuSim__SimShader__ValidDirward(dirward_here) && other_side && all(abs(dirward_here.rg - geo_id) < .001) || GpuSim__SimShader__ValidDirward(dirward_here2) && other_side2 && all(abs(dirward_here2.rg - geo_id) < .001)))
    {
        dir = geo_here.r;
    }
    else
    {
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
        if (abs(here.a - 0.03921569) < .001 && GpuSim__SimShader__IsUnit(here))
        {
            __FinalOutput.Color = float4(0, 0, 0, 0);
            return __FinalOutput;
        }
        float4 b = data_here;
        if (GpuSim__SimShader__IsBuilding(here))
        {
            if (abs(data_here.r - 0.01960784) < .001)
            {
                if (here.a >= 0.01960784 - .001)
                {
                    data_here.r = 0.02352941;
                }
            }
            else
            {
                float frame = GpuSim__ExplosionSpriteSheet__ExplosionFrame(0, b);
                if (frame >= 16 - .001)
                {
                    __FinalOutput.Color = float4(0, 0, 0, 0);
                    return __FinalOutput;
                }
            }
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
            else
            {
                if (abs(here.b - 0.01176471) < .001)
                {
                    value_right = _value_right.z;
                    value_left = _value_left.z;
                    value_up = _value_up.z;
                    value_down = _value_down.z;
                }
                else
                {
                    if (abs(here.b - 0.01568628) < .001)
                    {
                        value_right = _value_right.w;
                        value_left = _value_left.w;
                        value_up = _value_up.w;
                        value_down = _value_down.w;
                    }
                }
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
            GpuSim__Movement_UpdateDirection_RemoveDead__NaivePathfind(psin, psin, fs_param_Data, fs_param_Data_size, fs_param_Data_dxdy, fs_param_PrevData, fs_param_PrevData_size, fs_param_PrevData_dxdy, fs_param_TargetData, fs_param_TargetData_size, fs_param_TargetData_dxdy, fs_param_Geo, fs_param_Geo_size, fs_param_Geo_dxdy, fs_param_DirwardRight, fs_param_DirwardRight_size, fs_param_DirwardRight_dxdy, fs_param_DirwardLeft, fs_param_DirwardLeft_size, fs_param_DirwardLeft_dxdy, fs_param_DirwardUp, fs_param_DirwardUp_size, fs_param_DirwardUp_dxdy, fs_param_DirwardDown, fs_param_DirwardDown_size, fs_param_DirwardDown_dxdy, here, data_here, extra_here);
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