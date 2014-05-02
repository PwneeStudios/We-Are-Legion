using FragSharpFramework;

namespace GpuSim
{
    public partial class Pathfinding_ToOtherTeams : SimShader
    {
        /// <summary>
        /// Propagates the path to enemies of each team, story the result for Enemies of Team 1 in .x, of Team 2 in .y, etc.
        /// Four teams maximum.
        /// </summary>
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, VecField Path, UnitField Current, DataField CurData)
        {
            unit data = Current[Here];
            data cur_data = CurData[Here];

            vec4
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            vec4 output = min(right, up, left, down) + vec(_1,_1,_1,_1);

            if (Something(data))
            {
                output += 3 * vec(_1, _1, _1, _1);

                if (cur_data.team != Team.One)   output.r = _0;
                if (cur_data.team != Team.Two)   output.g = _0;
                if (cur_data.team != Team.Three) output.b = _0;
                if (cur_data.team != Team.Four)  output.a = _0;
            }

            return output;
        }
    }

    public partial class Pathfinding_Down : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Path, UnitField Current)
        {
            unit output = unit.Nothing;

            if (vertex.TexCoords.y - 2 * Path.DxDy.y < 0)
            {
                output.b = _1;
                return output;
            }

            output = PathHelper.Propagate(Path, Current, output);

            return output;
        }
    }

    public partial class Pathfinding_Up : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Path, UnitField Current)
        {
            unit output = unit.Nothing;

            if (vertex.TexCoords.y + 2 * Path.DxDy.y > 1)
            {
                output.b = _1;
                return output;
            }

            output = PathHelper.Propagate(Path, Current, output);

            return output;
        }
    }

    public partial class Pathfinding_Left : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Path, UnitField Current)
        {
            unit output = unit.Nothing;

            if (vertex.TexCoords.x - 2 * Path.DxDy.x < 0)
            {
                output.b = _1;
                return output;
            }

            output = PathHelper.Propagate(Path, Current, output);

            return output;
        }
    }

    public partial class Pathfinding_Right : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Path, UnitField Current)
        {
            unit output = unit.Nothing;
            
            if (vertex.TexCoords.x + 2 * Path.DxDy.x > 1)
            {
                output.b = _1;
                return output;
            }

            output = PathHelper.Propagate(Path, Current, output);

            return output;
        }
    }

    public class PathHelper : SimShader
    {
        public static unit Propagate(UnitField Path, UnitField Current, unit output)
        {
            unit data = Current[Here];

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

            //|| data.direction != output.direction || data.action == Action.Stopped))
            if (Something(data))
            {
                //if (data.change == Change.Stayed)
                //if (data.direction != output.direction || data.action == Action.Stopped)
                //if (data.action == Action.Stopped)
                //    min += _1;
            }

            output.g = floor(min) / 255.0f;
            output.b = min - floor(min);
            return output;
        }
    }
}
