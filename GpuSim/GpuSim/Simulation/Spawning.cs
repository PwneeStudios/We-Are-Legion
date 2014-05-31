using FragSharpFramework;

namespace GpuSim
{
    public partial class SpawnUnits : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> CurrentData, Field<data> PreviousData)
        {
            data
                cur_data = CurrentData[Here],
                prev_data = PreviousData[Here];

            if (!Something(cur_data) && !Something(prev_data))
            {
                unit
                    unit_right = Unit[RightOne],
                    unit_up = Unit[UpOne],
                    unit_left = Unit[LeftOne],
                    unit_down = Unit[DownOne];

                data
                    data_right = PreviousData[RightOne],
                    data_up = PreviousData[UpOne],
                    data_left = PreviousData[LeftOne],
                    data_down = PreviousData[DownOne];

                if (unit_left.type == UnitType.Barracks)
                {
                    cur_data.direction = Dir.Right;
                    cur_data.action = UnitAction.Spawning;
                    cur_data.change = Change.Stayed;
                    set_selected(ref cur_data, false);
                    set_prior_direction(ref cur_data, cur_data.direction);
                }
            }

            return cur_data;
        }
    }

    public partial class SetSpawn_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            if (Something(data_here) && data_here.action == UnitAction.Spawning)
            {
                unit barracks = Unit[dir_to_vec(Reverse(data_here.direction))];
                unit_here.player = barracks.player;
                unit_here.team   = barracks.team;
                unit_here.type   = UnitType.Footman;
                unit_here.anim   = Anim.None;
            }

            return unit_here;
        }
    }

    public partial class SetSpawn_Target : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Target, Field<data> Data)
        {
            data data_here = Data[Here];
            vec4 target = Target[Here];

            if (Something(data_here) && data_here.action == UnitAction.Spawning)
            {
                target = Target[dir_to_vec(Reverse(data_here.direction))];
                //target.x = _4;
                //target.z = _4;
            }

            return target;
        }
    }

    public partial class SetSpawn_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            if (Something(data_here) && data_here.action == UnitAction.Spawning)
            {
                data_here.action = UnitAction.Attacking;
            }

            return data_here;
        }
    }
}
