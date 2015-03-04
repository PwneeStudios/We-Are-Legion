using FragSharpFramework;

namespace Game
{
    /// <summary>
    /// Propagates the path to special units (buildings, necromancers, and dragon lords). Stores the result for Player 1 in .x, of Player 2 in .y, etc.
    /// Four players maximum.
    /// </summary>
    public partial class Pathfinding_ToSpecial : SimShader
    {
        public static readonly vec2 CenterOffset = vec(_40, _40);

        float abs_sum(vec2 v)
        {
            v = abs(v);
            return v.x + v.y;
        }

        [FragmentShader]
        BuildingDist FragmentShader(VertexOut vertex, Field<BuildingDist> Path, Field<data> Data, Field<unit> Unit)
        {
            BuildingDist output = BuildingDist.Nothing;

            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            if (Something(data_here) && (IsBuilding(unit_here) || unit_here.type == UnitType.DragonLord || unit_here.type == UnitType.Necromancer))
            {
                float type = unit_here.type;
                if (type == UnitType.DragonLord)  type = UnitType.DragonLordIcon;
                if (type == UnitType.Necromancer) type = UnitType.NecromancerIcon;

                set_type(ref output, type);
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
                if (left.dist  < min_dist) { output.player_and_type = left .player_and_type; min_dist = left.dist;  output.diff = left.diff  - vec(_1, _0); }
                if (down.dist  < min_dist) { output.player_and_type = down .player_and_type; min_dist = down.dist;  output.diff = down.diff  - vec(_0, _1); }
                if (right.dist < min_dist) { output.player_and_type = right.player_and_type; min_dist = right.dist; output.diff = right.diff + vec(_1, _0); }
                if (up.dist    < min_dist) { output.player_and_type = up   .player_and_type; min_dist = up.dist;    output.diff = up.diff    + vec(_0, _1); }

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
        PlayerTuple FragmentShader(VertexOut vertex, Field<PlayerTuple> Path, Field<data> Data, Field<unit> Units)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            PlayerTuple
                right = Path[RightOne],
                up    = Path[UpOne],
                left  = Path[LeftOne],
                down  = Path[DownOne];

            PlayerTuple distance_to = min(right, up, left, down) + vec(_1, _1, _1, _1);

            if (Something(data_here)) SetPlayerVal(ref distance_to, unit_here.player, _0);

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
                if (BlockingTileHere(unit_here) || unit_here.player == Player.None)
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
