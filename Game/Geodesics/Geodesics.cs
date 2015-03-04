using FragSharpFramework;

namespace Game
{
    public partial class Geodesic_Outline : SimShader
    {
        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<tile> Tiles, [Vals.Bool] bool Anti)
        {
            tile
                here       = Tiles[Here],
                right      = Tiles[RightOne],
                up         = Tiles[UpOne],
                left       = Tiles[LeftOne],
                down       = Tiles[DownOne],
                up_right   = Tiles[UpRight],
                up_left    = Tiles[UpLeft],
                down_right = Tiles[DownRight],
                down_left  = Tiles[DownLeft];

            if (IsBlockingTile(here)) return geo.Nothing;

            float dir = 0;

            if (IsBlockingTile(up_left))    dir = Anti ? Dir.Left  : Dir.Up;
            if (IsBlockingTile(up_right))   dir = Anti ? Dir.Up    : Dir.Right;
            if (IsBlockingTile(down_right)) dir = Anti ? Dir.Right : Dir.Down;
            if (IsBlockingTile(down_left))  dir = Anti ? Dir.Down  : Dir.Left;

            if (Anti)
            {
                if (IsBlockingTile(right))
                {
                    dir = Dir.Up;
                    if (IsBlockingTile(up))
                    {
                        dir = Dir.Left;
                        if (IsBlockingTile(left))
                            dir = Dir.Down;
                    }
                }

                if (IsBlockingTile(up))
                {
                    dir = Dir.Left;
                    if (IsBlockingTile(left))
                    {
                        dir = Dir.Down;
                        if (IsBlockingTile(down))
                            dir = Dir.Right;
                    }
                }

                if (IsBlockingTile(left))
                {
                    dir = Dir.Down;
                    if (IsBlockingTile(down))
                    {
                        dir = Dir.Right;
                        if (IsBlockingTile(right))
                            dir = Dir.Up;
                    }
                }

                if (IsBlockingTile(down))
                {
                    dir = Dir.Right;
                    if (IsBlockingTile(right))
                    {
                        dir = Dir.Up;
                        if (IsBlockingTile(up))
                            dir = Dir.Left;
                    }
                }
            }
            else
            {
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
        geo FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<geo> Geo, [Vals.Bool] bool Anti)
        {
            tile
                here       = Tiles[Here],
                right      = Tiles[RightOne],
                up         = Tiles[UpOne],
                left       = Tiles[LeftOne],
                down       = Tiles[DownOne],
                up_right   = Tiles[UpRight],
                up_left    = Tiles[UpLeft],
                down_right = Tiles[DownRight],
                down_left  = Tiles[DownLeft];

            geo
                geo_here       = Geo[Here],
                geo_right      = Geo[RightOne],
                geo_up         = Geo[UpOne],
                geo_left       = Geo[LeftOne],
                geo_down       = Geo[DownOne],
                geo_up_right   = Geo[UpRight],
                geo_up_left    = Geo[UpLeft],
                geo_down_right = Geo[DownRight],
                geo_down_left  = Geo[DownLeft];

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
                (IsBlockingTile(up)    ? 1 : 0) +
                (IsBlockingTile(left)  ? 1 : 0) +
                (IsBlockingTile(down)  ? 1 : 0) +
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
                extr_here       = geo_pos_id(here),
                extr_right      = geo_pos_id(right),
                extr_up         = geo_pos_id(up),
                extr_left       = geo_pos_id(left),
                extr_down       = geo_pos_id(down),
                extr_up_right   = geo_pos_id(up_right),
                extr_up_left    = geo_pos_id(up_left),
                extr_down_right = geo_pos_id(down_right),
                extr_down_left  = geo_pos_id(down_left);

            float
                val_here       = flatten(extr_here),
                val_right      = flatten(extr_right),
                val_up         = flatten(extr_up),
                val_left       = flatten(extr_left),
                val_down       = flatten(extr_down),
                val_up_right   = flatten(extr_up_right),
                val_up_left    = flatten(extr_up_left),
                val_down_right = flatten(extr_down_right),
                val_down_left  = flatten(extr_down_left);

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

    public partial class Geodesic_SetGeoId : SimShader
    {
        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<geo> Geo)
        {
            geo geo_here = Geo[Here];

            geo_here.geo_id = ReducedGeoId(geo_pos_id(geo_here));
            geo_here.dist = _0;

            return geo_here;
        }
    }

    public partial class Geodesic_PolarDistance : SimShader
    {
        [FragmentShader]
        geo_info FragmentShader(VertexOut vertex, Field<geo> Geo, Field<geo_info> Info)
        {
            geo
                here       = Geo[Here],
                right      = Geo[RightOne],
                up         = Geo[UpOne],
                left       = Geo[LeftOne],
                down       = Geo[DownOne];

            float
                dist_right      = polar_dist(Info[RightOne]),
                dist_up         = polar_dist(Info[UpOne]),
                dist_left       = polar_dist(Info[LeftOne]),
                dist_down       = polar_dist(Info[DownOne]);

            if (here.dir == _0) return geo_info.Zero;

            float dist = 0;

            // Calculate the geo_id of this cell
            geo temp_geo = geo.Nothing;
            vec2 pos = vertex.TexCoords * Geo.Size;
            set_geo_pos_id(ref temp_geo, pos);

            // ... if that geo_id matches the id of the geo info here, then this is the "master" or "12 o' clock" cell of the geodesic line going through this cell.
            if (here.pos_storage == temp_geo.pos_storage)
            {
                // That means its polar distance is 0 by definition.
                dist = 0;
            }
            else
            {
                // If this geodesic flows into another tile, then the polar distance here should be 1 less than the distance of the tile it flows into.
                // This is a fail safe condition that covers degenerate tiles which have nothing flowing into them and only flow out.
                if (here.dir == Dir.Left)  dist = max(_0, dist_left  - 1);
                if (here.dir == Dir.Right) dist = max(_0, dist_right - 1);
                if (here.dir == Dir.Up)    dist = max(_0, dist_up    - 1);
                if (here.dir == Dir.Down)  dist = max(_0, dist_down  - 1);

                // The polar distance is also 1 plus the polar distance of whatever cell comes "before" it (by following the geo backwards "counterclockwise").
                if (right.dir == Dir.Left  && dist_right >= dist) dist = dist_right + 1;
                if (left.dir  == Dir.Right && dist_left  >= dist) dist = dist_left  + 1;
                if (up.dir    == Dir.Down  && dist_up    >= dist) dist = dist_up    + 1;
                if (down.dir  == Dir.Up    && dist_down  >= dist) dist = dist_down  + 1;
            }

            // Pack the polar distance into 2-bytes and return it in
            geo_info output = geo_info.Zero;
            set_polar_dist(ref output, dist);
            
            return output;
        }
    }

    public partial class Geodesic_SetCircumference : SimShader
    {
        [FragmentShader]
        geo_info FragmentShader(VertexOut vertex, Field<geo> Geo, Field<geo_info> Info)
        {
            geo_info info_here = Info[Here];
            geo here           = Geo[Here];

            if (here.dir == _0) return geo_info.Zero;

            vec2 pos_here = vertex.TexCoords * Geo.Size;
            vec2 start_pos = geo_pos_id(here);
            RelativeIndex GeoStart = (RelativeIndex)(start_pos - pos_here);

            geo
                right      = Geo[GeoStart + RightOne],
                up         = Geo[GeoStart + UpOne],
                left       = Geo[GeoStart + LeftOne],
                down       = Geo[GeoStart + DownOne];

            float circum = 0;
            if (right.pos_storage == here.pos_storage) circum = max(circum, polar_dist(Info[GeoStart + RightOne]));
            if (up   .pos_storage == here.pos_storage) circum = max(circum, polar_dist(Info[GeoStart + UpOne]));
            if (left .pos_storage == here.pos_storage) circum = max(circum, polar_dist(Info[GeoStart + LeftOne]));
            if (down .pos_storage == here.pos_storage) circum = max(circum, polar_dist(Info[GeoStart + DownOne]));

            // Pack the polar circumference into 2-bytes
            set_circumference(ref info_here, circum);

            return info_here;
        }
    }

    public partial class Geodesic_Polarity : SimShader
    {
        [FragmentShader]
        dirward FragmentShader(VertexOut vertex, Field<dirward> Dirward, Field<geo> Geo, Field<geo> ShiftedGeo, Field<geo_info> Info, Field<geo_info> ShiftedInfo, [Dir.Vals] float dir)
        {
            geo
                geo_here  =        Geo[Here],
                geo_shift = ShiftedGeo[Here];

            if (geo_here.dir == _0) return dirward.Nothing;

            geo_info
                info_here  =        Info[Here],
                info_shift = ShiftedInfo[Here];

            if (geo_here.pos_storage != geo_shift.pos_storage) return Dirward[Here];

            float
                dist_here  = unpack_val(info_here.xy),
                dist_shift = unpack_val(info_shift.xy),
                circum     = unpack_val(info_here.zw);

            float diff = dist_here - dist_shift;

            float clockwise = 0, counterclockwise = 0;
            if (diff > 0)
            {
                clockwise = diff;
                counterclockwise = circum - diff;
            }
            else
            {
                clockwise = circum + diff;
                counterclockwise = -diff;
            }            

            dirward output = dirward.Nothing;
            output.polarity = clockwise > counterclockwise ? 0 : 1;
            output.polarity_set = _true;

            return output;
        }
    }

    public partial class Geodesic_FillMissingPolarity : SimShader
    {
        [FragmentShader]
        dirward FragmentShader(VertexOut vertex, Field<dirward> Dirward, Field<geo> Geo)
        {
            geo
                here  = Geo[Here],
                right = Geo[RightOne],
                up    = Geo[UpOne],
                left  = Geo[LeftOne],
                down  = Geo[DownOne];

            dirward
                dirward_here  = Dirward[Here],
                dirward_right = Dirward[RightOne],
                dirward_up    = Dirward[UpOne],
                dirward_left  = Dirward[LeftOne],
                dirward_down  = Dirward[DownOne];

            if (here.dir == _0) return dirward.Nothing;

            if (dirward_here.polarity_set == _false)
            {
                if (right.pos_storage == here.pos_storage && dirward_right.polarity_set == _true) { dirward_here.polarity = dirward_right.polarity; dirward_here.polarity_set = _true; }
                if (left.pos_storage  == here.pos_storage && dirward_left.polarity_set  == _true) { dirward_here.polarity = dirward_left.polarity;  dirward_here.polarity_set = _true; }
                if (up.pos_storage    == here.pos_storage && dirward_up.polarity_set    == _true) { dirward_here.polarity = dirward_up.polarity;    dirward_here.polarity_set = _true; }
                if (down.pos_storage  == here.pos_storage && dirward_down.polarity_set  == _true) { dirward_here.polarity = dirward_down.polarity;  dirward_here.polarity_set = _true; }
            }

            return dirward_here;
        }
    }

    public partial class Geodesic_ClearImportance : SimShader
    {
        [FragmentShader]
        dirward FragmentShader(VertexOut vertex, Field<dirward> Dirward)
        {
            dirward dirward_here = Dirward[Here];

            dirward_here.importance = _0;

            return dirward_here;
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
                geo_here       = Geo[Here],
                geo_right      = Geo[RightOne],
                geo_up         = Geo[UpOne],
                geo_left       = Geo[LeftOne],
                geo_down       = Geo[DownOne],
                geo_up_right   = Geo[UpRight],
                geo_up_left    = Geo[UpLeft],
                geo_down_right = Geo[DownRight],
                geo_down_left  = Geo[DownLeft];

            dirward
                dirward_here       = Dirward[Here],
                dirward_right      = Dirward[RightOne],
                dirward_up         = Dirward[UpOne],
                dirward_left       = Dirward[LeftOne],
                dirward_down       = Dirward[DownOne],
                dirward_up_right   = Dirward[UpRight],
                dirward_up_left    = Dirward[UpLeft],
                dirward_down_right = Dirward[DownRight],
                dirward_down_left  = Dirward[DownLeft];

            if (IsBlockingTile(here)) return dirward.Nothing;

            dirward output = dirward.Nothing;

            dirward forward = dirward.Nothing, forward_right = dirward.Nothing, forward_left = dirward.Nothing, rightward = dirward.Nothing, leftward = dirward.Nothing;
            geo geo_forward = geo.Nothing, geo_forward_right = geo.Nothing, geo_forward_left = geo.Nothing, geo_rightward = geo.Nothing, geo_leftward = geo.Nothing;

            // Get the surrounding dirward info and store it relative to the direction we consider forward
            if (dir == Dir.Up)
            {
                forward       = dirward_up;
                forward_right = dirward_up_right;
                forward_left  = dirward_up_left;
                rightward     = dirward_right;
                leftward      = dirward_left;

                geo_forward       = geo_up;
                geo_forward_right = geo_up_right;
                geo_forward_left  = geo_up_left;
                geo_rightward     = geo_right;
                geo_leftward      = geo_left;
            }
            else if (dir == Dir.Right)
            {
                forward       = dirward_right;
                forward_right = dirward_down_right;
                forward_left  = dirward_up_right;
                rightward     = dirward_down;
                leftward      = dirward_up;

                geo_forward       = geo_right;
                geo_forward_right = geo_down_right;
                geo_forward_left  = geo_up_right;
                geo_rightward     = geo_down;
                geo_leftward      = geo_up;
            }
            else if (dir == Dir.Down)
            {
                forward       = dirward_down;
                forward_right = dirward_down_left;
                forward_left  = dirward_down_right;
                rightward     = dirward_left;
                leftward      = dirward_right;

                geo_forward       = geo_down;
                geo_forward_right = geo_down_left;
                geo_forward_left  = geo_down_right;
                geo_rightward     = geo_left;
                geo_leftward      = geo_right;
            }
            else if (dir == Dir.Left)
            {
                forward       = dirward_left;
                forward_right = dirward_up_left;
                forward_left  = dirward_down_left;
                rightward     = dirward_up;
                leftward      = dirward_down;

                geo_forward       = geo_left;
                geo_forward_right = geo_up_left;
                geo_forward_left  = geo_down_left;
                geo_rightward     = geo_up;
                geo_leftward      = geo_down;
            }

            if (geo_here.dir > 0 && IsBlockingTile(Tiles[dir_to_vec(dir)]))
            {
                output = dirward_here;

                output.geo_id = geo_here.geo_id;
                output.dist_to_wall = _0;
            }

            else if (ValidDirward(forward)       && forward.geo_id       == geo_forward.geo_id)       { output = forward;       output.dist_to_wall += _1; }
            else if (ValidDirward(forward_right) && forward_right.geo_id == geo_forward_right.geo_id) { output = forward_right; output.dist_to_wall += _1; }
            else if (ValidDirward(forward_left)  && forward_left.geo_id  == geo_forward_left.geo_id)  { output = forward_left;  output.dist_to_wall += _1; }
            //else if (ValidDirward(rightward)     && rightward.geo_id     == geo_rightward.geo_id)     { output = rightward;     output.dist_to_wall += _0; }
            //else if (ValidDirward(leftward)      && leftward.geo_id      == geo_leftward.geo_id)      { output = leftward;      output.dist_to_wall += _0; }

            return output;
        }
    }

    public partial class Geodesic_ConvertToBlocking : SimShader
    {
        [FragmentShader]
        tile FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<geo> Geo)
        {
            tile tile_here = Tiles[Here];
            geo  geo_here  = Geo[Here];

            if (IsValid(geo_here.dir))
                tile_here.type = TileType.Trees;

            return tile_here;
        }
    }

    public partial class Geodesic_Flatten : SimShader
    {
        void InheritsFrom(ref geo outer_geo, geo inner_geo)
        {
            outer_geo.dist   = inner_geo.dist + _1;
            outer_geo.geo_id = inner_geo.geo_id;            
        }

        [FragmentShader]
        geo FragmentShader(VertexOut vertex, Field<geo> Geo, Field<geo> OuterGeo)
        {
            geo
                geo_here       = Geo[Here],
                geo_right      = Geo[RightOne],
                geo_up         = Geo[UpOne],
                geo_left       = Geo[LeftOne],
                geo_down       = Geo[DownOne],
                geo_up_right   = Geo[UpRight],
                geo_up_left    = Geo[UpLeft],
                geo_down_right = Geo[DownRight],
                geo_down_left  = Geo[DownLeft];

            geo
                outer_geo_here = OuterGeo[Here];

            if (IsValid(geo_here.dir)) return geo_here;

            outer_geo_here.dist = _255; // Start off as maximum possible 1-byte distance, since we will be taking the min of surrounding distances (and adding _1)
            if      (outer_geo_here.dist > geo_right.dist      && IsValid(geo_right.dir)     ) InheritsFrom(ref outer_geo_here, geo_right);
            else if (outer_geo_here.dist > geo_up.dist         && IsValid(geo_up.dir)        ) InheritsFrom(ref outer_geo_here, geo_up);
            else if (outer_geo_here.dist > geo_left.dist       && IsValid(geo_left.dir)      ) InheritsFrom(ref outer_geo_here, geo_left);
            else if (outer_geo_here.dist > geo_down.dist       && IsValid(geo_down.dir)      ) InheritsFrom(ref outer_geo_here, geo_down);
            else if (outer_geo_here.dist > geo_up_right.dist   && IsValid(geo_up_right.dir)  ) InheritsFrom(ref outer_geo_here, geo_up_right);
            else if (outer_geo_here.dist > geo_up_left.dist    && IsValid(geo_up_left.dir)   ) InheritsFrom(ref outer_geo_here, geo_up_left);
            else if (outer_geo_here.dist > geo_down_right.dist && IsValid(geo_down_right.dir)) InheritsFrom(ref outer_geo_here, geo_down_right);
            else if (outer_geo_here.dist > geo_down_left.dist  && IsValid(geo_down_left.dir) ) InheritsFrom(ref outer_geo_here, geo_down_left);

            return outer_geo_here;
        }
    }

    public partial class Geodesic_Boundary : SimShader
    {
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

            if (!IsValid(here.dir)) return here;

            vec2 id_here = here.geo_id;
            if (right     .geo_id != id_here ||
                left      .geo_id != id_here ||
                up        .geo_id != id_here ||
                down      .geo_id != id_here ||
                up_right  .geo_id != id_here ||
                up_left   .geo_id != id_here ||
                down_right.geo_id != id_here ||
                down_left .geo_id != id_here)
            {
                //here.dist = 1;
                return geo.Nothing;
            }

            return here;
        }
    }    
}
