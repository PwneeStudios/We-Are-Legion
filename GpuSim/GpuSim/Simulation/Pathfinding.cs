using FragSharpFramework;

namespace GpuSim
{
    public partial class Pathfinding : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Path, UnitField Current)
        {
            unit output = unit.Nothing;

            var r = length(vertex.TexCoords - vec(.5f, .5f));
            if (abs(r - .3f) < .00125)
            {
                output.b = _1;
                return output;
            }

            unit data = Current[Here];
            //if (Something(data))
            //{
            //    output.a = 1;
            //}

            unit
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne],
                here  = Path[Here];

            float
                val_right = 255 * right.g + right.b,
                val_up    = 255 * up.g    + up.b,
                val_left  = 255 * left.g  + left.b,
                val_down  = 255 * down.g  + down.b,
                val_here  = 255 * here.g  + here.b;

            float min = 255;
            if (val_here > 0 && val_here < min)
            {
                min = val_here;
                output.direction = here.direction;
            }
            if (val_right > 0 && val_right < min && right.a == 0)
            {
                min = val_right;
                output.direction = Dir.Right;
            }
            if (val_up > 0 && val_up < min && up.a == 0)
            {
                min = val_up;
                output.direction = Dir.Up;
            }
            if (val_left > 0 && val_left < min && left.a == 0)
            {
                min = val_left;
                output.direction = Dir.Left;
            }
            if (val_down > 0 && val_down < min && down.a == 0)
            {
                min = val_down;
                output.direction = Dir.Down;
            }

            min = min + _1;

            if (Something(data))
                min += _1;

            output.g = min / 255;
            output.b = min - floor(min);

            return output;
        }
    }
}
