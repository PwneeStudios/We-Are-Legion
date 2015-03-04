using FragSharpFramework;

namespace Game
{
    public partial class DrawTiles : BaseShader
    {
        protected color Sprite(tile c, vec2 pos, PointSampler Texture, bool solid_blend_flag, float solid_blend)
        {
            color clr = color.TransparentBlack;

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return clr;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += Float(c.i);
            pos.y += Float(c.j);
            pos *= TileSpriteSheet.SpriteSize;

            clr = Texture[pos];

            if (solid_blend_flag)
            {
                color solid_clr = FarColor[Int(c.type), 6 + (int)(c.type)];
                clr = solid_blend * clr + (1 - solid_blend) * solid_clr;
            }

            return clr;
        }

        protected color GridLines(vec2 pos)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            if (pos.x < .025 || pos.x > .975 || pos.y < .025 || pos.y > .975)
                return rgba(1, 1, 1, 1) * .2f;

            return color.TransparentBlack;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<tile> Tiles, PointSampler Texture, [Vals.Bool] bool draw_grid,
            [Vals.Bool] bool solid_blend_flag, float solid_blend)
        {
            color output = color.TransparentBlack;

            tile here = Tiles[Here];
            
            vec2 subcell_pos = get_subcell_pos(vertex, Tiles.Size);

            if (here.type > _0)
            {
                output += Sprite(here, subcell_pos, Texture, solid_blend_flag, solid_blend);

                if (draw_grid) output += GridLines(subcell_pos);
            }

            return output;
        }
    }

    public partial class DrawOutsideTiles : DrawTiles
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<tile> Tiles, PointSampler Texture,
            [Vals.Bool] bool solid_blend_flag, float solid_blend)
        {
            color output = color.TransparentBlack;

            vec2 subcell_pos = get_subcell_pos(vertex, Tiles.Size);

            tile here = tile.Nothing;
            here.i = _0;
            here.j = _25;
            here.type = TileType.Trees;

            output += Sprite(here, subcell_pos, Texture, solid_blend_flag, solid_blend);

            return output;
        }
    }
}
