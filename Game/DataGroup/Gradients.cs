namespace Game
{
    public partial class DataGroup : SimShader
    {
        public void UpdateGradient_ToOtherTeams()
        {
            Pathfinding_ToOtherTeams.Apply(DistanceToOtherTeams, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToOtherTeams, ref Temp1);
        }

        public void UpdateGradient_ToPlayers()
        {
            Pathfinding_ToPlayers.Apply(DistanceToPlayers, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToPlayers, ref Temp1);
        }

        public void UpdateGradient_ToBuildings()
        {
            Pathfinding_ToSpecial.Apply(DistanceToBuildings, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToBuildings, ref Temp1);
        }
    }
}
