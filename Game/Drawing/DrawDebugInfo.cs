using FragSharpFramework;

namespace Game
{
    public partial class DrawDebugInfo : BaseShader
    {
        protected color DrawDebugInfoTile(float index_x, float index_y, vec2 pos, PointSampler Texture, vec2 SpriteSize)
        {
            color clr = color.TransparentBlack;

            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return clr;

            pos = pos * .98f + vec(.01f, .01f);

            pos.x += index_x;
            pos.y += index_y;
            pos *= SpriteSize;

            clr += Texture[pos];

            return clr;
        }

        protected color DrawDebugArrow(float dir, vec2 pos, PointSampler Texture)
        {
            return DrawDebugInfoTile(_0, Float(dir - _1), pos, Texture, DebugArrowsSpriteSheet.SpriteSize);
        }

        protected color DrawDebugNum(float num, vec2 pos, PointSampler Texture)
        {
            return DrawDebugInfoTile(num, _0, pos, Texture, DebugNumSpriteSheet.SpriteSize);
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
                output *= DrawDebugArrow(here.dir, subcell_pos, Texture);
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
                return (color)(here.polarity > .5 ? vec(1, 0, 0, 1) : vec(0, 1, 0, 1));
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

            if (here.dir > _0)
            {
                if (subcell_pos > vec(.5f, .5f))
                {
                    vec2 subcell_pos_1 = get_subcell_pos(vertex, Geo.Size * 2);
                    output += DrawDebugNum(unpack_val(PolarDistance[Here].xy), subcell_pos_1, Texture) * rgb(0xFF8080);
                }

                if (subcell_pos < vec(.5f, .5f))
                {
                    vec2 subcell_pos_2 = get_subcell_pos(vertex, Geo.Size * 2);
                    output += DrawDebugNum(unpack_val(PolarDistance[Here].zw), subcell_pos_2, Texture) * rgb(0xFF8080);
                }

                return output;
            }



            if (subcell_pos.y > .5)
                dist = unpack_val(PolarDistance[Here].xy);
            else
                dist = unpack_val(PolarDistance[Here].zw);

            if (here.dir > _0)
            {
                dist = dist / 1024.0f;
                output = (color)vec(dist, dist, dist, 1.0f);
            }

            return output;
        }
    }
}
