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
        PlayerTuple FragmentShader(VertexOut vertex, Field<PlayerTuple> Path, Field<data> Current, Field<unit> CurData)
        {
            data data = Current[Here];
            unit cur_data = CurData[Here];

            PlayerTuple
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            PlayerTuple distance_to = min(right, up, left, down) + vec(_1, _1, _1, _1);

            if (Something(data))
            {
                distance_to += 3 * PlayerTuple(_1, _1, _1, _1);

                if (cur_data.player == Team.One)   distance_to.PlayerOne   = _0;
                if (cur_data.player == Team.Two)   distance_to.PlayerTwo   = _0;
                if (cur_data.player == Team.Three) distance_to.PlayerThree = _0;
                if (cur_data.player == Team.Four)  distance_to.PlayerFour  = _0;
            }

            return distance_to;
        }
    }

    public partial class Pathfinding_ToOtherTeams : SimShader
    {
        /// <summary>
        /// Propagates the path to enemies of each team. Stores the result for Enemies of Team 1 in .x, of Team 2 in .y, etc.
        /// Four teams maximum.
        /// </summary>
        [FragmentShader]
        TeamTuple FragmentShader(VertexOut vertex, Field<TeamTuple> Path, Field<data> Current, Field<unit> CurData)
        {
            data data = Current[Here];
            unit cur_data = CurData[Here];

            TeamTuple
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            TeamTuple dist_to_enemy_of = min(right, up, left, down) + vec(_1,_1,_1,_1);

            if (Something(data))
            {
                if (IsNeutralBuilding(cur_data))
                {
                    dist_to_enemy_of += 100 * TeamTuple(_1, _1, _1, _1);
                }
                else
                {
                    dist_to_enemy_of += 3 * TeamTuple(_1, _1, _1, _1);

                    if (cur_data.team != Team.One)   dist_to_enemy_of.TeamOne   = _0;
                    if (cur_data.team != Team.Two)   dist_to_enemy_of.TeamTwo   = _0;
                    if (cur_data.team != Team.Three) dist_to_enemy_of.TeamThree = _0;
                    if (cur_data.team != Team.Four)  dist_to_enemy_of.TeamFour  = _0;
                }
            }

            return dist_to_enemy_of;
        }
    }
}
