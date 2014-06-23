using FragSharpFramework;

namespace GpuSim
{
    public partial class Action_PaintTiles : SimShader
    {
        [FragmentShader]
        tile FragmentShader(VertexOut vertex, Field<tile> Tiles, Field<data> Select, float type)
        {
            tile here = Tiles[Here];
            data select = Select[Here];

            if (Something(select))
            {
                here.type = type;
                here.i = _0;
                here.j = _31;
            }

            return here;
        }
    }

    public partial class UpdateTiles : SimShader
    {
        bool xor(bool a, bool b)
        {
            return a != b;
        }

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
                bool grass_on_left = left.type == TileType.Grass || up_left.type == TileType.Grass || down_left.type == TileType.Grass;
                bool grass_on_right = right.type == TileType.Grass || up_right.type == TileType.Grass || down_right.type == TileType.Grass;
                bool grass_on_top = up_left.type == TileType.Grass || up.type == TileType.Grass || up_right.type == TileType.Grass;
                bool grass_on_bottom = down_left.type == TileType.Grass || down.type == TileType.Grass || down_right.type == TileType.Grass;

                // If we're straddled on two opposite sides by grass, then just turn into grass
                if (left.type == TileType.Grass && right.type == TileType.Grass ||
                    up.type == TileType.Grass && down.type == TileType.Grass)
                    //left.type == TileType.Grass && xor(up_right.type == TileType.Grass, down_right.type == TileType.Grass) ||
                    //right.type == TileType.Grass && xor(up_left.type == TileType.Grass, down_left.type == TileType.Grass) ||
                    //up.type == TileType.Grass && xor(down_right.type == TileType.Grass, down_left.type == TileType.Grass) ||
                    //down.type == TileType.Grass && xor(up_right.type == TileType.Grass, up_left.type == TileType.Grass))
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

                else if (grass_on_left && right.type == TileType.Grass ||
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
            }

            return here;
        }
    }
}
