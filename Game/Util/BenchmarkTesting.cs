#define BENCHMARKING_SHADERS

using Microsoft.Xna.Framework.Graphics;
using FragSharpFramework;

namespace Game
{
    public static class BenchmarkTests
    {
        public static void Run(RenderTarget2D Current, RenderTarget2D Previous)
        {
            return;

            for (int i = 0; i < 100; i++)
            {
                // Testing whether it's faster to pack/unpack variables or to do an extra texture lookup.
                // Conclusion: Better to pack/unpack
                //BenchmarkTest_MathPackingVec.Apply(Current, Output: null); // 7 fps
                //BenchmarkTest_MathPacking.Apply(Current, Output: null); // 7 fps
                //BenchmarkTest_TextureLookup4x4.Apply(Current, Previous, CurData, PreData, Output: null); // 3.5 fps
                //for (int j = 0; j < 4; j++) BenchmarkTest_TextureLookup1x5.Apply(Current, Output: null); // 3.5 fps

                // Testing how much *more* expensive it is to lookup up+right+left+down.
                // Conclusion: Additional lookups are more expensive, but not drastically so (roughly 50% for the 4 extra lookups).
                //BenchmarkTest_TextureLookup1x5.Apply(Current, Output: null); // 11.5 fps
                //BenchmarkTest_TextureLookup1x1.Apply(Current, Output: null); // 15.5 fps

                // Testing how expensive math and simple conditionals are compared to vanilla lookups.
                // Conclusion: It appears some math and conditionals doesn't hurt too much, but this may be an artifact. Needs more investigation.
                //BenchmarkTest_TextureLookup1x1.Apply(Current, Output: null); // 15.5 fps
                //BenchmarkTest_TextureLookupWithComplexMath.Apply(Current, Output: null); // 15.5 fps
                //BenchmarkTest_TextureLookupWithConditional.Apply(Current, Output: null); // 15.5 fps
            }
        }
    }

#if BENCHMARKING_SHADERS
    public partial class BenchmarkTest_TextureLookup4x4 : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s1, Field<vec4> s2, Field<vec4> s3, Field<vec4> s4)
        {
            return (
                   s1[Here] + s1[RightOne] + s1[LeftOne] + s1[UpOne] + s1[DownOne] +
                   s2[Here] + s2[RightOne] + s2[LeftOne] + s2[UpOne] + s2[DownOne] +
                   s3[Here] + s3[RightOne] + s3[LeftOne] + s3[UpOne] + s3[DownOne] +
                   s4[Here] + s4[RightOne] + s4[LeftOne] + s4[UpOne] + s4[DownOne]
                   ) / 16.0f;
        }
    }

    public partial class BenchmarkTest_TextureLookup1x1 : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
        {
            return (
                   s[Here]
                   ) / 4.0f;
        }
    }

    public partial class BenchmarkTest_TextureLookupWithConditional : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
        {
            var here = s[Here];

            vec4 output = vec4.Zero;

            if (here.x == _1)
            {
                if (here.x > here.y)
                    output.x = here.z * here.x;
                else
                    output.x = here.w * here.w;

                if (here.z > here.x)
                    output.y = here.x * here.w;
                else
                    output.y = here.y * here.x;

                if (here.z > here.w)
                    output.z = here.x * here.w;
                else
                    output.z = here.w * here.y;

                if (here.w > here.x)
                    output.w = here.z * here.w;
                else
                    output.w = here.x * here.x;
            }
            else
            {
                if (here.y > here.x)
                    output.x = here.z * here.y;
                else
                    output.x = here.w * here.w;

                if (here.z > here.y)
                    output.y = here.y * here.w;
                else
                    output.y = here.y * here.y;

                if (here.z > here.w)
                    output.z = here.y * here.w;
                else
                    output.z = here.w * here.y;

                if (here.w > here.y)
                    output.w = here.z * here.w;
                else
                    output.w = here.y * here.y;
            }

            return output;
        }
    }

    public partial class BenchmarkTest_TextureLookupWithComplexMath : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
        {
            var here = s[Here];

            var output = vec(
                sin(cos(here.x) * sin(sin(here.y))),
                cos(sin(here.y) * here.z),
                sin(cos(here.z + here.w)),
                cos(sin(here.x + here.y) * .125f));
            return output;

            // This is the same speed
            //var output = vec(
            //    sin(cos(here.x)),
            //    sin(cos(here.y)),
            //    sin(cos(here.z)),
            //    sin(cos(here.w)));
            //return output;
            
            // This is much slower. Why?
            //return sin(cos(here));
        }
    }

    public partial class BenchmarkTest_TextureLookup1x5 : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
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
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
        {
            vec4 output = vec4.Zero;

            vec4
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
        vec4 FragmentShader(VertexOut vertex, Field<vec4> s)
        {
            vec4 output = vec4.Zero;

            vec4
                right = s[RightOne],
                up    = s[UpOne],
                left  = s[LeftOne],
                down  = s[DownOne];

            output = MathPacking(right) + MathPacking(left) + MathPacking(up) + MathPacking(down);

            return output;
        }
    }
#endif
}
