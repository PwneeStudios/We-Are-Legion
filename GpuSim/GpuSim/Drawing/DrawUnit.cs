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
        readonly vec2 SpriteSize = vec(1.0f / 5.0f, 1.0f / 8.0f);

        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        protected color Sprite(unit data, vec2 pos, float anim, float frame, Sampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(data) ? 4 : 0;

            pos.x += ((int)(floor(frame)) % 5);
            pos.y += (floor(anim * 255 + .5f) - 1 + selected_offset);
            pos *= SpriteSize;

            var clr = Texture[pos];

            //if (data.a == _1)
            //{
            //    float r = clr.r;
            //    clr.r = clr.g;
            //    clr.g = r;
            //}
            //else if (data.a == _2)
            //{
            //    float b = clr.b;
            //    clr.b = clr.g;
            //    clr.g = b;
            //}
            //else if (data.a == _3)
            //{
            //    float r = clr.r;
            //    clr.r = clr.b;
            //    clr.b = r;
            //}

            return clr;

            //return rgba(1,1,1,1);
            //return Circle(pos);
            //return tex2D(TextureSampler, pos);
        }

        vec2 get_subcell_pos(VertexOut vertex, vec2 grid_size)
        {
            vec2 coords = vertex.TexCoords * grid_size;
            float i = floor(coords.x);
            float j = floor(coords.y);

            return coords - vec(i, j);
        }

        vec2 direction_to_vec(float direction)
        {
	        float angle = (direction * 255 - 1) * (3.1415926f / 2.0f);
	        return IsValid(direction) ? vec(cos(angle), sin(angle)) : vec2.Zero;
        }


        [FragmentShader]
        color FragmentShader(VertexOut vertex, UnitField Extra, UnitField Current, UnitField Previous, Sampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            unit cur = Current[Here];
	        unit pre = Previous[Here];

            //unit extra = Extra[Here];
            //output = .2f * (color)extra;

            vec2 subcell_pos = get_subcell_pos(vertex, Current.Size);

            if (Something(cur) && cur.change == Change.Stayed)
	        {
		        if (PercentSimStepComplete > .5) pre = cur;

                output += Sprite(pre, subcell_pos, pre.direction, 0, Texture);
	        }
            else
            {
                if (IsValid(cur.direction))
                {
                    vec2 vel = direction_to_vec(prior_direction(cur));

                    output += Sprite(cur, subcell_pos + (1 - PercentSimStepComplete) * vel, prior_direction(cur), PercentSimStepComplete * 5, Texture);
                }

                if (IsValid(pre.direction))
                {
                    vec2 vel = direction_to_vec(pre.direction);

                    output += Sprite(pre, subcell_pos - PercentSimStepComplete * vel, pre.direction, PercentSimStepComplete * 5, Texture);
                }
            }

            //output.a *= 2; // Increase alpha when zoomed out for better visibility

            return output;
        }
    }
}
