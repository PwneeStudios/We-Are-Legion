using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawDebugInfo : BaseShader
    {
        color DrawDebugInfoTile(float dir, float val, vec2 pos, PointSampler Texture)
        {
            color clr = color.TransparentBlack;

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return clr;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += Float(val);
            pos.y += Float(dir - _1);
            pos *= DebugInfoSpriteSheet.SpriteSize;

            clr += Texture[pos];

            return clr;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<geo> Geo, PointSampler Texture)
        {
            color output = color.TransparentBlack;

            geo here = Geo[Here];
            
            vec2 subcell_pos = get_subcell_pos(vertex, Geo.Size);

            if (here.dir > _0)
            {
                // Draw arrow
                //output += DrawDebugInfoTile(here.dir, 0, subcell_pos, Texture);

                // Draw bad cell info
                //if (here.bad == _true) output.r = 1;

                // Draw guid coloring
                vec2 guid = fmod(here.geo_id * 1293.4184145f, 1.0f);
                output.r += guid.x;
                output.g += guid.y;
                output.a = 1f;
                output.rgb *= output.a;

                // Draw arrow over
                output *= DrawDebugInfoTile(here.dir, 0, subcell_pos, Texture);
            }            

            return output;
        }
    }
}
