using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public partial class Identity : SimShader
    {
        public static void CopyFromTo(RenderTarget2D Source, ref RenderTarget2D Destination, ref RenderTarget2D Temporary)
        {
            Identity.Apply(Source, Output: Temporary);
            CoreMath.Swap(ref Destination, ref Temporary);
        }

        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, PeriodicField<vec4> Field)
        {
            return Field[Here];
        }
    }

    public partial class Shift : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, PeriodicField<vec4> Random, [Dir.Vals] float dir)
        {
            return Random[dir_to_vec(dir)];
        }
    }
}
