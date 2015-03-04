using FragSharpFramework;

namespace Game
{
    public partial class UpdateRandomField : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, PeriodicField<vec4> Random)
        {
            vec4 val = Random[RightOne];

            return vec(val.y, val.z, val.w, val.x);
        }
    }
}
