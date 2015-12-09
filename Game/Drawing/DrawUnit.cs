using FragSharpFramework;

namespace Game
{
    public partial class DrawUnitsZoomedOutBlur : DrawUnits
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<unit> CurrentUnit, [Player.Vals] float player)
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
                            .25f * (Presence(player, data_right, unit_right) + Presence(player, data_up, unit_up) + Presence(player, data_left, unit_left) + Presence(player, data_down, unit_down))
                      + .5f *
                                    Presence(player, data_here, unit_here);

            return output;
        }
    }

    public partial class DrawUnits : BaseShader
    {
        protected color Presence(float player, data data, unit unit)
        {
            return (Something(data) && !IsStationary(data)) ?
                SolidColor(player, data, unit) :
                color.TransparentBlack;
        }

        protected color SolidColor(float player, data data, unit unit)
        {
            return unit.player == player && fake_selected(data) ? SelectedUnitColor.Get(unit.player) : UnitColor.Get(unit.player);           
        }

        protected color Sprite(float player, data d, unit u, vec2 pos, float frame, TextureSampler Texture,
            float selection_blend, float selection_size,
            bool solid_blend_flag, float solid_blend)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            bool draw_selected = u.player == player && fake_selected(d) && pos.y > selection_size;

            pos.x += floor(frame);
            pos.y += Dir.Num(d) + 4 * Player.Num(u) + 4 * 4 * UnitType.UnitIndex(u);
            pos *= UnitSpriteSheet.SpriteSize;

            var clr = Texture[pos];

            //if (draw_selected)
            //{
            //    float a = clr.a * selection_blend;
            //    clr = a * clr + (1 - a) * SelectedUnitColor.Get(u.player);
            //}

            if (solid_blend_flag)
            {
                clr = solid_blend * clr + (1 - solid_blend) * SolidColor(player, d, u);
            }

            return clr;
        }

        protected color ShadowSprite(float player, data d, unit u, vec2 pos, TextureSampler Texture,
            float selection_blend, float selection_size,
            bool solid_blend_flag, float solid_blend)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            //bool draw_selected = u.player == player && show_selected(d) && pos.y > selection_size;
            bool draw_selected = u.player == player && fake_selected(d);

            var clr = Texture[pos];

            if (draw_selected)
            {
                if (clr.a > 0)
                {
                    float a = clr.a;
                    clr =  SelectedUnitColor.Get(u.player);
                    clr.a = a;
                }
                //float a = clr.a * selection_blend;
                //clr = a * clr + (1 - a) * SelectedUnitColor.Get(u.player);
            }

            //if (solid_blend_flag)
            //{
            //    clr = solid_blend * clr + (1 - solid_blend) * SolidColor(player, d, u);
            //}

            return clr;
        }

        //RelativeIndex ShiftedHere = Here + new RelativeIndex(0f, .3f);
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<data> CurrentData, Field<data> PreviousData, Field<unit> CurrentUnits, Field<unit> PreviousUnits,
            TextureSampler UnitTexture, TextureSampler ShadowTexture,
            [Player.Vals] float player,
            float s, float t,
            float selection_blend, float selection_size,
            [Vals.Bool] bool solid_blend_flag, float solid_blend)
        {
            // Calculate shadow pixel
            color shadow = color.TransparentBlack;
            vec2 shadow_subcell_pos = get_subcell_pos(vertex, CurrentData.Size, vec(0f, -.5f));
            var shadow_here = Here + new RelativeIndex(0, -.5f);

            data
                shadow_cur = CurrentData[shadow_here],
                shadow_pre = PreviousData[shadow_here];

            unit
                shadow_cur_unit = CurrentUnits[shadow_here],
                shadow_pre_unit = PreviousUnits[shadow_here];

            if (IsUnit(shadow_cur_unit) || IsUnit(shadow_pre_unit))
            {
                if (Something(shadow_cur) && shadow_cur.change == Change.Stayed)
                {
                    if (s > .5) shadow_pre = shadow_cur;

                    shadow += ShadowSprite(player, shadow_pre, shadow_pre_unit, shadow_subcell_pos, ShadowTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                }
                else
                {
                    if (IsValid(shadow_cur.direction))
                    {
                        var prior_dir = prior_direction(shadow_cur);
                        shadow_cur.direction = prior_dir;

                        vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                        shadow += ShadowSprite(player, shadow_cur, shadow_cur_unit, shadow_subcell_pos + offset, ShadowTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                    }

                    if (IsValid(shadow_pre.direction) && shadow.a < .025f)
                    {
                        vec2 offset = -s * direction_to_vec(shadow_pre.direction);
                        shadow += ShadowSprite(player, shadow_pre, shadow_pre_unit, shadow_subcell_pos + offset, ShadowTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                    }
                }
            }

            // Calculate unit pixel
            color output = color.TransparentBlack;
            vec2 subcell_pos = get_subcell_pos(vertex, CurrentData.Size);

            data
                cur = CurrentData[Here],
                pre = PreviousData[Here];
            
            unit
                cur_unit = CurrentUnits[Here],
                pre_unit = PreviousUnits[Here];

            if (!IsUnit(cur_unit) && !IsUnit(pre_unit)) return shadow;

            if (Something(cur) && cur.change == Change.Stayed)
            {
                if (s > .5) pre = cur;

                float _s = (cur_unit.anim == _0 ? t : s);

                if (cur_unit.anim == Anim.DoRaise)
                {
                    cur_unit.anim = Anim.Die;
                    _s = 1f - _s;
                }

                float frame = _s * UnitSpriteSheet.AnimLength + Float(cur_unit.anim);
                output += Sprite(player, pre, pre_unit, subcell_pos, frame, UnitTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
            }
            else
            {
                float frame = s * UnitSpriteSheet.AnimLength + Float(Anim.Walk);

                if (IsValid(cur.direction))
                {
                    var prior_dir = prior_direction(cur);
                    cur.direction = prior_dir;

                    vec2 offset = (1 - s) * direction_to_vec(prior_dir);
                    output += Sprite(player, cur, cur_unit, subcell_pos + offset, frame, UnitTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                }

                if (IsValid(pre.direction) && output.a < .025f)
                {
                    vec2 offset = -s * direction_to_vec(pre.direction);
                    output += Sprite(player, pre, pre_unit, subcell_pos + offset, frame, UnitTexture, selection_blend, selection_size, solid_blend_flag, solid_blend);
                }
            }

            if (output.a < .025f) output = shadow;

            return output;
        }

        //data
        //    cur_left  = CurrentData[LeftOne],
        //    cur_mid   = CurrentData[Here],
        //    cur_right = CurrentData[RightOne],
        //    pre_left  = PreviousData[LeftOne],
        //    pre_mid   = PreviousData[Here],
        //    pre_right = PreviousData[RightOne];

        //AddCurShadow(vec(-2, 0), s, ref cur_right, ref subcell_pos, ref output);
        //AddPreShadow(vec(-2, 0), s, ref pre_right, ref subcell_pos, ref output);

        //void AddCurShadow(vec2 offset, float s, ref data cell, ref vec2 subcell_pos, ref color output)
        //{
        //    if (Something(cell))
        //    {
        //        vec2 dir = cell.change == Change.Stayed ? vec2.Zero : direction_to_vec(cell.direction);
        //        offset = offset + (1 - s) * dir + subcell_pos;

        //        if (offset.x > -1.5 && offset.y > 0 && offset.y < 1)
        //        {
        //            output = new color(1f, 1f, 1f, 1f);
        //        }
        //    }
        //}

        //void AddPreShadow(vec2 offset, float s, ref data cell, ref vec2 subcell_pos, ref color output)
        //{
        //    if (Something(cell))
        //    {
        //        vec2 dir = cell.change == Change.Stayed ? vec2.Zero : direction_to_vec(cell.direction);
        //        offset = offset - s * dir + subcell_pos;

        //        if (offset.x > -1.5 && offset.y > 0 && offset.y < 1)
        //        {
        //            output = new color(1f, 1f, 1f, 1f);
        //        }
        //    }
        //}
    }
}
