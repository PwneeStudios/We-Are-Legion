namespace Game
{
    public partial class DataGroup : SimShader
    {
        public void EditorSimulationUpdate()
        {
            // Pathfinding
            Movement_UpdateDirection_RemoveDead.Apply(TargetData, CurrentUnits, Extra, CurrentData, PreviousData, DistanceToOtherTeams, RandomField, Magic,
                                                      Geo, AntiGeo, Dirward[Dir.Right], Dirward[Dir.Left], Dirward[Dir.Up], Dirward[Dir.Down],
                                                      Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
        }

        public void PausedSimulationUpdate()
        {
            UpdateGradient_ToOtherTeams();
            UpdateGradient_ToOtherTeams();

            UpdateGradient_ToPlayers();
            UpdateGradient_ToPlayers();

            UpdateGradient_ToBuildings();

            Building_InfusionDiffusion();
        }

        public void SimulationUpdate()
        {
            // Spawning
            SpawnUnits.Apply(CurrentUnits, CurrentData, PreviousData, RandomField, Magic, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);
            SetSpawn_Unit.Apply(CurrentUnits, CurrentData, Magic, GameClass.World.PlayerTeamVals, Output: Temp1);
            Swap(ref CurrentUnits, ref Temp1);
            SetSpawn_Target.Apply(TargetData, CurrentData, RandomField, Magic, Output: Temp1);
            Swap(ref TargetData, ref Temp1);
            SetSpawn_Data.Apply(CurrentUnits, CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

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
        }

        public void UpdateSelect()
        {
            // Building data spread
            Building_InfusionDiffusion();

            // Update fake selection
            UpdateFakeSelect.Apply(CurrentData, Output: Temp1);
            Swap(ref CurrentData, ref Temp1);

            UpdateIcons();
        }

        public void UpdateIcons()
        {
            for (int i = 0; i < 5; i++) UpdateGradient_ToBuildings();
        }

        public void UpdateRnd()
        {
            UpdateRandomField.Apply(RandomField, Output: Temp1);
            Swap(ref RandomField, ref Temp1);
        }

        public void UpdateMagicFields()
        {
            UpdateMagic.Apply(Magic, CurrentData, PreviousData, Corpses, Necromancy, Output: Temp1);
            CoreMath.Swap(ref Temp1, ref Magic);
        }

        public void UpdateMagicAuras()
        {
            PropagateNecromancyAuro.Apply(Necromancy, CurrentData, CurrentUnits, Output: Temp1);
            CoreMath.Swap(ref Temp1, ref Necromancy);

            PropagateAntiMagicAuro.Apply(AntiMagic, CurrentData, CurrentUnits, Output: Temp1);
            CoreMath.Swap(ref Temp1, ref AntiMagic);
        }

        public void UpdateGradients()
        {
            UpdateGradient_ToOtherTeams();
            UpdateGradient_ToOtherTeams();

            UpdateGradient_ToPlayers();
            UpdateGradient_ToPlayers();
        }
    }
}
