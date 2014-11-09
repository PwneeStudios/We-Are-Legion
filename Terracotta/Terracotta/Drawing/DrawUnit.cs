using FragSharpFramework;

namespace Terracotta
{
    public partial class DrawUnitsZoomedOutBlur : DrawUnits
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnit, Field<unit> PreviousUnit, PointSampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            data
                data_right = CurrentData[RightOne],
                data_up = CurrentData[UpOne],
                data_left = CurrentData[LeftOne],
                data_down = CurrentData[DownOne],
                data_here = CurrentData[Here];

            unit
                unit_right = CurrentUnit[RightOne],
                unit_up = CurrentUnit[UpOne],
                unit_left = CurrentUnit[LeftOne],
                unit_down = CurrentUnit[DownOne],
                unit_here = CurrentUnit[Here];

            output = .5f *
                            .25f * (Presence(data_right, unit_right) + Presence(data_up, unit_up) + Presence(data_left, unit_left) + Presence(data_down, unit_down))
                      + .5f *
                                    Presence(data_here, unit_here);

            return output;
        }
    }

    public partial class DrawUnits : BaseShader
    {
        protected color Presence(data data, unit unit)
        {
            return (Something(data) && !IsStationary(data)) ?
                SolidColor(data, unit) :
                color.TransparentBlack;
        }

        protected color SolidColor(data data, unit unit)
        {
            return selected(data) ? SelectedUnitColor.Get(unit.player) : UnitColor.Get(unit.player);           
        }

        protected color Sprite(data d, unit u, vec2 pos, float frame, PointSampler Texture,
            float selection_blend, float selection_size,
            bool solid_blend_flag, float solid_blend)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            bool draw_selected = selected(d) && pos.y > selection_size;

            pos.x += floor(frame);
            pos.y += Dir.Num(d) + 4 * Player.Num(u) + 4 * 4 * UnitType.UnitIndex(u);
            pos *= UnitSpriteSheet.SpriteSize;

            var clr = Texture[pos];

            if (draw_selected)
            {
                float a = clr.a * selection_blend;
                clr = a * clr + (1 - a) * SelectedUnitColor.Get(u.player);
            }

            if (solid_blend_flag)
            {
                clr = solid_blend * clr + (1 - solid_blend) * SolidColor(d, u);
            }

            return clr;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnits, Field<unit> PreviousUnits, PointSampler Texture,
            float s, float t,
            float selection_blend, float selection_size,
            [Vals.Bool] bool solid_blend_flag, float solid_blend)
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

                float _s = cur_unit.anim == _0 ? t : s;

                float frame = _s * UnitSpriteSheet.AnimLength + Float(cur_unit.anim);
                output += Sprite(pre, pre_unit, subcell_pos, frame, Texture, selection_blend, selection_size, solid_blend_flag, solid_blend);
            }
            else
            {
                float frame = s * UnitSpriteSheet.AnimLength + Float(Anim.Walk);

                if (IsValid(cur.direction))
                {
                    var prior_dir = prior_direction(cur);
                    cur.direction = prior_dir;

                    vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                    output += Sprite(cur, cur_unit, subcell_pos + offset, frame, Texture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                }

                if (IsValid(pre.direction) && output.a < .025f)
                {
                    vec2 offset = -s * direction_to_vec(pre.direction);
                    output += Sprite(pre, pre_unit, subcell_pos + offset, frame, Texture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                }
            }

            return output;
        }
    }
}
