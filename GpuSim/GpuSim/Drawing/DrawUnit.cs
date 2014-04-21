using FragSharpFramework;

namespace GpuSim
{
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

        color Sprite(unit data, vec2 pos, float anim, float frame, Sampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos.x += ((int)(floor(frame)) % 5);
            pos.y += (anim * 255 - 1 + 4 * data.a * 255);
            pos *= SpriteSize;

            //pos.y += data.a * _4 * SpriteSize.y;

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
        color FragmentShader(VertexOut vertex, UnitField Current, UnitField Previous, UnitField Paths, Sampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            unit cur = Current[Here];
	        unit pre = Previous[Here];

            unit path = Paths[Here];
            
            //output.r = path.direction * 50;
            //if (path.direction == Dir.Left) output.r = 1;
            //if (path.direction == Dir.Right) output.g = 1;
            //if (path.direction == Dir.Up) output.b = 1;
            //if (path.direction == Dir.Down) output.a = 1;

            vec2 subcell_pos = get_subcell_pos(vertex, Current.Size);

            if (Something(cur) && cur.change == Change.Stayed)
            //if (cur.a == pre.a && cur.a != 0)
	        {
		        if (PercentSimStepComplete > .5) pre = cur;

                output += Sprite(pre, subcell_pos, pre.direction, 0, Texture);
	        }
            else
            {
                if (IsValid(cur.direction))
                {
                    vec2 vel = direction_to_vec(cur.prior_direction);

                    output += Sprite(cur, subcell_pos + (1 - PercentSimStepComplete) * vel, cur.prior_direction, PercentSimStepComplete * 5, Texture);
                }

                if (IsValid(pre.direction))
                {
                    vec2 vel = direction_to_vec(pre.direction);

                    output += Sprite(pre, subcell_pos - PercentSimStepComplete * vel, pre.direction, PercentSimStepComplete * 5, Texture);
                }
            }

            return output;
        }
    }
}
