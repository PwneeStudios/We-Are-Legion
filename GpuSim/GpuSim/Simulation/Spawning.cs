using FragSharpFramework;

namespace GpuSim
{
    public partial class SpawnUnits_Data : SimShader
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
                    cur_data.action = UnitAction.Attacking;
                    cur_data.change = Change.Stayed;
                    set_selected(ref cur_data, false);
                    set_prior_direction(ref cur_data, cur_data.direction);
                }
            }

            return cur_data;
        }
    }

    public partial class SpawnUnits_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> CurrentData, Field<data> PreviousData)
        {
            data
                cur_data = CurrentData[Here],
                prev_data = PreviousData[Here];

            unit unit_here = Unit[Here];

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
                    unit_here.player = unit_left.player;
                    unit_here.team   = unit_left.team;
                    unit_here.type   = UnitType.Footman;
                    unit_here.anim   = Anim.None;
                }
            }

            return unit_here;
        }
    }
}
