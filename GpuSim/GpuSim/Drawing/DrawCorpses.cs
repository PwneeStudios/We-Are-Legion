using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawCorpses : BaseShader
    {
        protected color Sprite(corpse c, vec2 pos, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos.x += floor(255*Anim.Dead);
            pos.y += floor(c.direction * 255 + .5f) - 1;
            pos *= SpriteSize;

            var clr = Texture[pos];

            return PlayerColorize(clr, c.player);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<corpse> Corpses, PointSampler Texture)
        {
            color output = color.TransparentBlack;

            corpse here = Corpses[Here];
            
            vec2 subcell_pos = get_subcell_pos(vertex, Corpses.Size);

            if (Something(here))
	        {
                output += Sprite(here, subcell_pos, Texture);
	        }

            return output;
        }
    }
}
