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
// Texture Sampler for fs_param_CurrentData, using register location 1
float2 fs_param_CurrentData_size;
float2 fs_param_CurrentData_dxdy;

Texture fs_param_CurrentData_Texture;
sampler fs_param_CurrentData : register(s1) = sampler_state
{
    texture   = <fs_param_CurrentData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_PreviousData, using register location 2
float2 fs_param_PreviousData_size;
float2 fs_param_PreviousData_dxdy;

Texture fs_param_PreviousData_Texture;
sampler fs_param_PreviousData : register(s2) = sampler_state
{
    texture   = <fs_param_PreviousData_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_CurrentUnits, using register location 3
float2 fs_param_CurrentUnits_size;
float2 fs_param_CurrentUnits_dxdy;

Texture fs_param_CurrentUnits_Texture;
sampler fs_param_CurrentUnits : register(s3) = sampler_state
{
    texture   = <fs_param_CurrentUnits_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_PreviousUnits, using register location 4
float2 fs_param_PreviousUnits_size;
float2 fs_param_PreviousUnits_dxdy;

Texture fs_param_PreviousUnits_Texture;
sampler fs_param_PreviousUnits : register(s4) = sampler_state
{
    texture   = <fs_param_PreviousUnits_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Texture, using register location 5
float2 fs_param_Texture_size;
float2 fs_param_Texture_dxdy;

Texture fs_param_Texture_Texture;
sampler fs_param_Texture : register(s5) = sampler_state
{
    texture   = <fs_param_Texture_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Wrap;
    AddressV  = Wrap;
};

float fs_param_s;

float fs_param_t;

float fs_param_selection_blend;

float fs_param_selection_size;

float fs_param_solid_blend;

// The following variables are included because they are referenced but are not function parameters. Their values will be set at call time.
// Texture Sampler for fs_param_FarColor, using register location 6
float2 fs_param_FarColor_size;
float2 fs_param_FarColor_dxdy;

Texture fs_param_FarColor_Texture;
sampler fs_param_FarColor : register(s6) = sampler_state
{
    texture   = <fs_param_FarColor_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// The following methods are included because they are referenced by the fragment shader.
bool GpuSim__SimShader__IsUnit(float4 u)
{
    return abs(u.r - 0.003921569) < .001;
}

float2 GpuSim__SimShader__get_subcell_pos(VertexToPixel vertex, float2 grid_size)
{
    float2 coords = vertex.TexCoords * grid_size;
    float i = floor(coords.x);
    float j = floor(coords.y);
    return coords - float2(i, j);
}

bool GpuSim__SimShader__Something(float4 u)
{
    return u.r > 0 + .001;
}

float FragSharpFramework__FragSharpStd__Float(float v)
{
    return floor(255 * v + 0.5);
}

bool GpuSim__SimShader__selected(float4 u)
{
    float val = u.b;
    return val >= 0.5019608 - .001;
}

float4 GpuSim__SelectedUnitColor__Get(float player)
{
    if (abs(player - 0.003921569) < .001)
    {
        return float4(0.0627451, 0.6666667, 0.0627451, 1.0);
    }
    if (abs(player - 0.007843138) < .001)
    {
        return float4(0.0627451, 0.6666667, 0.0627451, 1.0);
    }
    if (abs(player - 0.01176471) < .001)
    {
        return float4(0.0627451, 0.6666667, 0.0627451, 1.0);
    }
    if (abs(player - 0.01568628) < .001)
    {
        return float4(0.0627451, 0.6666667, 0.0627451, 1.0);
    }
    return float4(0.0, 0.0, 0.0, 0.0);
}

float4 GpuSim__UnitColor__Get(VertexToPixel psin, float player)
{
    if (abs(player - 0.003921569) < .001)
    {
        return tex2D(fs_param_FarColor, float2(2+.5,.5+ 1 + (int)player) * fs_param_FarColor_dxdy);
    }
    if (abs(player - 0.007843138) < .001)
    {
        return tex2D(fs_param_FarColor, float2(2+.5,.5+ 2 + (int)player) * fs_param_FarColor_dxdy);
    }
    if (abs(player - 0.01176471) < .001)
    {
        return tex2D(fs_param_FarColor, float2(2+.5,.5+ 3 + (int)player) * fs_param_FarColor_dxdy);
    }
    if (abs(player - 0.01568628) < .001)
    {
        return tex2D(fs_param_FarColor, float2(2+.5,.5+ 4 + (int)player) * fs_param_FarColor_dxdy);
    }
    if (abs(player - 0.003921569) < .001)
    {
        return float4(0, 0, 0, 1.0);
    }
    if (abs(player - 0.007843138) < .001)
    {
        return float4(0.9176471, 0.5215687, 0.3372549, 1.0);
    }
    if (abs(player - 0.01176471) < .001)
    {
        return float4(0.4, 0.85, 0.65, 0.5);
    }
    if (abs(player - 0.01568628) < .001)
    {
        return float4(0.4, 0.4, 0.85, 0.5);
    }
    return float4(0.0, 0.0, 0.0, 0.0);
}

float4 GpuSim__DrawUnits__SolidColor(VertexToPixel psin, float4 data, float4 unit)
{
    return GpuSim__SimShader__selected(data) ? GpuSim__SelectedUnitColor__Get(unit.g) : GpuSim__UnitColor__Get(psin, unit.g);
}

float4 GpuSim__DrawUnits__Sprite(VertexToPixel psin, float4 d, float4 u, float2 pos, float frame, sampler Texture, float2 Texture_size, float2 Texture_dxdy, float selection_blend, float selection_size, bool solid_blend_flag, float solid_blend)
{
    if (pos.x > 1 + .001 || pos.y > 1 + .001 || pos.x < 0 - .001 || pos.y < 0 - .001)
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
    bool draw_selected = GpuSim__SimShader__selected(d) && pos.y > selection_size + .001;
    pos.x += floor(frame);
    pos.y += (FragSharpFramework__FragSharpStd__Float(d.r) - 1) + 4 * (FragSharpFramework__FragSharpStd__Float(u.g) - 1);
    pos *= float2(1.0 / 32, 1.0 / 32);
    float4 clr = tex2D(Texture, pos);
    if (draw_selected)
    {
        float a = clr.a * selection_blend;
        clr = a * clr + (1 - a) * GpuSim__SelectedUnitColor__Get(u.g);
    }
    if (solid_blend_flag)
    {
        clr = solid_blend * clr + (1 - solid_blend) * GpuSim__DrawUnits__SolidColor(psin, d, u);
    }
    return clr;
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

float2 GpuSim__SimShader__direction_to_vec(float direction)
{
    float angle = (direction * 255 - 1) * (3.141593 / 2.0);
    return GpuSim__SimShader__IsValid(direction) ? float2(cos(angle), sin(angle)) : float2(0, 0);
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
    float4 cur = tex2D(fs_param_CurrentData, psin.TexCoords + (float2(0, 0)) * fs_param_CurrentData_dxdy), pre = tex2D(fs_param_PreviousData, psin.TexCoords + (float2(0, 0)) * fs_param_PreviousData_dxdy);
    float4 cur_unit = tex2D(fs_param_CurrentUnits, psin.TexCoords + (float2(0, 0)) * fs_param_CurrentUnits_dxdy), pre_unit = tex2D(fs_param_PreviousUnits, psin.TexCoords + (float2(0, 0)) * fs_param_PreviousUnits_dxdy);
    if (!(GpuSim__SimShader__IsUnit(cur_unit)) && !(GpuSim__SimShader__IsUnit(pre_unit)))
    {
        __FinalOutput.Color = output;
        return __FinalOutput;
    }
    float2 subcell_pos = GpuSim__SimShader__get_subcell_pos(psin, fs_param_CurrentData_size);
    if (GpuSim__SimShader__Something(cur) && abs(cur.g - 0.003921569) < .001)
    {
        if (fs_param_s > 0.5 + .001)
        {
            pre = cur;
        }
        float _s = abs(cur_unit.a - 0.0) < .001 ? fs_param_t : fs_param_s;
        float frame = _s * 6 + FragSharpFramework__FragSharpStd__Float(cur_unit.a);
        output += GpuSim__DrawUnits__Sprite(psin, pre, pre_unit, subcell_pos, frame, fs_param_Texture, fs_param_Texture_size, fs_param_Texture_dxdy, fs_param_selection_blend, fs_param_selection_size, false, fs_param_solid_blend);
    }
    else
    {
        float frame = fs_param_s * 6 + FragSharpFramework__FragSharpStd__Float(0.02352941);
        if (GpuSim__SimShader__IsValid(cur.r))
        {
            float prior_dir = GpuSim__SimShader__prior_direction(cur);
            cur.r = prior_dir;
            float2 offset = (1 - fs_param_s) * GpuSim__SimShader__direction_to_vec(prior_dir);
            output += GpuSim__DrawUnits__Sprite(psin, cur, cur_unit, subcell_pos + offset, frame, fs_param_Texture, fs_param_Texture_size, fs_param_Texture_dxdy, fs_param_selection_blend, fs_param_selection_size, false, fs_param_solid_blend);
        }
        if (GpuSim__SimShader__IsValid(pre.r) && output.a < 0.025 - .001)
        {
            float2 offset = -(fs_param_s) * GpuSim__SimShader__direction_to_vec(pre.r);
            output += GpuSim__DrawUnits__Sprite(psin, pre, pre_unit, subcell_pos + offset, frame, fs_param_Texture, fs_param_Texture_size, fs_param_Texture_dxdy, fs_param_selection_blend, fs_param_selection_size, false, fs_param_solid_blend);
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