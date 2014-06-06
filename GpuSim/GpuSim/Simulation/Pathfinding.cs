using FragSharpFramework;

namespace GpuSim
{
    public partial class Pathfinding_ToPlayers : SimShader
    {
        /// <summary>
        /// Propagates the path to units of each player. Stores the result for Player 1 in .x, of Player 2 in .y, etc.
        /// Four players maximum.
        /// </summary>
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Path, Field<data> Current, Field<unit> CurData)
        {
            data data = Current[Here];
            unit cur_data = CurData[Here];

            vec4
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            vec4 output = min(right, up, left, down) + vec(_1, _1, _1, _1);

            if (Something(data))
            {
                output += 3 * vec(_1, _1, _1, _1);

                if (cur_data.team == Team.One) output.r = _0;
                if (cur_data.team == Team.Two) output.g = _0;
                if (cur_data.team == Team.Three) output.b = _0;
                if (cur_data.team == Team.Four) output.a = _0;
            }

            return output;
        }
    }

    public partial class Pathfinding_ToOtherTeams : SimShader
    {
        /// <summary>
        /// Propagates the path to enemies of each team. Stores the result for Enemies of Team 1 in .x, of Team 2 in .y, etc.
        /// Four teams maximum.
        /// </summary>
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Path, Field<data> Current, Field<unit> CurData)
        {
            data data = Current[Here];
            unit cur_data = CurData[Here];

            vec4
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            vec4 output = min(right, up, left, down) + vec(_1,_1,_1,_1);

            if (Something(data))
            {
                if (IsNeutralBuilding(cur_data))
                {
                    output += 100 * vec(_1, _1, _1, _1);
                }
                else
                {
                    output += 3 * vec(_1, _1, _1, _1);

                    if (cur_data.team != Team.One) output.r = _0;
                    if (cur_data.team != Team.Two) output.g = _0;
                    if (cur_data.team != Team.Three) output.b = _0;
                    if (cur_data.team != Team.Four) output.a = _0;
                }
            }

            return output;
        }
    }
}
