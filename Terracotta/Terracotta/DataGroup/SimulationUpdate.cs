namespace Terracotta
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


            Building_InfusionDiffusion();


            // Corpses
            AddCorpses.Apply(CurrentUnits, CurrentData, Corpses, Magic, Output: Temp1);
            Swap(ref Corpses, ref Temp1);


            // Pathfinding
            Movement_UpdateDirection_RemoveDead.Apply(TargetData, CurrentUnits, Extra, CurrentData, PreviousData, DistanceToOtherTeams, RandomField, Magic,
                                                      Geo, AntiGeo, Dirward[Dir.Right], Dirward[Dir.Left], Dirward[Dir.Up], Dirward[Dir.Down],
                                                      Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            Movement_SetPolarity_Phase1.Apply(CurrentData, Extra, Geo, AntiGeo, Output: Temp1);
            Swap(ref Extra, ref Temp1);
            Movement_SetPolarity_Phase2.Apply(CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            // Movement execution
            Movement_Phase1.Apply(CurrentData, RandomField, Output: Temp1);
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

            // Attacking
            CheckForAttacking.Apply(CurrentUnits, CurrentData, RandomField, Magic, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);

            // Magic
            UpdateMagic.Apply(Magic, CurrentData, PreviousData, Corpses, Necromancy, Output: Temp1);
            CoreMath.Swap(ref Temp1, ref Magic);

            PropagateNecromancyAuro.Apply(Necromancy, CurrentData, CurrentUnits, Output: Temp1);
            CoreMath.Swap(ref Temp1, ref Necromancy);

            // Spawning
            SpawnUnits.Apply(CurrentUnits, CurrentData, PreviousData, RandomField, Magic, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            SetSpawn_Unit.Apply(CurrentUnits, CurrentData, Magic, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            SetSpawn_Target.Apply(TargetData, CurrentData, RandomField, Magic, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            SetSpawn_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            // Random field
            UpdateRandomField.Apply(RandomField, Output: Temp1);
            Swap(ref RandomField, ref Temp1);
        }
    }
}
