using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawUnitZoomedOut : DrawUnit
    {
        color Presence(unit data)
        {
            return Something(data) ?
                (selected(data) ? rgb(0x54c96b) : rgb(0x917c82)) :
                rgba(0,0,0,0);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, Sampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            unit
                right = Current[RightOne],
                up    = Current[UpOne],
                left  = Current[LeftOne],
                down  = Current[DownOne],
                here  = Current[Here];

            output =    .5f *
                            .25f * (Presence(right) + Presence(up) + Presence(left) + Presence(down))
                      + .5f *
                             Presence(here);
            
            return output;
        }
    }

    public partial class DrawUnit : BaseShader
    {
        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        protected color Sprite(unit u, data d, vec2 pos, float anim, float frame, Sampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(u) ? 4 : 0;

            pos.x += ((int)(floor(frame)) % 5);
            pos.y += (floor(anim * 255 + .5f) - 1 + selected_offset);
            pos *= SpriteSize;

            var clr = Texture[pos];

            if (d.player == _1)
            {
                float r = clr.r;
                clr.r = clr.g;
                clr.g = r;
                clr.rgb *= .5f;
            }
            else if (d.player == _2)
            {
                float b = clr.b;
                clr.b = clr.g;
                clr.g = b;
            }
            else if (d.player == _3)
            {
                float r = clr.r;
                clr.r = clr.b;
                clr.b = r;
            }

            return clr;

            //return rgba(1,1,1,1);
            //return Circle(pos);
            //return tex2D(TextureSampler, pos);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, DataField CurData, DataField PrevData, Sampler Texture, float s)
        {
            color output = color.TransparentBlack;

            unit
                cur = Current[Here],
	            pre = Previous[Here];
            
            data
                cur_data  = CurData[Here],
                pre_data = PrevData[Here];

            vec2 subcell_pos = get_subcell_pos(vertex, Current.Size);

            if (Something(cur) && cur.change == Change.Stayed)
	        {
		        if (s > .5) pre = cur;

                output += Sprite(pre, pre_data, subcell_pos, pre.direction, 0, Texture);
	        }
            else
            {
                if (IsValid(cur.direction))
                {
                    var prior_dir = prior_direction(cur);

                    vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                    output += Sprite(cur, cur_data, subcell_pos + offset, prior_dir, s * 5, Texture);
                }

                if (IsValid(pre.direction) && output.a < .025f)
                {
                    vec2 offset = -s * direction_to_vec(pre.direction);
                    output += Sprite(pre, pre_data, subcell_pos + offset, pre.direction, s * 5, Texture);
                }
            }

            return output;
        }
    }

    public partial class DrawUnit_v2 : BaseShader
    {
        protected color Sprite(vec2 sprite, vec2 pos, float frame, Sampler Texture)
        {
            if (pos.x >= 1 || pos.y >= 1 || pos.x <= 0 || pos.y <= 0)
                return color.TransparentBlack;

            //return rgb(0xffffff);

            pos.x += ((int)(floor(frame)) % 5);
            pos.y -= 1;
            pos = (sprite * 255.0f + pos) * SpriteSize;

            return Texture[pos];
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, Sampler Texture, float s)
        {
            color output = color.TransparentBlack;

            unit
                right = Current[RightOne],
                up = Current[UpOne],
                left = Current[LeftOne],
                down = Current[DownOne],
                here = Current[Here];

            vec2 subcell_pos = (get_subcell_pos(vertex, Current.Size) + vec(.5f, .5f)) / 2;

            //vec2 cur_offset = (1 - s) * (here.zw - vec(.5f, .5f)) * 2;
            vec2 cur_offset = vec2.Zero;

            if (up.y > 0 && output.a < .025f)
            {
                output += Sprite(up.xy, subcell_pos + cur_offset + vec(0, -.5f), s * 5, Texture);
            }

            if (right.y > 0 && output.a < .025f)
            {
                output += Sprite(right.xy, subcell_pos + cur_offset + vec(-.5f, 0), s * 5, Texture);
            }

            if (left.y > 0 && output.a < .025f)
            {
                output += Sprite(left.xy, subcell_pos + cur_offset + vec(.5f, 0), s * 5, Texture);
            }

            if (here.y > 0 && output.a < .025f)
            {
                output += Sprite(here.xy, subcell_pos + cur_offset, s * 5, Texture);
            }

            if (down.y > 0 && output.a < .025f)
            {
                output += Sprite(down.xy, subcell_pos + cur_offset + vec(0, .5f), s * 5, Texture);
            }

            //if (pre.y > 0 && output.a < .025f)
            //{
            //    unit pre = Previous[Here];
            //    vec2 pre_offset = -s * (pre.zw - vec(.5f, .5f)) * 2;
            //    output += Sprite(pre.xy, subcell_pos + pre_offset, s * 5, Texture);
            //}

            return output;
        }
    }
}
