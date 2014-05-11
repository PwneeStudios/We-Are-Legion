using FragSharpFramework;

namespace GpuSim
{
    public partial class Movement_Phase1 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Current)
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

            if (right.action != UnitAction.Stopped && right.direction == Dir.Left)  output = right;
            if (up   .action != UnitAction.Stopped && up.direction    == Dir.Down)  output = up;
            if (left .action != UnitAction.Stopped && left.direction  == Dir.Right) output = left;
            if (down .action != UnitAction.Stopped && down.direction  == Dir.Up)    output = down;

            if (Something(output))
            {
                output.change = Change.Moved;
                return output;
            }
            else
            {
                output = here;
                output.change = Change.Stayed;
                return output;
            }
        }
    }

    public partial class Movement_Phase2 : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Current, Field<unit> Next)
        {
            unit next = Next[Here];
            unit here = Current[Here];

            unit ahead = Next[dir_to_vec(here.direction)];
            if (ahead.change == Change.Moved && ahead.direction == here.direction)
                next = unit.Nothing;

            set_prior_direction(ref next, next.direction);

            return next;
        }
    }

    public partial class Movement_Convect : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Data, Field<unit> Current)
        {
            unit here = Current[Here], output = unit.Nothing;

            if (Something(here))
            {
                if (here.change == Change.Stayed)
                    output = Data[Here];
                else
                    output = Data[dir_to_vec(Reverse(prior_direction(here)))];
            }

            return output;
        }
    }

    public partial class Movement_UpdateDirection : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> TargetData, Field<data> Data, Field<unit> Current, Field<unit> Paths_Right, Field<unit> Paths_Left, Field<unit> Paths_Up, Field<unit> Paths_Down)
        {
            unit here = Current[Here];

            if (Something(here))
            {
                unit path = unit.Nothing;

                unit
                    right = Current[RightOne],
                    up    = Current[UpOne],
                    left  = Current[LeftOne],
                    down  = Current[DownOne];

                unit
                    right_path = Paths_Right[Here],
                    up_path    = Paths_Up   [Here],
                    left_path  = Paths_Left [Here],
                    down_path  = Paths_Down [Here];

                unit target = TargetData[Here];
                data data   = Data[Here];
                vec2 Destination = unpack_vec2((vec4)target);

                float cur_angle    = atan(vertex.TexCoords.y - Destination.y * TargetData.DxDy.y, vertex.TexCoords.x - Destination.x * TargetData.DxDy.x);
                cur_angle          = (cur_angle + 3.14159f) / (2 * 3.14159f);
                float target_angle = data.target_angle;

                if (Destination.x > vertex.TexCoords.x * TargetData.Size.x)
                {
                    path = right_path;

                    if (Destination.y < vertex.TexCoords.y * TargetData.Size.y)
                    {
                        if (cur_angle < target_angle || right_path.direction == Dir.Right && Something(right))
                        {
                            path = down_path;
                            if (Something(down))
                                path = right_path;
                        }
                    }
                    else
                    {
                        if (cur_angle > target_angle || right_path.direction == Dir.Right && Something(right))
                        {
                            path = up_path;
                            if (Something(up))
                                path = right_path;
                        }
                    }
                }
                else
                {
                    path = left_path;

                    if (Destination.y < vertex.TexCoords.y * TargetData.Size.y)
                    {
                        if (cur_angle > target_angle || left_path.direction == Dir.Left && Something(left))
                        {
                            path = down_path;
                            if (Something(down))
                                path = left_path;
                        }
                    }
                    else
                    {
                        if (cur_angle < target_angle || left_path.direction == Dir.Left && Something(left))
                        {
                            path = up_path;
                            if (Something(up))
                                path = left_path;
                        }
                    }
                }

                if ((path.g > 1 || path.b > 1) && IsValid(path.direction))
                {
                    here.direction = path.direction;
                }
            }

            return here;
        }
    }

    public partial class Movement_UpdateDirectionToEnemy : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> TargetData, Field<data> Data, Field<unit> Current, Field<vec4> PathToOtherTeams)
        {
            unit here = Current[Here];

            if (Something(here))
            {
                unit path = unit.Nothing;

                if (here.change == Change.Stayed)
                {
                    TurnLeft(ref here);
                }
                return here;

                // Get info for this unit
                data data = Data[Here];

                // Get nearby paths to other teams
                vec4
                    _value_right = PathToOtherTeams[RightOne],
                    _value_up    = PathToOtherTeams[UpOne],
                    _value_left  = PathToOtherTeams[LeftOne],
                    _value_down  = PathToOtherTeams[DownOne];

                // Get specific paths to enemies of this particular unit
                float value_right = 1, value_left = 1, value_up = 1, value_down = 1;
                if (data.team == Team.One)
                {
                    value_right = _value_right.x;
                    value_left  = _value_left.x;
                    value_up    = _value_up.x;
                    value_down  = _value_down.x;
                }
                else if (data.team == Team.Two)
                {
                    value_right = _value_right.y;
                    value_left  = _value_left.y;
                    value_up    = _value_up.y;
                    value_down  = _value_down.y;
                }

                float auto_attack_cutoff = _4;

                float min = 256;
                float hold_dir = here.direction;
                if (here.action == UnitAction.Attacking)
                {
                    if (value_right < min) { here.direction = Dir.Right; min = value_right; }
                    if (value_up    < min) { here.direction = Dir.Up;    min = value_up; }
                    if (value_left  < min) { here.direction = Dir.Left;  min = value_left; }
                    if (value_down  < min) { here.direction = Dir.Down;  min = value_down; }
                }

                if (min > auto_attack_cutoff) here.direction = hold_dir;

                // If we aren't attacking, or if a unit is too far away
                if (min > auto_attack_cutoff && here.action == UnitAction.Attacking || here.action == UnitAction.Moving)
                {
                    NaivePathfind(vertex, Current, TargetData, data, ref here);
                }
            }

            return here;
        }

        void NaivePathfind(VertexOut vertex, Field<unit> Current, Field<unit> TargetData, data data, ref unit here)
        {
            float dir = 0;

            unit target = TargetData[Here];

            // Unpack packed info
            vec2 Destination = unpack_vec2((vec4)target);
            float cur_angle = atan(vertex.TexCoords.y - Destination.y * TargetData.DxDy.y, vertex.TexCoords.x - Destination.x * TargetData.DxDy.x);
            cur_angle = (cur_angle + 3.14159f) / (2 * 3.14159f);
            float target_angle = data.target_angle;

            // Get nearby units
            unit
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne];

            if (Destination.x > vertex.TexCoords.x * TargetData.Size.x)
            {
                dir = Dir.Right;

                if (Destination.y < vertex.TexCoords.y * TargetData.Size.y)
                {
                    if (cur_angle < target_angle || Something(right))
                    {
                        dir = Dir.Down;

                        if (Something(down))
                            dir = Dir.Right;
                    }
                }
                else
                {
                    if (cur_angle > target_angle || Something(right))
                    {
                        dir = Dir.Up;

                        if (Something(up))
                            dir = Dir.Right;
                    }
                }
            }
            else
            {
                dir = Dir.Left;

                if (Destination.y < vertex.TexCoords.y * TargetData.Size.y)
                {
                    if (cur_angle > target_angle || Something(left))
                    {
                        dir = Dir.Down;

                        if (Something(down))
                            dir = Dir.Left;
                    }
                }
                else
                {
                    if (cur_angle < target_angle || Something(left))
                    {
                        dir = Dir.Up;

                        if (Something(up))
                            dir = Dir.Left;
                    }
                }
            }

            if (IsValid(dir))
                here.direction = dir;
        }
    }
}
