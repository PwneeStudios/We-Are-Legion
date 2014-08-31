using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawTiles : BaseShader
    {
        color Sprite(tile c, vec2 pos, PointSampler Texture)
        {
            color clr = color.TransparentBlack;

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return clr;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += Float(c.i);
            pos.y += Float(c.j);
            pos *= TileSpriteSheet.SpriteSize;

            clr += Texture[pos];

            return clr;
        }

        color GridLines(vec2 pos)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            if (pos.x < .025 || pos.x > .975 || pos.y < .025 || pos.y > .975)
                return rgba(1, 1, 1, 1) * .2f;

            return color.TransparentBlack;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<tile> Tiles, PointSampler Texture, bool draw_grid)
        {
            color output = color.TransparentBlack;

            tile here = Tiles[Here];
            
            vec2 subcell_pos = get_subcell_pos(vertex, Tiles.Size);

            if (here.type > _0)
            {
                output += Sprite(here, subcell_pos, Texture);

                if (draw_grid) output += GridLines(subcell_pos);
            }

            return output;
        }
    }
}
