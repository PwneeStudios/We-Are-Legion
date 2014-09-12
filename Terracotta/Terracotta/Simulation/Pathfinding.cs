using FragSharpFramework;

namespace GpuSim
{
    /// <summary>
    /// Propagates the path to units of each player. Stores the result for Player 1 in .x, of Player 2 in .y, etc.
    /// Four players maximum.
    /// </summary>
    public partial class Pathfinding_ToBuildings : SimShader
    {
        public static readonly vec2 CenterOffset = vec(_40, _40);

        float abs_sum(vec2 v)
        {
            v = abs(v);
            return v.x + v.y;
        }

        [FragmentShader]
        BuildingDist FragmentShader(VertexOut vertex, Field<BuildingDist> Path, Field<data> Current, Field<unit> CurData)
        {
            BuildingDist output = BuildingDist.Nothing;

            data data_here = Current[Here];
            unit unit_here = CurData[Here];

            if (Something(data_here) && IsBuilding(unit_here))
            {
                set_type(ref output, unit_here.type);
                set_player(ref output, unit_here.player);
                output.diff = CenterOffset;
                output.dist = _0;
            }
            else
            {
                BuildingDist
                    right = Path[RightOne],
                    up    = Path[UpOne],
                    left  = Path[LeftOne],
                    down  = Path[DownOne];
                
                float min_dist = _255;
                if (left.dist  < min_dist) { set_player(ref output, get_player(left));  min_dist = left.dist;  output.diff = left.diff  - vec(_1, _0); }
                if (down.dist  < min_dist) { set_player(ref output, get_player(down));  min_dist = down.dist;  output.diff = down.diff  - vec(_0, _1); }
                if (right.dist < min_dist) { set_player(ref output, get_player(right)); min_dist = right.dist; output.diff = right.diff + vec(_1, _0); }
                if (up.dist    < min_dist) { set_player(ref output, get_player(up));    min_dist = up.dist;    output.diff = up.diff    + vec(_0, _1); }

                output.dist = min_dist + _1;
            }

            return output;
        }
    }


    /// <summary>
    /// Propagates the path to units of each player. Stores the result for Player 1 in .x, of Player 2 in .y, etc.
    /// Four players maximum.
    /// </summary>
    public partial class Pathfinding_ToPlayers : SimShader
    {
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

    /// <summary>
    /// Propagates the path to enemies of each team. Stores the result for Enemies of Team 1 in .x, of Team 2 in .y, etc.
    /// Four teams maximum.
    /// </summary>
    public partial class Pathfinding_ToOtherTeams : SimShader
    {
        [FragmentShader]
        TeamTuple FragmentShader(VertexOut vertex, Field<TeamTuple> Path, Field<data> Data, Field<unit> Units)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            TeamTuple
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            TeamTuple dist_to_enemy_of = min(right, up, left, down) + vec(_1,_1,_1,_1);

            if (Something(data_here))
            {
                if (IsNeutralBuilding(unit_here) || BlockingTileHere(unit_here))
                {
                    dist_to_enemy_of += 100 * TeamTuple(_1, _1, _1, _1);
                }
                else
                {
                    dist_to_enemy_of += 3 * TeamTuple(_1, _1, _1, _1);

                    if (unit_here.team != Team.One)   dist_to_enemy_of.TeamOne   = _0;
                    if (unit_here.team != Team.Two)   dist_to_enemy_of.TeamTwo   = _0;
                    if (unit_here.team != Team.Three) dist_to_enemy_of.TeamThree = _0;
                    if (unit_here.team != Team.Four)  dist_to_enemy_of.TeamFour  = _0;
                }
            }

            return dist_to_enemy_of;
        }
    }
}
