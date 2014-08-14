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

    public partial class Geodesic_StorePos : SimShader
    {
        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<geo> Geo)
        {
            geo here = Geo[Here];

            if (here.dir == _0) return here;

            vec2 pos = vertex.TexCoords * Geo.Size;
            set_geo_pos_id(ref here, pos);

            return here;
        }
    }

    public partial class Geodesic_ExtremityPropagation : SimShader
    {
        float flatten(vec2 pos)
        {
            return pos.x + 4096 * pos.y;
        }

        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<geo> Geo)
        {
            geo
                here       = Geo[Here],
                right      = Geo[RightOne],
                up         = Geo[UpOne],
                left       = Geo[LeftOne],
                down       = Geo[DownOne],
                up_right   = Geo[UpRight],
                up_left    = Geo[UpLeft],
                down_right = Geo[DownRight],
                down_left  = Geo[DownLeft];

            if (here.dir == _0) return here;

            vec2
                extr_here = geo_pos_id(here),
                extr_right = geo_pos_id(right),
                extr_up = geo_pos_id(up),
                extr_left = geo_pos_id(left),
                extr_down = geo_pos_id(down),
                extr_up_right = geo_pos_id(up_right),
                extr_up_left = geo_pos_id(up_left),
                extr_down_right = geo_pos_id(down_right),
                extr_down_left = geo_pos_id(down_left);

            float
                val_here = flatten(extr_here),
                val_right = flatten(extr_right),
                val_up = flatten(extr_up),
                val_left = flatten(extr_left),
                val_down = flatten(extr_down),
                val_up_right = flatten(extr_up_right),
                val_up_left = flatten(extr_up_left),
                val_down_right = flatten(extr_down_right),
                val_down_left = flatten(extr_down_left);

            if (val_here < val_right)      { here.pos_storage = right     .pos_storage; val_here = val_right; }
            if (val_here < val_up)         { here.pos_storage = up        .pos_storage; val_here = val_up; }
            if (val_here < val_left)       { here.pos_storage = left      .pos_storage; val_here = val_left; }
            if (val_here < val_down)       { here.pos_storage = down      .pos_storage; val_here = val_down; }
            if (val_here < val_up_right)   { here.pos_storage = up_right  .pos_storage; val_here = val_up_right; }
            if (val_here < val_up_left)    { here.pos_storage = up_left   .pos_storage; val_here = val_up_left; }
            if (val_here < val_down_right) { here.pos_storage = down_right.pos_storage; val_here = val_down_right; }
            if (val_here < val_down_left)  { here.pos_storage = down_left .pos_storage; val_here = val_down_left; }

            return here;
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
                output.geo_id = ReducedGeoId(geo_pos_id(geo_here));

                vec2 pos_here = vertex.TexCoords * Tiles.Size;

                if (dir == Dir.Right || dir == Dir.Left) set_pos(ref output, pos_here.x);
                if (dir == Dir.Up    || dir == Dir.Down) set_pos(ref output, pos_here.y);
            }

            else if (ValidDirward(forward)      ) output = forward;
            else if (ValidDirward(forward_right)) output = forward_right;
            else if (ValidDirward(forward_left) ) output = forward_left;
            else if (ValidDirward(right)        ) output = right;
            else if (ValidDirward(left)         ) output = left;

            return output;
        }
    }
}
