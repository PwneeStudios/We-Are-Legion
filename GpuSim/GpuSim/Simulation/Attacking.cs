using FragSharpFramework;

namespace GpuSim
{
    public partial class CheckForAttacking : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Unit, Field<data> Data)
        {
            unit unit_here = Unit[Here];
            data data_here = Data[Here];

            unit_here.anim = _0;

            if (data_here.action == UnitAction.Attacking)
            {
                unit facing = Unit[dir_to_vec(data_here.direction)];

                if (facing.team != unit_here.team && facing.team != Team.None)
                {
                    unit_here.anim = _5;
                }
            }

            return unit_here;
        }
    }
}
