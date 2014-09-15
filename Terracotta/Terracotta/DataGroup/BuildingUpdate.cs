namespace Terracotta
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

        public void Building_SelectionSpread()
        {
            BuildingInfusion_Selection.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Selection.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
        }
    }
}
