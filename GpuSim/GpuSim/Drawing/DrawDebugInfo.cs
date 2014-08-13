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
                //output += DrawDebugInfoTile(here.dir, 0, subcell_pos, Texture);

                //if (here.bad == _true)
                //    output.r = 1;

                vec2 v = pos(here);
                int hash = (int)(v.x + 4096 * v.y) % 4;

                if (hash == 0) output += rgb(0x330000);
                if (hash == 1) output += rgb(0x003300);
                if (hash == 2) output += rgb(0x000033);
                if (hash == 3) output += rgb(0x330033);
            }            

            return output;
        }
    }
}
