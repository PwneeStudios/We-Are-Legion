using FragSharpFramework;

namespace GpuSim
{
    /*
    public partial class DrawUnitsZoomedOut : DrawUnits
    {
        color Presence(data data)
        {
            return (Something(data) && !IsStationary(data)) ?
                (selected(data) ? Color_Selected : Color_Unselected) :
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
    */

    public partial class DrawUnitsZoomedOut : DrawUnits
    {
        color Presence(data data, unit unit)
        {
            return (Something(data) && !IsStationary(data)) ?
                (selected(data) ? SelectedUnitColor.Get(unit.player) : UnitColor.Get(unit.player)) :
                rgba(0, 0, 0, 0);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnit, Field<unit> PreviousUnit, PointSampler Texture, float PercentSimStepComplete)
        {
            color output = color.TransparentBlack;

            data data_here  = CurrentData[Here];
            unit unit_here  = CurrentUnit[Here];

            return Presence(data_here, unit_here);
        }
    }

    public partial class DrawUnitsZoomedOutBlur : DrawUnits
    {
        color Presence(data data, unit unit)
        {
            return (Something(data) && !IsStationary(data)) ?
                (selected(data) ? SelectedUnitColor.Get(unit.player) : UnitColor.Get(unit.player)) :
                rgba(0, 0, 0, 0);
        }

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
        color Circle(vec2 pos)
        {
            float r = length(pos - vec(.5f, .5f));
            if (r < .3f)
                return rgba(1, 1, 1, 1);
            else
                return rgba(0, 0, 0, 0);
        }

        protected color Sprite(data u, unit d, vec2 pos, float direction, float frame, PointSampler Texture, float blend, float select_size)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            bool draw_selected = selected(u) && pos.y > select_size;

            pos.x += floor(frame);
            pos.y += (Float(direction) - 1);
            pos *= UnitSpriteSheet.SpriteSize;

            var clr = Texture[pos];

            clr = PlayerColorize(clr, d.player);
            
            if (draw_selected)
            {
                float a = clr.a * blend;
                clr = a * clr + (1 - a) * SelectedUnitColor.Get(d.player);
            }

            return clr;
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnits, Field<unit> PreviousUnits, PointSampler Texture, float s, float second, float blend, float select_size)
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

                float _s = cur_unit.anim == _0 ? second : s;

                float frame = _s * UnitSpriteSheet.AnimLength + Float(cur_unit.anim);
                output += Sprite(pre, pre_unit, subcell_pos, pre.direction, frame, Texture, blend, select_size);
            }
            else
            {
                float frame = s * UnitSpriteSheet.AnimLength + Float(Anim.Walk);

                if (IsValid(cur.direction))
                {
                    var prior_dir = prior_direction(cur);

                    vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                    output += Sprite(cur, cur_unit, subcell_pos + offset, prior_dir, frame, Texture, blend, select_size);
                }

                if (IsValid(pre.direction) && output.a < .025f)
                {
                    vec2 offset = -s * direction_to_vec(pre.direction);
                    output += Sprite(pre, pre_unit, subcell_pos + offset, pre.direction, frame, Texture, blend, select_size);
                }
            }

            return output;
        }
    }
}
