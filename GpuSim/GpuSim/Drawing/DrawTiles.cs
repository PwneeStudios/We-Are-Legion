using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawTiles : BaseShader
    {
        protected color Sprite(tile c, vec2 pos, PointSampler Texture)
        {
            color clr = color.TransparentBlack;

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return clr;

            if (pos.x < .025 || pos.x > .975 || pos.y < .025 || pos.y > .975)
                clr += rgba(1, 1, 1, 1) * .2f;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += Float(c.i);
            pos.y += Float(c.j);
            pos *= TileSpriteSheet.SpriteSize;

            clr += Texture[pos];

            return clr;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<tile> tiles, PointSampler Texture)
        {
            color output = color.TransparentBlack;

            tile here = tiles[Here];
            
            vec2 subcell_pos = get_subcell_pos(vertex, tiles.Size);

            if (here.type > _0)
            {
                output += Sprite(here, subcell_pos, Texture);
            }

            return output;
        }
    }
}
