using Microsoft.Xna.Framework;
using FragSharpFramework;

namespace Game
{
    public partial class DataGroup : SimShader
    {
        public vec2 DragonLordDeathGridCoord()
        {
            DyingDragonLordGridCoord.Apply(CurrentUnits, Output: Multigrid[0]);
            color packed = MultigridReduce(_BoundingTr.Apply);
            vec2 pos = SimShader.unpack_vec2(packed);
            
            return pos;
        }

        public vec2 DragonLordPos(float player)
        {
            DragonLordGridCoord.Apply(CurrentUnits, player, Output: Multigrid[0]);

            color packed = MultigridReduce(_PreferTl.Apply);

            vec2 pos = SimShader.unpack_vec2_corrected(packed);

            return pos + vec(0.5f, 0.75f);
        }

        public vec2 SelectedBound_BL, SelectedBound_TR;
        /// <summary>
        /// Warning: This is not network synchronized. Should only affect local clients fake selection field or the information should be communicated over the network.
        /// </summary>
        public void SelectedUnitsBounds()
        {
            BoundingTr.Apply(CurrentData, Output: Multigrid[0]);
            color bound_tr = MultigridReduce(_BoundingTr.Apply);

            BoundingBl.Apply(CurrentData, Output: Multigrid[0]);
            color bound_bl = MultigridReduce(_BoundingBl.Apply);

            SelectedBound_TR = SimShader.unpack_vec2(bound_tr);
            SelectedBound_BL = SimShader.unpack_vec2(bound_bl);
        }

        public void SelectInBox(vec2 bl, vec2 tr, bool Deselect, float Player, bool Fake = false)
        {
            ActionSelectInBox.Apply(CurrentData, CurrentUnits, bl, tr, Player, Deselect, Fake, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);

            ActionSelectInBox.Apply(PreviousData, PreviousUnits, bl, tr, Player, Deselect, Fake, Output: Temp1);
            Swap(ref Temp1, ref PreviousData);
        }

        public void SelectAlongLine(vec2 p1, vec2 p2, vec2 size, bool Deselect, bool Selecting, float Player, bool EffectSelection, bool Fake = false)
        {
            DataDrawMouse.Using(Assets.SelectCircle_Data, Player, Output: SelectField, Clear: Color.Transparent);

            if (Selecting)
            {
                vec2 shift = CellSpacing.FlipY();

                for (int i = 0; i <= 10; i++)
                {
                    float t = i / 10.0f;
                    var pos = t * p2 + (1 - t) * p1;
                    RectangleQuad.Draw(GraphicsDevice, pos - shift, size);
                }
            }

            if (EffectSelection)
            {
                SelectUnits(Player, Deselect, Fake);
            }
        }

        public void SelectInAreaViaRasterize(vec2 pos, vec2 size, bool Deselect, bool Selecting, float Player, bool EffectSelection, bool Fake = false)
        {
            DataDrawMouse.Using(Assets.SelectCircle_Data, Player, Output: SelectField, Clear: Color.Transparent);

            if (Selecting)
            {
                RectangleQuad.Draw(GraphicsDevice, pos, size);
            }

            if (EffectSelection)
            {
                SelectUnits(Player, Deselect, Fake);
            }
        }

        public void SelectInArea(vec2 pos, float radius, float Player)
        {
            float r2 = radius * radius;
            DataDrawMouseCircle.Apply(pos, r2, Player, CurrentData, Output: Temp1);
            Swap(ref Temp1, ref SelectField);
        }

        private void SelectUnits(float Player, bool Deselect, bool Fake = false)
        {
            ActionSelect.Apply(CurrentData, CurrentUnits, SelectField, Player, Deselect, Fake, Output: Temp1);
            Swap(ref Temp1, ref CurrentData);

            ActionSelect.Apply(PreviousData, PreviousUnits, SelectField, Player, Deselect, Fake, Output: Temp1);
            Swap(ref Temp1, ref PreviousData);
        }

        public void AttackMoveApply(float Player, vec2 Pos, vec2 Selected_BL, vec2 Selected_Size, vec2 Destination_BL, vec2 Destination_Size, float filter)
        {
            SetSelectedAction.Apply(CurrentData, CurrentUnits, SimShader.UnitAction.Attacking, Player, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            var ConversionRatio = Destination_Size / Selected_Size;
            ActionAttackSquare.Apply(CurrentData, CurrentUnits, TargetData, Destination_BL, Selected_BL, ConversionRatio, Player, filter, Output: Temp1);
            Swap(ref TargetData, ref Temp1);

            ActionAttack2.Apply(CurrentData, CurrentUnits, Extra, Pos, Player, filter, Output: Temp1);
            Swap(ref Extra, ref Temp1);
        }
    }
}
