using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawTiles : BaseShader
    {
        protected color Sprite(tile c, vec2 pos, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += Float(c.i);
            pos.y += Float(c.j);
            pos *= TileSpriteSheet.SpriteSize;

            return Texture[pos];
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
