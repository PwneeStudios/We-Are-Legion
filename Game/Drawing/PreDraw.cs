using FragSharpFramework;

namespace Game
{
    public partial class DrawPrecomputation_Cur : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> Current, Field<data> Previous)
        {
            color output = color.TransparentBlack;

            data cur = Current[Here];
            data pre = Previous[Here];

            float selected_offset = selected(cur) ? _4 : _0;

            float anim = 0;
            vec2 vel = vec2.Zero;

            if (Something(cur) && cur.change == Change.Stayed)
            {
                anim = cur.direction;
            }
            else
            {
                if (IsValid(cur.direction))
                {
                    anim = prior_direction(cur);
                    vel = direction_to_vec(prior_direction(cur));
                }
                else
                {
                    return color.Zero;
                }
            }

            vec2 uv;
            uv.x = 0;
            uv.y = anim + selected_offset;

            output.xy = uv;
            output.zw = vel / 2 + vec(.5f, .5f);
            
            return output;
        }
    }

    public partial class DrawPrecomputation_Pre : SimShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> Current, Field<data> Previous)
        {
            color output = color.TransparentBlack;

            data cur = Current[Here];
            data pre = Previous[Here];

            float selected_offset = selected(pre) ? _4 : _0;

            float anim = 0;
            vec2 vel = vec2.Zero;

            if (Something(cur) && cur.change == Change.Stayed)
            {
                return color.Zero;
            }
            else
            {
                if (IsValid(pre.direction))
                {
                    anim = pre.direction;
                    vel = direction_to_vec(pre.direction);
                }
                else
                {
                    return color.Zero;
                }
            }

            vec2 uv;
            uv.x = 0;
            uv.y = anim + selected_offset;

            output.xy = uv;
            output.zw = vel / 2 + vec(.5f, .5f);

            return output;
        }
    }
}