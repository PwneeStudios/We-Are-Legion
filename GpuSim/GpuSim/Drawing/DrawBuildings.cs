using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawBuildings : BaseShader
    {
        protected color Sprite(data u, unit d, vec2 pos, float direction, float frame, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(u) ? 4 : 0;

            pos.x += floor(frame);
            pos.y += (floor(direction * 255 + .5f) - 1 + selected_offset);
            pos *= SpriteSize;

            var clr = Texture[pos];

            return PlayerColorize(clr, d.player);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> Current, Field<unit> CurData, PointSampler Texture, float s)
        {
            color output = color.TransparentBlack;

            data cur = Current[Here];
            unit cur_data  = CurData[Here];

            vec2 subcell_pos = get_subcell_pos(vertex, Current.Size);

            if (Something(cur) && cur_data.type == UnitType.Barracks)
	        {
                float frame = cur_data.anim > 0 ? s * AnimLength + 255*cur_data.anim : 0;
                //output += Sprite(pre, pre_data, subcell_pos, pre.direction, frame, Texture);
	        }

            return output;
        }
    }
}
