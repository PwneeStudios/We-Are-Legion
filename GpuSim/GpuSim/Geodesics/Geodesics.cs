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
}
