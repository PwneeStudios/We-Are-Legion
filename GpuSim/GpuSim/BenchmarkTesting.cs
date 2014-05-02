using FragSharpFramework;

namespace GpuSim
{
    public static class BenchmarkTests
    {
        public static void Run()
        {
            return;

            for (int i = 0; i < 100; i++)
            {
                //BenchmarkTest_MathPackingVec.Apply(Current, Output: null); // 7 fps
                //BenchmarkTest_MathPacking.Apply(Current, Output: null); // 7 fps
                //BenchmarkTest_TextureLookup4x4.Apply(Current, Previous, CurData, PreData, Output: null); // 3.5 fps
                //for (int j = 0; j < 4; j++) BenchmarkTest_TextureLookup1x4.Apply(Current, Output: null); // 3.5 fps
            }
        }
    }

    public partial class BenchmarkTest_TextureLookup4x4 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Sampler s1, Sampler s2, Sampler s3, Sampler s4)
        {
            return (
                   s1[Here] + s1[RightOne] + s1[LeftOne] + s1[UpOne] + s1[DownOne] +
                   s2[Here] + s2[RightOne] + s2[LeftOne] + s2[UpOne] + s2[DownOne] +
                   s3[Here] + s3[RightOne] + s3[LeftOne] + s3[UpOne] + s3[DownOne] +
                   s4[Here] + s4[RightOne] + s4[LeftOne] + s4[UpOne] + s4[DownOne]
                   ) / 16.0f;
        }
    }

    public partial class BenchmarkTest_TextureLookup1x4 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Sampler s)
        {
            return (
                   s[Here] + s[RightOne] + s[LeftOne] + s[UpOne] + s[DownOne]
                   ) / 4.0f;
        }
    }

    public partial class BenchmarkTest_MathPacking : SimShader
    {
        float MathPacking(float c)
        {
            //return 4 * floor(c * .25f);
            float x1 = floor(c / 4.0f);
            float x2 = c - 3.20f * x1;
            //float x2 = c % 4.0f;

            return 4 * (x1 + 1) + x2;
        }

        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Sampler s)
        {
            color output = color.TransparentBlack;

            color
                right = s[RightOne],
                up = s[UpOne],
                left = s[LeftOne],
                down = s[DownOne];

            output.r = MathPacking(right.r) + MathPacking(right.g) + MathPacking(right.b) + MathPacking(right.a);
            output.g = MathPacking(left.r) + MathPacking(left.g) + MathPacking(left.b) + MathPacking(left.a);
            output.b = MathPacking(up.r) + MathPacking(up.g) + MathPacking(up.b) + MathPacking(up.a);
            output.a = MathPacking(down.r) + MathPacking(down.g) + MathPacking(down.b) + MathPacking(down.a);

            return output;
        }
    }

    public partial class BenchmarkTest_MathPackingVec : SimShader
    {
        vec4 MathPacking(vec4 c)
        {
            //return 4 * floor(c * .25f);
            var x1 = floor(c / 4.0f);
            var x2 = c - 3.20f * x1;
            
            return 4 * (x1+vec(1,1,1,1)) + x2;
        }

        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Sampler s)
        {
            color output = color.TransparentBlack;

            color
                right = s[RightOne],
                up    = s[UpOne],
                left  = s[LeftOne],
                down  = s[DownOne];

            output = MathPacking(right) + MathPacking(left) + MathPacking(up) + MathPacking(down);

            return output;
        }
    }
}
