using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class DataGroup : SimShader
    {
        public vec2 SelectedBound_BL, SelectedBound_TR;
        public void SelectedUnitsBounds()
        {
            BoundingTr.Apply(CurrentData, Output: Multigrid[0]);
            color bound_tr = MultigridReduce(_BoundingTr.Apply);

            BoundingBl.Apply(CurrentData, Output: Multigrid[0]);
            color bound_bl = MultigridReduce(_BoundingBl.Apply);

            SelectedBound_TR = SimShader.unpack_vec2(bound_tr);
            SelectedBound_BL = SimShader.unpack_vec2(bound_bl);
        }

        public void SelectAlongLine(vec2 p1, vec2 p2, vec2 size, bool Deselect, bool Selecting, float PlayerValue, bool EffectSelection)
        {
            DataDrawMouse.Using(Assets.SelectCircle_Data, PlayerValue, Output: SelectField, Clear: Color.Transparent);

            if (Selecting)
            {
                vec2 shift = CellSize.FlipY();

                for (int i = 0; i <= 10; i++)
                {
                    float t = i / 10.0f;
                    var pos = t * p2 + (1 - t) * p1;
                    RectangleQuad.Draw(GraphicsDevice, pos - shift, size);
                }
            }

            if (EffectSelection)
            {
                SelectUnits(Deselect);
            }
        }

        public void SelectInArea(vec2 pos, vec2 size, bool Deselect, bool Selecting, float PlayerValue, bool EffectSelection)
        {
            DataDrawMouse.Using(Assets.SelectCircle_Data, PlayerValue, Output: SelectField, Clear: Color.Transparent);

            if (Selecting)
            {
                RectangleQuad.Draw(GraphicsDevice, pos, size);
            }

            if (EffectSelection)
            {
                SelectUnits(Deselect);
            }
        }

        private void SelectUnits(bool Deselect)
        {
            ActionSelect.Apply(CurrentData, CurrentUnits, SelectField, Deselect, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);

            ActionSelect.Apply(PreviousData, CurrentUnits, SelectField, Deselect, Output: Temp1);
            Swap(ref Temp1, ref PreviousData);
        }

        public void AttackMoveApply(float Player, vec2 pos, vec2 Selected_BL, vec2 Selected_Size, vec2 Destination_BL, vec2 Destination_Size)
        {
            SetSelectedAction.Apply(CurrentData, CurrentUnits, SimShader.UnitAction.Attacking, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);

            ActionAttackSquare.Apply(CurrentData, TargetData, Destination_BL, Destination_Size, Selected_BL, Selected_Size, Output: Temp1);
            Swap(ref TargetData, ref Temp1);

            ActionAttack2.Apply(CurrentData, Extra, pos, Output: Temp1);
            Swap(ref Extra, ref Temp1);
        }
    }
}
