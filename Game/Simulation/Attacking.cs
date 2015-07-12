using FragSharpFramework;

namespace Game
{
    public partial class CheckForAttacking : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data, Field<vec4> Random, Field<magic> Magic)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];

            bool DoRaiseAnim = false;
            if (unit_here.anim == Anim.StartRaise) DoRaiseAnim = true;

            // Reset unit animation to stand/walk
            if (IsUnit(unit_here)) unit_here.anim = _0;

            if (Stayed(data_here) && unit_here.team != Team.None) // A unit has to be stationary to attack or be attacked. It must also have a team.
            {
                if (IsUnit(unit_here) && data_here.action == UnitAction.Attacking)
                {
                    unit facing = Unit[dir_to_vec(data_here.direction)];

                    if (facing.team != unit_here.team && facing.team != Team.None)
                    {
                        unit_here.anim = Anim.Attack;
                    }
                }

                // Check for being attacked
                data
                    data_right = Data[RightOne],
                    data_up    = Data[UpOne],
                    data_left  = Data[LeftOne],
                    data_down  = Data[DownOne];
                unit
                    unit_right = Unit[RightOne],
                    unit_up    = Unit[UpOne],
                    unit_left  = Unit[LeftOne],
                    unit_down  = Unit[DownOne];

                vec4 rnd = Random[Here];

                if (unit_here.type != UnitType.DragonLord && rnd.x > .7f || rnd.x > .915f)
                {
                    if (Something(data_right) && unit_right.team != unit_here.team && unit_right.team != Team.None && data_right.direction == Dir.Left  && data_right.action == UnitAction.Attacking && data_right.change == Change.Stayed ||
                        Something(data_left)  && unit_left.team  != unit_here.team && unit_left.team  != Team.None && data_left.direction  == Dir.Right && data_left.action  == UnitAction.Attacking && data_left.change  == Change.Stayed ||
                        Something(data_up)    && unit_up.team    != unit_here.team && unit_up.team    != Team.None && data_up.direction    == Dir.Down  && data_up.action    == UnitAction.Attacking && data_up.change    == Change.Stayed ||
                        Something(data_down)  && unit_down.team  != unit_here.team && unit_down.team  != Team.None && data_down.direction  == Dir.Up    && data_down.action  == UnitAction.Attacking && data_down.change  == Change.Stayed)
                    {
                        if (IsBuilding(unit_here))
                        {
                            unit_here.hit_count += _1;
                        }
                        else
                        {
                            unit_here.anim = Anim.Die;
                        }
                    }
                }
            }

            if (IsUnit(unit_here) && Magic[Here].kill == _true && !UnitIsFireImmune(unit_here))
            {
                unit_here.anim = Anim.Die;
            }

            if (IsUnit(unit_here) && DoRaiseAnim)
            {
                unit_here.anim = Anim.DoRaise;
            }
            
            return unit_here;
        }
    }
}
