using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawDebugInfo : BaseShader
    {
        protected color DrawDebugInfoTile(float dir, float val, vec2 pos, PointSampler Texture)
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
    }

    public partial class DrawGeoInfo : DrawDebugInfo
    {
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

    public partial class DrawDirwardInfo : DrawDebugInfo
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<dirward> Dirward, PointSampler Texture)
        {
            color output = color.TransparentBlack;

            dirward here = Dirward[Here];

            vec2 subcell_pos = get_subcell_pos(vertex, Dirward.Size);

            if (ValidDirward(here))
            {
                // Draw guid coloring
                vec2 guid = fmod(here.geo_id * 1293.4184145f, 1.0f);
                output.r += guid.x;
                output.g += guid.y;
                output.a = 1f;
                output.rgb *= output.a;
            }

            // Draw polarity only
            if (ValidDirward(here))
            {
                return here.polarity > .5 ? vec(1, 0, 0, 1) : vec(0, 1, 0, 1);
            }

            return output;
        }
    }

    public partial class DrawPolarInfo : DrawDebugInfo
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<geo> Geo, Field<vec4> PolarDistance, PointSampler Texture)
        {
            color output = color.TransparentBlack;

            geo here = Geo[Here];
            float dist = 0;
            
            vec2 subcell_pos = get_subcell_pos(vertex, Geo.Size);

            if (subcell_pos.y > .5)
                dist = unpack_val(PolarDistance[Here].xy);
            else
                dist = unpack_val(PolarDistance[Here].zw);

            if (here.dir > _0)
            {
                dist = dist / 1024.0f;
                output = vec(dist, dist, dist, 1.0f);
            }

            return output;
        }
    }
}
