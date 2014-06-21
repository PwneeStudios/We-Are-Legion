namespace GpuSim
{
    public partial class DataGroup : SimShader
    {
        public void SimulationUpdate()
        {
            UpdateGradient_ToOtherTeams();
            UpdateGradient_ToOtherTeams();

            UpdateGradient_ToPlayers();
            UpdateGradient_ToPlayers();

            UpdateGradient_ToBuildings();


            BuildingInfusion_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            BuildingDiffusion_Target.Apply(CurrentUnits, CurrentData, TargetData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);


            AddCorpses.Apply(CurrentUnits, CurrentData, Corspes, Output: Temp1);
            Swap(ref Corspes, ref Temp1);


            Movement_UpdateDirection_RemoveDead.Apply(TargetData, CurrentUnits, Extra, CurrentData, DistanceToOtherTeams, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            Movement_Phase1.Apply(CurrentData, Output: Temp1);
            Movement_Phase2.Apply(CurrentData, Temp1, Output: Temp2);

            Swap(ref CurrentData, ref PreviousData);
            Swap(ref Temp2, ref CurrentData);

            Movement_Convect.Apply(TargetData, CurrentData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            Movement_Convect.Apply(Extra, CurrentData, Output: Temp1);
            Swap(ref Extra, ref Temp1);
            Movement_Convect.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            Swap(ref PreviousUnits, ref Temp1);


            CheckForAttacking.Apply(CurrentUnits, CurrentData, RandomField, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);


            SpawnUnits.Apply(CurrentUnits, CurrentData, PreviousData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            SetSpawn_Unit.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            SetSpawn_Target.Apply(TargetData, CurrentData, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            SetSpawn_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);


            UpdateRandomField.Apply(RandomField, Output: Temp1);
            Swap(ref RandomField, ref Temp1);
        }
    }
}
