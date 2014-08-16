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
// Texture Sampler for fs_param_Current, using register location 1
float2 fs_param_Current_size;
float2 fs_param_Current_dxdy;

Texture fs_param_Current_Texture;
sampler fs_param_Current : register(s1) = sampler_state
{
    texture   = <fs_param_Current_Texture>;
    MipFilter = Point;
    MagFilter = Point;
    MinFilter = Point;
    AddressU  = Clamp;
    AddressV  = Clamp;
};

// Texture Sampler for fs_param_Random, using register location 2
float2 fs_param_Random_size;
float2 fs_param_Random_dxdy;

Texture fs_param_Random_Texture;
sampler fs_param_Random : register(s2) = sampler_state
{
    texture   = <fs_param_Random_Texture>;
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

bool GpuSim__SimShader__IsStationary(float4 d)
{
    return d.r >= 0.01960784 - .001;
}

float FragSharpFramework__FragSharpStd__fint_round(float v)
{
    return floor(255 * v + 0.5) * 0.003921569;
}

float GpuSim__SimShader__RndFint(float rnd, float f1, float f2)
{
    float val = rnd * (f2 - f1) + f1;
    return FragSharpFramework__FragSharpStd__fint_round(val);
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
    float4 here = tex2D(fs_param_Current, psin.TexCoords + (float2(0, 0)) * fs_param_Current_dxdy), output = float4(0, 0, 0, 0);
    if (GpuSim__SimShader__Something(here))
    {
        output = here;
        if (!(GpuSim__SimShader__IsStationary(here)))
        {
            output.g = 0.003921569;
        }
        __FinalOutput.Color = output;
        return __FinalOutput;
    }
    float4 right = tex2D(fs_param_Current, psin.TexCoords + (float2(1, 0)) * fs_param_Current_dxdy), up = tex2D(fs_param_Current, psin.TexCoords + (float2(0, 1)) * fs_param_Current_dxdy), left = tex2D(fs_param_Current, psin.TexCoords + (float2(-(1), 0)) * fs_param_Current_dxdy), down = tex2D(fs_param_Current, psin.TexCoords + (float2(0, -(1))) * fs_param_Current_dxdy);
    float rnd = GpuSim__SimShader__RndFint(tex2D(fs_param_Random, psin.TexCoords + (float2(0, 0)) * fs_param_Random_dxdy).x, 0.0, 0.01176471);
    if (abs(rnd - 0.0) < .001)
    {
        if (abs(right.a - 0.0) > .001 && abs(right.a - 0.01176471) > .001 && abs(right.r - 0.01176471) < .001)
        {
            output = right;
        }
        if (abs(up.a - 0.0) > .001 && abs(up.a - 0.01176471) > .001 && abs(up.r - 0.01568628) < .001)
        {
            output = up;
        }
        if (abs(left.a - 0.0) > .001 && abs(left.a - 0.01176471) > .001 && abs(left.r - 0.003921569) < .001)
        {
            output = left;
        }
        if (abs(down.a - 0.0) > .001 && abs(down.a - 0.01176471) > .001 && abs(down.r - 0.007843138) < .001)
        {
            output = down;
        }
    }
    else
    {
        if (abs(rnd - 0.003921569) < .001)
        {
            if (abs(down.a - 0.0) > .001 && abs(down.a - 0.01176471) > .001 && abs(down.r - 0.007843138) < .001)
            {
                output = down;
            }
            if (abs(right.a - 0.0) > .001 && abs(right.a - 0.01176471) > .001 && abs(right.r - 0.01176471) < .001)
            {
                output = right;
            }
            if (abs(up.a - 0.0) > .001 && abs(up.a - 0.01176471) > .001 && abs(up.r - 0.01568628) < .001)
            {
                output = up;
            }
            if (abs(left.a - 0.0) > .001 && abs(left.a - 0.01176471) > .001 && abs(left.r - 0.003921569) < .001)
            {
                output = left;
            }
        }
        else
        {
            if (abs(rnd - 0.007843138) < .001)
            {
                if (abs(left.a - 0.0) > .001 && abs(left.a - 0.01176471) > .001 && abs(left.r - 0.003921569) < .001)
                {
                    output = left;
                }
                if (abs(down.a - 0.0) > .001 && abs(down.a - 0.01176471) > .001 && abs(down.r - 0.007843138) < .001)
                {
                    output = down;
                }
                if (abs(right.a - 0.0) > .001 && abs(right.a - 0.01176471) > .001 && abs(right.r - 0.01176471) < .001)
                {
                    output = right;
                }
                if (abs(up.a - 0.0) > .001 && abs(up.a - 0.01176471) > .001 && abs(up.r - 0.01568628) < .001)
                {
                    output = up;
                }
            }
            else
            {
                if (abs(rnd - 0.01176471) < .001)
                {
                    if (abs(up.a - 0.0) > .001 && abs(up.a - 0.01176471) > .001 && abs(up.r - 0.01568628) < .001)
                    {
                        output = up;
                    }
                    if (abs(left.a - 0.0) > .001 && abs(left.a - 0.01176471) > .001 && abs(left.r - 0.003921569) < .001)
                    {
                        output = left;
                    }
                    if (abs(down.a - 0.0) > .001 && abs(down.a - 0.01176471) > .001 && abs(down.r - 0.007843138) < .001)
                    {
                        output = down;
                    }
                    if (abs(right.a - 0.0) > .001 && abs(right.a - 0.01176471) > .001 && abs(right.r - 0.01176471) < .001)
                    {
                        output = right;
                    }
                }
            }
        }
    }
    if (GpuSim__SimShader__Something(output))
    {
        output.g = 0.0;
        __FinalOutput.Color = output;
        return __FinalOutput;
    }
    else
    {
        output = here;
        output.g = 0.003921569;
        __FinalOutput.Color = output;
        return __FinalOutput;
    }
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