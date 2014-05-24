using FragSharpFramework;

namespace GpuSim
{
    public partial class Movement_Phase1 : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Current)
        {
            data here = Current[Here], output = data.Nothing;

            // If something is here, they have the right to stay.
            if (Something(here))
            {
                output = here;
                output.change = Change.Stayed;
                return output;
            }

            // Otherwise, check each direction to see if something is incoming.
            data
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
        data FragmentShader(VertexOut vertex, Field<data> Current, Field<data> Next)
        {
            data next = Next[Here];
            data here = Current[Here];

            data ahead = Next[dir_to_vec(here.direction)];
            if (ahead.change == Change.Moved && ahead.direction == here.direction)
                next = data.Nothing;

            set_prior_direction(ref next, next.direction);

            return next;
        }
    }

    public partial class Movement_Convect : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<data> Current)
        {
            data here = Current[Here], output = data.Nothing;

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
        data FragmentShader(VertexOut vertex, Field<data> TargetData, Field<unit> Data, Field<extra> Extra, Field<data> Current, Field<data> Paths_Right, Field<data> Paths_Left, Field<data> Paths_Up, Field<data> Paths_Down)
        {
            data here = Current[Here];
            extra extra_here = Extra[Here];

            if (Something(here))
            {
                data path = data.Nothing;

                data
                    right = Current[RightOne],
                    up    = Current[UpOne],
                    left  = Current[LeftOne],
                    down  = Current[DownOne];

                data
                    right_path = Paths_Right[Here],
                    up_path    = Paths_Up   [Here],
                    left_path  = Paths_Left [Here],
                    down_path  = Paths_Down [Here];

                data target = TargetData[Here];
                unit data_here   = Data[Here];
                vec2 Destination = unpack_vec2((vec4)target);

                float cur_angle    = atan(vertex.TexCoords.y - Destination.y * TargetData.DxDy.y, vertex.TexCoords.x - Destination.x * TargetData.DxDy.x);
                cur_angle          = (cur_angle + 3.14159f) / (2 * 3.14159f);
                float target_angle = extra_here.target_angle;

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
        data FragmentShader(VertexOut vertex, Field<vec4> TargetData, Field<unit> Unit, Field<extra> Extra, Field<data> Data, Field<vec4> PathToOtherTeams)
        {
            data  data_here  = Data[Here];

            if (Something(data_here))
            {
                data path = data.Nothing;

                // Get info for this unit
                unit  here       = Unit[Here];
                extra extra_here = Extra[Here];

                // Get nearby paths to other teams
                vec4
                    _value_right = PathToOtherTeams[RightOne],
                    _value_up    = PathToOtherTeams[UpOne],
                    _value_left  = PathToOtherTeams[LeftOne],
                    _value_down  = PathToOtherTeams[DownOne];

                // Get specific paths to enemies of this particular unit
                float value_right = 1, value_left = 1, value_up = 1, value_down = 1;
                if (here.team == Team.One)
                {
                    value_right = _value_right.x;
                    value_left  = _value_left.x;
                    value_up    = _value_up.x;
                    value_down  = _value_down.x;
                }
                else if (here.team == Team.Two)
                {
                    value_right = _value_right.y;
                    value_left  = _value_left.y;
                    value_up    = _value_up.y;
                    value_down  = _value_down.y;
                }

                float auto_attack_cutoff = _4;

                float min = 256;
                float hold_dir = data_here.direction;
                if (data_here.action == UnitAction.Attacking)
                {
                    if (value_right < min) { data_here.direction = Dir.Right; min = value_right; }
                    if (value_up    < min) { data_here.direction = Dir.Up;    min = value_up; }
                    if (value_left  < min) { data_here.direction = Dir.Left;  min = value_left; }
                    if (value_down  < min) { data_here.direction = Dir.Down;  min = value_down; }
                }

                if (min > auto_attack_cutoff) data_here.direction = hold_dir;

                // If we aren't attacking, or if a unit is too far away
                if (min > auto_attack_cutoff && data_here.action == UnitAction.Attacking || data_here.action == UnitAction.Moving)
                {
                    NaivePathfind(vertex, Data, TargetData, here, ref data_here, ref extra_here);
                }
            }

            return data_here;
        }

        void NaivePathfind(VertexOut vertex, Field<data> Current, Field<vec4> TargetData, unit data, ref data here, ref extra extra_here)
        {
            float dir = 0;

            vec4 target = TargetData[Here];

            // Unpack packed info
            vec2 CurPos = vertex.TexCoords * TargetData.Size;
            vec2 Destination = unpack_vec2((vec4)target);

            // Get nearby units
            data
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne];

            if (Destination.x > CurPos.x + .75f) dir = Dir.Right;
            if (Destination.x < CurPos.x - .75f) dir = Dir.Left;
            if (Destination.y > CurPos.y + .75f) dir = Dir.Up;
            if (Destination.y < CurPos.y - .75f) dir = Dir.Down;

            // Simple pathing: Go toward the cardinal direction that is furthest away. If something is in your way, go perpendicularly, assuming you also need to go in that direction.
            vec2 diff = Destination - CurPos;
            vec2 mag = abs(diff);
            if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x > CurPos.x + 1 && !Something(right)) dir = Dir.Right;
            if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y > CurPos.y + 1 && !Something(up))    dir = Dir.Up;
            if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x < CurPos.x - 1 && !Something(left))  dir = Dir.Left;
            if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y < CurPos.y - 1 && !Something(down))  dir = Dir.Down;

            /*
            // Heading conserving pathing: Tries to maintain the original heading of the unit as it paths toward its destination.
            // Calculate angles
            float cur_angle = atan(CurPos.y - Destination.y, CurPos.x - Destination.x);
            cur_angle = (cur_angle + 3.14159f) / (2 * 3.14159f);
            float target_angle = extra_here.target_angle;

            float grace = 1;
            if (Destination.x > CurPos.x + 1)
            {
                dir = Dir.Right;

                if (Something(right) && Destination.x - CurPos.x > 5) grace = -1;

                if (Destination.y < CurPos.y - grace)
                {
                    if (cur_angle < target_angle || Something(right))
                    {
                        dir = Dir.Down;

                        if (Something(down))
                            dir = Dir.Right;
                    }
                }
                else if (Destination.y > CurPos.y + grace)
                {
                    if (cur_angle > target_angle || Something(right))
                    {
                        dir = Dir.Up;

                        if (Something(up))
                            dir = Dir.Right;
                    }
                }
            }
            else if (Destination.x < CurPos.x - 1)
            {
                dir = Dir.Left;

                if (Something(left) && CurPos.x - Destination.x > 5) grace = -1;

                if (Destination.y < CurPos.y - grace)
                {
                    if (cur_angle > target_angle || Something(left))
                    {
                        dir = Dir.Down;

                        if (Something(down))
                            dir = Dir.Left;
                    }
                }
                else if (Destination.y > CurPos.y + grace)
                {
                    if (cur_angle < target_angle || Something(left))
                    {
                        dir = Dir.Up;

                        if (Something(up))
                            dir = Dir.Left;
                    }
                }
            }
            else
            {
                if (Destination.y > CurPos.y + 1)
                {
                    dir = Dir.Up;
                    if (Something(up))
                    {
                        if (Destination.x > CurPos.x && !Something(right)) dir = Dir.Right;
                        if (Destination.x < CurPos.x && !Something(left))  dir = Dir.Left;
                    }
                }

                if (Destination.y < CurPos.y - 1)
                {
                    dir = Dir.Down;
                    if (Something(up))
                    {
                        if (Destination.x > CurPos.x && !Something(right)) dir = Dir.Right;
                        if (Destination.x < CurPos.x && !Something(left)) dir = Dir.Left;
                    }
                }
            }
            */

            if (IsValid(dir))
                here.direction = dir;
            else
                here.action = UnitAction.Stopped;
        }
    }
}
