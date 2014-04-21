using FragSharpFramework;

namespace GpuSim
{
    public partial class Movement_Phase1 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current)
        {
            unit here = Current[Here], output = unit.Nothing;

            // If something is here, they have the right to stay.
            if (Something(here))
            {
                output = here;
                output.change = Change.Stayed;
                return output;
            }

            // Otherwise, check each direction to see if something is incoming.
            unit
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne];

            if (right.direction == Dir.Left)  output = right;
            if (up.direction    == Dir.Down)  output = up;
            if (left.direction  == Dir.Right) output = left;
            if (down.direction  == Dir.Up)    output = down;

            output.change = Change.Moved;

            return output;
        }
    }

    public partial class Movement_Phase2 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, UnitField Current, UnitField Next, UnitField Paths)
        {
            unit next = Next[Here];
            unit here = Current[Here];

            unit ahead = Next[dir_to_vec(here.direction)];
            if (ahead.change == Change.Moved && ahead.direction == here.direction)
                next = unit.Nothing;

            next.prior_direction = next.direction;

            // If unit hasn't moved, change direction
            //if (next.a == here.a && Something(next))
            //    TurnLeft(ref next);

            unit path = Paths[Here];
            if (Something(next) && (path.g > 0 || path.b > 0) && IsValid(path.direction))
                next.direction = path.direction;

            return next;
        }
    }
}
