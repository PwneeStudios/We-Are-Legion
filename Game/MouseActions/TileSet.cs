using FragSharpFramework;

namespace Game
{
    public partial class Action_PaintTiles : SimShader
    {
        [FragmentShader]
        tile FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<data> Select, Field<vec4> Random, [TileType.Vals] float type)
        {
            tile here = Tiles[Here];
            data select = Select[Here];
            vec4 rndv = Random[Here];

            float rnd = rndv.x * rndv.x * rndv.x * rndv.x;

            if (Something(select))
            {
                here.type = type;

                if (type == TileType.Grass) { here.i = RndFint(rnd, _0, _6); here.j = _31; }
                else if (type == TileType.Dirt) { here.i = RndFint(rnd, _0, _9); here.j = _30; }
                else if (type == TileType.Trees) { here.i = _0; here.j = _25; }
            }

            return here;
        }
    }

    public partial class PaintTiles_UpdateData : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<unit> Units, Field<data> Data)
        {
            tile tile_here = Tiles[Here];
            unit unit_here = Units[Here];
            data data_here = Data[Here];

            if (IsBlockingTile(tile_here))
            {
                data_here = data.Nothing;
                data_here.direction = Dir.Stationary;
            }
            else
            {
                if (BlockingTileHere(unit_here))
                {
                    data_here = data.Nothing;
                }
            }

            return data_here;
        }
    }

    public partial class PaintTiles_UpdateUnits : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<unit> Units)
        {
            tile tile_here = Tiles[Here];
            unit unit_here = Units[Here];

            if (IsBlockingTile(tile_here))
            {
                unit_here.type = UnitType.BlockingTile;
                unit_here.player = Player.None;
                unit_here.team = Team.None;
            }
            else
            {
                if (BlockingTileHere(unit_here))
                {
                    unit_here.type = UnitType.None;
                    unit_here.player = Player.None;
                    unit_here.team = Team.None;
                }
            }

            return unit_here;
        }
    }

    public partial class PaintTiles_UpdateTiles : SimShader
    {
        [FragmentShader]
        tile FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<data> Select)
        {
            tile here = Tiles[Here];
            data select = Select[Here];

            tile
                right      = Tiles[RightOne],
                up         = Tiles[UpOne],
                left       = Tiles[LeftOne],
                down       = Tiles[DownOne],
                up_right   = Tiles[UpRight],
                up_left    = Tiles[UpLeft],
                down_right = Tiles[DownRight],
                down_left  = Tiles[DownLeft];

            if (here.type == TileType.Dirt)
            {
                DirtGrassInterface(ref here, ref right, ref up, ref left, ref down, ref up_right, ref up_left, ref down_right, ref down_left);
            }
            else if (here.type == TileType.Grass || here.type == TileType.Trees)
            {
                GrassTreeInterface(ref here, ref right, ref up, ref left, ref down, ref up_right, ref up_left, ref down_right, ref down_left);
            }

            return here;
        }

        void DirtGrassInterface(ref tile here, ref tile right, ref tile up, ref tile left, ref tile down, ref tile up_right, ref tile up_left, ref tile down_right, ref tile down_left)
        {
            bool grass_on_left = left.type == TileType.Grass || up_left.type == TileType.Grass || down_left.type == TileType.Grass;
            bool grass_on_right = right.type == TileType.Grass || up_right.type == TileType.Grass || down_right.type == TileType.Grass;
            bool grass_on_top = up_left.type == TileType.Grass || up.type == TileType.Grass || up_right.type == TileType.Grass;
            bool grass_on_bottom = down_left.type == TileType.Grass || down.type == TileType.Grass || down_right.type == TileType.Grass;

            // If we're straddled on two opposite sides by grass, or if any tile adjacent to us is trees, then just turn into grass
            if (left.type == TileType.Grass && right.type == TileType.Grass ||
                up.type == TileType.Grass && down.type == TileType.Grass ||
                up.type == TileType.Trees || right.type == TileType.Trees || down.type == TileType.Trees || left.type == TileType.Trees)
            {
                here.type = TileType.Grass;
                here.i = _0;
                here.j = _31;
            }

            // If two adjacent sides are grass, then use a 2-grass corner piece
            else if (left.type == TileType.Grass && up.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _2;
                here.j = _26;
            }

            else if (up.type == TileType.Grass && right.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _0;
                here.j = _26;
            }

            else if (right.type == TileType.Grass && down.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _4;
                here.j = _26;
            }

            else if (down.type == TileType.Grass && left.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _6;
                here.j = _26;
            }

            else
            if (grass_on_left && right.type == TileType.Grass ||
                grass_on_right && left.type == TileType.Grass ||
                grass_on_top && down.type == TileType.Grass ||
                grass_on_bottom && up.type == TileType.Grass)
            {
                here.type = TileType.Grass;
                here.i = _0;
                here.j = _31;
            }

            // If a single side is grass, then use a 1-grass side piece
            else if (right.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _0;
                here.j = _28;
            }

            else if (up.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _3;
                here.j = _28;
            }

            else if (left.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _6;
                here.j = _28;
            }

            else if (down.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _9;
                here.j = _28;
            }

            // If a single diagonal corner is grass, then use a 1-grass diagonal corner
            else if (down_left.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _0;
                here.j = _27;
            }

            else if (down_right.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _2;
                here.j = _27;
            }

            else if (up_left.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _4;
                here.j = _27;
            }

            else if (up_right.type == TileType.Grass)
            {
                here.type = TileType.Dirt;
                here.i = _6;
                here.j = _27;
            }

            else
            {
                if (here.j != _30)
                {
                    here.i = _0;
                    here.j = _30;
                }
            }
        }

        void GrassTreeInterface(ref tile here, ref tile right, ref tile up, ref tile left, ref tile down, ref tile up_right, ref tile up_left, ref tile down_right, ref tile down_left)
        {
            if (here.type == TileType.Trees)
            {
                // If this is a tree with trees on both sides of it
                if (left.type == TileType.Trees && right.type == TileType.Trees || 
                    up_left.type == TileType.Trees && up_right.type == TileType.Trees)
                {
                    // If there is a full row of 3 trees underneath this tree, then this is a "full" center tree tile
                    if (up_left.type == TileType.Trees && up.type == TileType.Trees && up_right.type == TileType.Trees)
                    {
                        here.type = TileType.Trees;
                        here.i = _0;
                        here.j = _25;
                    }

                    // otherwise if we at least have a tree directly beneath us, there are 3 possibilities (missing left, missing right, or both)
                    else if (up_left.type != TileType.Trees && up.type == TileType.Trees && up_right.type == TileType.Trees)
                    {
                        here.type = TileType.Trees;
                        here.i = _0;
                        here.j = _22;
                    }

                    else if (up_left.type == TileType.Trees && up.type == TileType.Trees && up_right.type != TileType.Trees)
                    {
                        here.type = TileType.Trees;
                        here.i = _3;
                        here.j = _22;
                    }

                    else if (up_left.type != TileType.Trees && up.type == TileType.Trees && up_right.type != TileType.Trees)
                    {
                        here.type = TileType.Trees;
                        here.i = _6;
                        here.j = _22;
                    }

                    // otherwise there are no trees beneath us, so we are just a side tree tile
                    else
                    {
                        here.type = TileType.Trees;
                        here.i = _0;
                        here.j = _24;
                    }
                }

                // Corner piece, bottom left
                else if (right.type == TileType.Trees && up.type != TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _0;
                    here.j = _21;
                }

                // Corner piece, bottom right
                else if (left.type == TileType.Trees && up.type != TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _2;
                    here.j = _21;
                }

                // Side piece, left side
                else if ((right.type == TileType.Trees || up_right.type == TileType.Trees) && up.type == TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _0;
                    here.j = _20;
                }

                // Side piece, right side
                else if ((left.type == TileType.Trees || up_left.type == TileType.Trees) && up.type == TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _2;
                    here.j = _20;
                }

                // Single tree
                else if (up.type != TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _0;
                    here.j = _19;
                }

                // Single tree, with a tree below it
                else if (up.type == TileType.Trees)
                {
                    here.type = TileType.Trees;
                    here.i = _1;
                    here.j = _19;
                }
            }

            // Tree tops (caps) are drawn on grass tiles
            else if (here.type == TileType.Grass)
            {
                // If there is a full row of 3 trees underneath this tree, then this is a "full" tree cap tile
                if (up_left.type == TileType.Trees && up.type == TileType.Trees && up_right.type == TileType.Trees)
                {
                    here.type = TileType.Grass;
                    here.i = _0;
                    here.j = _23;
                }

                // If there is a tree below us and down left or down right, then this is a tree cap corner
                else if (up_left.type != TileType.Trees && up.type == TileType.Trees && up_right.type == TileType.Trees)
                {
                    here.type = TileType.Grass;
                    here.i = _4;
                    here.j = _21;
                }

                else if (up_left.type == TileType.Trees && up.type == TileType.Trees && up_right.type != TileType.Trees)
                {
                    here.type = TileType.Grass;
                    here.i = _6;
                    here.j = _21;
                }

                // If there is a tree below us but not down left or down right, then this is a lone tree cap
                else if (up_left.type != TileType.Trees && up.type == TileType.Trees && up_right.type != TileType.Trees)
                {
                    here.type = TileType.Grass;
                    here.i = _0;
                    here.j = _18;
                }

                else
                {
                    if (here.j != _31)
                    {
                        here.i = _0;
                        here.j = _31;
                    }
                }
            }
        }
    }
}
