using FragSharpFramework;

namespace GpuSim
{
    public partial class Movement_Phase1 : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Current, Field<vec4> Random)
        {
            data here = Current[Here], output = data.Nothing;

            // If something is here, they have the right to stay.
            if (Something(here))
            {
                output = here;
                
                if (!IsStationary(here)) output.change = Change.Stayed;
                return output;
            }

            // Otherwise, check each direction to see if something is incoming.
            data
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne];

            float rnd = RndFint(Random[Here].x, _0, _3);
            if (rnd == _0)
            {
                if (right.action != UnitAction.Stopped && right.action != UnitAction.Guard && right.direction == Dir.Left) output = right;
                if (up.action != UnitAction.Stopped && up.action != UnitAction.Guard && up.direction == Dir.Down) output = up;
                if (left.action != UnitAction.Stopped && left.action != UnitAction.Guard && left.direction == Dir.Right) output = left;
                if (down.action != UnitAction.Stopped && down.action != UnitAction.Guard && down.direction == Dir.Up) output = down;
            }
            else if (rnd == _1)
            {
                if (down.action != UnitAction.Stopped && down.action != UnitAction.Guard && down.direction == Dir.Up) output = down;
                if (right.action != UnitAction.Stopped && right.action != UnitAction.Guard && right.direction == Dir.Left) output = right;
                if (up.action != UnitAction.Stopped && up.action != UnitAction.Guard && up.direction == Dir.Down) output = up;
                if (left.action != UnitAction.Stopped && left.action != UnitAction.Guard && left.direction == Dir.Right) output = left;
            }
            else if (rnd == _2)
            {
                if (left.action != UnitAction.Stopped && left.action != UnitAction.Guard && left.direction == Dir.Right) output = left;
                if (down.action != UnitAction.Stopped && down.action != UnitAction.Guard && down.direction == Dir.Up) output = down;
                if (right.action != UnitAction.Stopped && right.action != UnitAction.Guard && right.direction == Dir.Left) output = right;
                if (up.action != UnitAction.Stopped && up.action != UnitAction.Guard && up.direction == Dir.Down) output = up;
            }
            else if (rnd == _3)
            {
                if (up.action != UnitAction.Stopped && up.action != UnitAction.Guard && up.direction == Dir.Down) output = up;
                if (left.action != UnitAction.Stopped && left.action != UnitAction.Guard && left.direction == Dir.Right) output = left;
                if (down.action != UnitAction.Stopped && down.action != UnitAction.Guard && down.direction == Dir.Up) output = down;
                if (right.action != UnitAction.Stopped && right.action != UnitAction.Guard && right.direction == Dir.Left) output = right;
            }


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

            if (IsStationary(next)) return next;

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
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Data, Field<data> CurrentData)
        {
            data here = CurrentData[Here];
            vec4 output = vec4.Zero;

            if (Something(here))
            {
                if (Stayed(here))
                    output = Data[Here];
                else
                    output = Data[dir_to_vec(Reverse(prior_direction(here)))];
            }

            return output;
        }
    }

    public partial class Movement_UpdateDirection_WithAaPathfinding : SimShader
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

                data target      = TargetData[Here];
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

    public partial class Movement_UpdateDirection_RemoveDead : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<vec4> TargetData, Field<unit> Unit, Field<extra> Extra, Field<data> Data, Field<data> PrevData, Field<vec4> PathToOtherTeams,
                            Field<geo> Geo, Field<geo> AntiGeo,
                            Field<dirward> DirwardRight, Field<dirward> DirwardLeft, Field<dirward> DirwardUp, Field<dirward> DirwardDown)
        {
            data  data_here  = Data[Here];

            if (Something(data_here))
            {
                data path = data.Nothing;

                // Get info for this unit
                unit  here       = Unit[Here];
                extra extra_here = Extra[Here];

                // Remove if dead unit
                if (here.anim == Anim.Dead && IsUnit(here))
                {
                    return data.Nothing;
                }

                building b = (building)(vec4)data_here;
                if (IsBuilding(here))
                {
                    // If this building is alive
                    if (data_here.direction == Dir.Stationary)
                    {
                        // If this is a building that has been hit enough times to explode
                        if (here.hit_count >= _5)
                        {
                            data_here.direction = Dir.StationaryDying;
                        }
                    }
                    else
                    {
                        // Otherwise remove it if the explosion animation is done
                        float frame = ExplosionSpriteSheet.ExplosionFrame(0, b);

                        if (frame >= ExplosionSpriteSheet.AnimLength)
                            return data.Nothing;
                    }
                }

                // Buildings can't move.
                if (IsBuilding(here))
                {
                    if (IsCenter((building)(vec4)data_here))
                    {
                        // Set the building direction toward its "target".
                        set_prior_direction(ref data_here, BuildingDirection(vertex, TargetData, (building)(vec4)data_here));
                    }
                    return data_here;
                }

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
                else if (here.team == Team.Three)
                {
                    value_right = _value_right.z;
                    value_left  = _value_left.z;
                    value_up    = _value_up.z;
                    value_down  = _value_down.z;
                }
                else if (here.team == Team.Four)
                {
                    value_right = _value_right.w;
                    value_left  = _value_left.w;
                    value_up    = _value_up.w;
                    value_down  = _value_down.w;
                }

                const float auto_attack_cutoff = _12;

                float min = 256;
                float hold_dir = data_here.direction;
                if (data_here.action == UnitAction.Attacking || data_here.action == UnitAction.Guard)
                {
                    if (value_right < min) { data_here.direction = Dir.Right; min = value_right; }
                    if (value_up    < min) { data_here.direction = Dir.Up;    min = value_up; }
                    if (value_left  < min) { data_here.direction = Dir.Left;  min = value_left; }
                    if (value_down  < min) { data_here.direction = Dir.Down;  min = value_down; }
                }

                if (min > auto_attack_cutoff) data_here.direction = hold_dir;

                // If we are guarding and a unit is close, switch to attacking
                if (min < auto_attack_cutoff && data_here.action == UnitAction.Guard)
                {
                    data_here.action = UnitAction.Attacking;
                }

                // If we aren't attacking, or if a unit is too far away
                if (min > auto_attack_cutoff && data_here.action == UnitAction.Attacking || data_here.action == UnitAction.Moving)
                {
                    NaivePathfind(vertex, Data, PrevData, TargetData,
                                  Geo, AntiGeo,
                                  DirwardRight, DirwardLeft, DirwardUp, DirwardDown,
                                  here, ref data_here, ref extra_here);
                }
            }

            return data_here;
        }

        void NaivePathfind(VertexOut vertex, Field<data> Current, Field<data> Previous, Field<vec4> TargetData,
                           Field<geo> Geo, Field<geo> AntiGeo,
                           Field<dirward> DirwardRight, Field<dirward> DirwardLeft, Field<dirward> DirwardUp, Field<dirward> DirwardDown,
                           unit data, ref data here, ref extra extra_here)
        {
            float dir = 0;

            vec4 target = TargetData[Here];

            // Unpack packed info
            vec2 CurPos = floor((vertex.TexCoords * TargetData.Size + vec(.5f, .5f)));
            vec2 Destination = floor(unpack_vec2((vec4)target));

            // Get nearby units
            data
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne];

            data
                prev_right = Previous[RightOne],
                prev_up    = Previous[UpOne],
                prev_left  = Previous[LeftOne],
                prev_down  = Previous[DownOne];

            if (Destination.x > CurPos.x + .75f) dir = Dir.Right;
            if (Destination.x < CurPos.x - .75f) dir = Dir.Left;
            if (Destination.y > CurPos.y + .75f) dir = Dir.Up;
            if (Destination.y < CurPos.y - .75f) dir = Dir.Down;

            // Simple pathing: Go toward the cardinal direction that is furthest away. If something is in your way, go perpendicularly, assuming you also need to go in that direction.
            vec2 diff = Destination - CurPos;
            vec2 mag = abs(diff);
            //if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x > CurPos.x + 1 && !Something(right)) dir = Dir.Right;
            //if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y > CurPos.y + 1 && !Something(up))    dir = Dir.Up;
            //if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x < CurPos.x - 1 && !Something(left))  dir = Dir.Left;
            //if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y < CurPos.y - 1 && !Something(down))  dir = Dir.Down;
            float dir2 = Dir.None;
            bool blocked = false;
            if (mag.x > mag.y && Destination.x > CurPos.x + 1) { dir = Dir.Right; blocked = Something(right) || Something(prev_right); }
            if (mag.y > mag.x && Destination.y > CurPos.y + 1) { dir = Dir.Up;    blocked = Something(up)    || Something(prev_up); }
            if (mag.x > mag.y && Destination.x < CurPos.x - 1) { dir = Dir.Left;  blocked = Something(left)  || Something(prev_left); }
            if (mag.y > mag.x && Destination.y < CurPos.y - 1) { dir = Dir.Down;  blocked = Something(down)  || Something(prev_down); }

            //if (mag.x > mag.y && Destination.y > CurPos.y + .5) dir2 = Dir.Up;
            //if (mag.x > mag.y && Destination.y < CurPos.y - .5) dir2 = Dir.Down;
            //if (mag.x <= mag.y && Destination.x > CurPos.x + 0) dir2 = Dir.Right;
            //if (mag.x <= mag.y && Destination.x < CurPos.x - 0) dir2 = Dir.Left;
            bool blocked2 = false;
            if (dir == Dir.Right || dir == Dir.Left)
            {
                if      (Destination.y > CurPos.y + 0) { dir2 = Dir.Up;    blocked2 = Something(up)    || Something(prev_up); }
                else if (Destination.y < CurPos.y - 0) { dir2 = Dir.Down;  blocked2 = Something(down)  || Something(prev_down); }
            }
            if (dir == Dir.Up || dir == Dir.Down)
            {
                if      (Destination.x > CurPos.x + 0) { dir2 = Dir.Right; blocked2 = Something(right) || Something(prev_right); }
                else if (Destination.x < CurPos.x - 0) { dir2 = Dir.Left;  blocked2 = Something(left)  || Something(prev_left); }
            }
            //dir2 = Dir.Right;

            // Check geodesics
            geo geo_here = Geo[Here];
            
            dirward dirward_here = dirward.Nothing;
            bool other_side = false;
            if      (dir == Dir.Right) { dirward_here = DirwardRight[Here]; other_side = Destination.x > wall_pos(dirward_here); }
            else if (dir == Dir.Left)  { dirward_here = DirwardLeft[Here];  other_side = Destination.x < wall_pos(dirward_here); }
            else if (dir == Dir.Up)    { dirward_here = DirwardUp[Here];    other_side = Destination.y > wall_pos(dirward_here); }
            else if (dir == Dir.Down)  { dirward_here = DirwardDown[Here];  other_side = Destination.y < wall_pos(dirward_here); }

            dirward dirward_here2 = dirward.Nothing;
            bool other_side2 = false;
            if      (dir2 == Dir.Right) { dirward_here2 = DirwardRight[Here]; other_side2 = Destination.x > wall_pos(dirward_here2); }
            else if (dir2 == Dir.Left)  { dirward_here2 = DirwardLeft[Here];  other_side2 = Destination.x < wall_pos(dirward_here2); }
            else if (dir2 == Dir.Up)    { dirward_here2 = DirwardUp[Here];    other_side2 = Destination.y > wall_pos(dirward_here2); }
            else if (dir2 == Dir.Down)  { dirward_here2 = DirwardDown[Here];  other_side2 = Destination.y < wall_pos(dirward_here2); }

            vec2 geo_id = geo_here.geo_id;
            //if (geo_here.dir > 0 && (geo_here.dist == _0 || blocked && other_side) &&// || blocked2 && other_side2) &&
            //   (
            //        ValidDirward(dirward_here)  && other_side  && dirward_here .geo_id == geo_id ||
            //        ValidDirward(dirward_here2) && other_side2 && dirward_here2.geo_id == geo_id
            //   ))
            if (geo_here.dir > 0 &&
               (
                    ValidDirward(dirward_here)  && other_side  && dirward_here.geo_id  == geo_id && (geo_here.dist == _0 || blocked  && other_side) ||
                    ValidDirward(dirward_here2) && other_side2 && dirward_here2.geo_id == geo_id && (geo_here.dist == _0 || blocked2 && other_side2) 
               ))
            {
                dir = geo_here.dir;
                //float avoid = Reverse(prior_direction(here));
                //if (IsValid(geo_here.dir) && geo_here.dir != avoid)
                //    dir = geo_here.dir;
                //else
                //    dir = dir2;
            }
            else
            {
                if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x > CurPos.x + 1 && !Something(right)) dir = Dir.Right;
                if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y > CurPos.y + 1 && !Something(up))    dir = Dir.Up;
                if ((mag.x > mag.y || diff.y > 0 && Something(up)    || diff.y < 0 && Something(down)) && Destination.x < CurPos.x - 1 && !Something(left))  dir = Dir.Left;
                if ((mag.y > mag.x || diff.x > 0 && Something(right) || diff.x < 0 && Something(left)) && Destination.y < CurPos.y - 1 && !Something(down))  dir = Dir.Down;
            }
            //if (Something(Current[dir_to_vec(dir)])) dir = dir2;

            if (IsValid(dir))
            {
                here.direction = dir;
            }
            else
            {
                if (here.action == UnitAction.Attacking)
                    here.action = UnitAction.Guard;
            }
        }

        float BuildingDirection(VertexOut vertex, Field<vec4> TargetData, building here)
        {
            float dir = Dir.Right;

            vec4 target = TargetData[Here];

            // Unpack packed info
            vec2 CurPos = vertex.TexCoords * TargetData.Size;
            vec2 Destination = unpack_vec2((vec4)target);

            vec2 diff = Destination - CurPos;
            vec2 mag = abs(diff);
            if (mag.x > mag.y && diff.x > 0) dir = Dir.Right;
            if (mag.x > mag.y && diff.x < 0) dir = Dir.Left;
            if (mag.y > mag.x && diff.y > 0) dir = Dir.Up;
            if (mag.y > mag.x && diff.y < 0) dir = Dir.Down;

            return dir;
        }
    }
}
