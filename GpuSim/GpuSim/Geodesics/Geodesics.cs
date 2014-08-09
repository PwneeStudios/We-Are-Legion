using FragSharpFramework;

namespace GpuSim
{
    public partial class Geodesic_Outline : SimShader
    {
        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<tile> Tiles)
        {
            tile
                here = Tiles[Here],
                right = Tiles[RightOne],
                up = Tiles[UpOne],
                left = Tiles[LeftOne],
                down = Tiles[DownOne],
                up_right = Tiles[UpRight],
                up_left = Tiles[UpLeft],
                down_right = Tiles[DownRight],
                down_left = Tiles[DownLeft];

            if (IsBlockingTile(here)) return geo.Nothing;

            float dir = 0;

            if (IsBlockingTile(up_left)) dir = Dir.Up;
            if (IsBlockingTile(up_right)) dir = Dir.Right;
            if (IsBlockingTile(down_right)) dir = Dir.Down;
            if (IsBlockingTile(down_left)) dir = Dir.Left;

            if (IsBlockingTile(right))
            {
                dir = Dir.Down;
                if (IsBlockingTile(down))
                {
                    dir = Dir.Left;
                    if (IsBlockingTile(left))
                        dir = Dir.Up;
                }
            }

            if (IsBlockingTile(up))
            {
                dir = Dir.Right;
                if (IsBlockingTile(right))
                {
                    dir = Dir.Down;
                    if (IsBlockingTile(down))
                        dir = Dir.Left;
                }
            }

            if (IsBlockingTile(left))
            {
                dir = Dir.Up;
                if (IsBlockingTile(up))
                {
                    dir = Dir.Right;
                    if (IsBlockingTile(right))
                        dir = Dir.Down;
                }
            }

            if (IsBlockingTile(down))
            {
                dir = Dir.Left;
                if (IsBlockingTile(left))
                {
                    dir = Dir.Up;
                    if (IsBlockingTile(up))
                        dir = Dir.Right;
                }
            }

            geo output = geo.Nothing;
            output.dir = dir;

            int surround_count =
                (IsBlockingTile(up)    ? 1 : 0) +
                (IsBlockingTile(left)  ? 1 : 0) +
                (IsBlockingTile(down)  ? 1 : 0) +
                (IsBlockingTile(right) ? 1 : 0);

            if (output.dir > _0 && surround_count == 3)
            {
                output.bad = _true;
            }

            return output;
        }
    }

    public partial class Geodesic_OutlineCleanup : SimShader
    {
        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<geo> Geo)
        {
            tile
                here = Tiles[Here],
                right = Tiles[RightOne],
                up = Tiles[UpOne],
                left = Tiles[LeftOne],
                down = Tiles[DownOne],
                up_right = Tiles[UpRight],
                up_left = Tiles[UpLeft],
                down_right = Tiles[DownRight],
                down_left = Tiles[DownLeft];

            geo
                geo_here = Geo[Here],
                geo_right = Geo[RightOne],
                geo_up = Geo[UpOne],
                geo_left = Geo[LeftOne],
                geo_down = Geo[DownOne],
                geo_up_right = Geo[UpRight],
                geo_up_left = Geo[UpLeft],
                geo_down_right = Geo[DownRight],
                geo_down_left = Geo[DownLeft];

            if (IsBlockingTile(here)) return geo.Nothing;

            geo output = geo_here;

            if (!(IsBlockingTile(right) && IsBlockingTile(left)) &&
                (geo_here.dir == Dir.Up && geo_up.dir == Dir.Down || geo_here.dir == Dir.Down && geo_down.dir == Dir.Up))
            {
                output.dir = IsBlockingTile(right) ? Dir.Left : Dir.Right;
            }

            if (!(IsBlockingTile(up) && IsBlockingTile(down)) &&
                (geo_here.dir == Dir.Right && geo_right.dir == Dir.Left || geo_here.dir == Dir.Left && geo_left.dir == Dir.Right))
            {
                output.dir = IsBlockingTile(up) ? Dir.Down : Dir.Up;
            }

            if (Geo[dir_to_vec(output.dir)].bad == _true && geo_here.bad == _false)
                output.dir = Reverse(output.dir);

            int surround_count =
                (IsBlockingTile(up) ? 1 : 0) +
                (IsBlockingTile(left) ? 1 : 0) +
                (IsBlockingTile(down) ? 1 : 0) +
                (IsBlockingTile(right) ? 1 : 0);

            float bad_count = geo_up.bad + geo_left.bad + geo_down.bad + geo_right.bad;

            if (surround_count >= 2 && bad_count >= _1 ||
                geo_up.bad == _true && geo_down.bad == _true ||
                geo_right.bad == _true && geo_left.bad == _true)
                output.bad = _true;

            return output;
        }
    }

    public partial class Geodesic_DirwardExtend : SimShader
    {
        [FragmentShader]
        dirward FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<geo> Geo, Field<dirward> Dirward, [Dir.Vals] float dir)
        {
            tile
                here = Tiles[Here];

            geo
                geo_here = Geo[Here],
                geo_right = Geo[RightOne],
                geo_up = Geo[UpOne],
                geo_left = Geo[LeftOne],
                geo_down = Geo[DownOne],
                geo_up_right = Geo[UpRight],
                geo_up_left = Geo[UpLeft],
                geo_down_right = Geo[DownRight],
                geo_down_left = Geo[DownLeft];

            dirward
                dirward_here = Dirward[Here],
                dirward_right = Dirward[RightOne],
                dirward_up = Dirward[UpOne],
                dirward_left = Dirward[LeftOne],
                dirward_down = Dirward[DownOne],
                dirward_up_right = Dirward[UpRight],
                dirward_up_left = Dirward[UpLeft],
                dirward_down_right = Dirward[DownRight],
                dirward_down_left = Dirward[DownLeft];

            if (IsBlockingTile(here)) return dirward.Nothing;

            dirward output = dirward.Nothing;

            dirward forward = dirward.Nothing, forward_right = dirward.Nothing, forward_left = dirward.Nothing, right = dirward.Nothing, left = dirward.Nothing;

            // Get the surrounding dirward info and store it relative to the direction we consider forward
            if (dir == Dir.Up)
            {
                forward       = dirward_up;
                forward_right = dirward_up_right;
                forward_left  = dirward_up_left;
                right         = dirward_right;
                left          = dirward_left;
            }
            else if (dir == Dir.Right)
            {
                forward       = dirward_right;
                forward_right = dirward_down_right;
                forward_left  = dirward_up_right;
                right         = dirward_down;
                left          = dirward_up;
            }
            else if (dir == Dir.Down)
            {
                forward       = dirward_down;
                forward_right = dirward_down_left;
                forward_left  = dirward_down_right;
                right         = dirward_left;
                left          = dirward_right;
            }
            else if (dir == Dir.Left)
            {
                forward       = dirward_left;
                forward_right = dirward_up_left;
                forward_left  = dirward_down_left;
                right         = dirward_up;
                left          = dirward_down;
            }

            if (geo_here.dir > 0 && IsBlockingTile(Tiles[dir_to_vec(dir)]))
            {
                output.dir = geo_here.dir;
                output.dist = _0;

                vec2 pos = vertex.TexCoords * Tiles.Size;
                
                if (dir == Dir.Right || dir == Dir.Left) set_pos(ref output, pos.x);
                if (dir == Dir.Up    || dir == Dir.Down) set_pos(ref output, pos.y);
            }

            else if (forward.dir       > 0) output = forward;
            else if (forward_right.dir > 0) output = forward_right;
            else if (forward_left.dir  > 0) output = forward_left;
            else if (right.dir         > 0) output = right;
            else if (left.dir          > 0) output = left;

            output.dist += _1;

            return output;
        }
    }
}
