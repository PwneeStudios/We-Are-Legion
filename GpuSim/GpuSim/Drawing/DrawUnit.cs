using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawUnit : BaseShader
    {
        readonly vec2 SpriteSize = vec(1.0f / 5.0f, 1.0f / 4.0f);

        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        color Sprite(unit data, vec2 pos, float cycle_offset, Sampler Texture, float PercentSimStepComplete)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos *= SpriteSize;
            pos.x += SpriteSize.x * (((int)(PercentSimStepComplete / SpriteSize.x) + (int)(cycle_offset * 255)) % 5) * data.b;
            pos.y += (data.direction * 255 - 1) * SpriteSize.y;

            //return Texture[pos];
            var clr = Texture[pos];

            if (data.a > .75)
            {
                float r = clr.r;
                clr.r = clr.g;
                clr.g = r;
            }
            else if (data.a > .5)
            {
                float b = clr.b;
                clr.b = clr.g;
                clr.g = b;
            }
            else if (data.a > .25)
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
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, Sampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            unit cur = Current[Here];
	        unit pre = Previous[Here];

            vec2 subcell_pos = get_subcell_pos(vertex, Current.Size);

            if (cur.a == pre.a && cur.a != 0)
	        {
		        if (PercentSimStepComplete > .5) pre = cur;

                pre.b = 0;
                output += Sprite(pre, subcell_pos, cur.a, Texture, PercentSimStepComplete);
	        }
            else
            {
                if (IsValid(cur.direction))
                {
                    vec2 vel = direction_to_vec(cur.direction);

                    cur.b = 1;
                    output += Sprite(cur, subcell_pos + (1 - PercentSimStepComplete) * vel, cur.a, Texture, PercentSimStepComplete);
                }

                if (IsValid(pre.direction))
                {
                    vec2 vel = direction_to_vec(pre.direction);

                    pre.b = 1;
                    output += Sprite(pre, subcell_pos - PercentSimStepComplete * vel, pre.a, Texture, PercentSimStepComplete);
                }
            }

            return output;
        }
    }
}
