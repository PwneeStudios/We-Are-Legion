using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawUnitsZoomedOut : DrawUnits
    {
        color Presence(data data)
        {
            return Something(data) ?
                (selected(data) ? rgb(0x54c96b) : rgb(0x917c82)) :
                rgba(0,0,0,0);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, PointSampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            data
                right = CurrentData[RightOne],
                up    = CurrentData[UpOne],
                left  = CurrentData[LeftOne],
                down  = CurrentData[DownOne],
                here  = CurrentData[Here];

            output =    .5f *
                            .25f * (Presence(right) + Presence(up) + Presence(left) + Presence(down))
                      + .5f *
                             Presence(here);
            
            return output;
        }
    }

    public partial class DrawUnits : BaseShader
    {
        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        protected color Sprite(data u, unit d, vec2 pos, float direction, float frame, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(u) ? 4 : 0;

            pos.x += floor(frame);
            pos.y += (floor(direction * 255 + .5f) - 1 + selected_offset);
            pos *= UnitSpriteSheet.SpriteSize;

            var clr = Texture[pos];

            return PlayerColorize(clr, d.player);

            //return rgba(1,1,1,1);
            //return Circle(pos);
            //return tex2D(TextureSampler, pos);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnits, Field<unit> PreviousUnits, PointSampler Texture, float s)
        {
            color output = color.TransparentBlack;

            data
                cur = CurrentData[Here],
	            pre = PreviousData[Here];
            
            unit
                cur_unit  = CurrentUnits[Here],
                pre_unit = PreviousUnits[Here];

            if (!IsUnit(cur_unit) && !IsUnit(pre_unit)) return output;

            vec2 subcell_pos = get_subcell_pos(vertex, CurrentData.Size);

            if (Something(cur) && cur.change == Change.Stayed)
	        {
		        if (s > .5) pre = cur;

                float frame = cur_unit.anim > 0 ? s * UnitSpriteSheet.AnimLength + 255*cur_unit.anim : 0;
                output += Sprite(pre, pre_unit, subcell_pos, pre.direction, frame, Texture);
	        }
            else
            {
                float frame = s * UnitSpriteSheet.AnimLength;

                if (IsValid(cur.direction))
                {
                    var prior_dir = prior_direction(cur);

                    vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                    output += Sprite(cur, cur_unit, subcell_pos + offset, prior_dir, frame, Texture);
                }

                if (IsValid(pre.direction) && output.a < .025f)
                {
                    vec2 offset = -s * direction_to_vec(pre.direction);
                    output += Sprite(pre, pre_unit, subcell_pos + offset, pre.direction, frame, Texture);
                }
            }

            return output;
        }
    }
}
