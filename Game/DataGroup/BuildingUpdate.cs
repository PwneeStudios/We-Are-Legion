namespace Game
{
    public partial class DataGroup : SimShader
    {
        public void Building_InfusionDiffusion()
        {
            BuildingInfusion_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Target.Apply(CurrentUnits, CurrentData, TargetData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
        }

        /// <summary>
        /// Warning: This is not network synchronized. Should only affect local clients fake selection field.
        /// </summary>
        public void Building_FakeSelectionSpread()
        {
            BuildingInfusion_Selection.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Selection.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
        }
    }
}
