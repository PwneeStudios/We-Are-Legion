using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
        void SubtractGold(int amount, int player)
        {
            PlayerInfo[player].Gold -= amount;
        }

        bool CanAffordBuilding(float building_type, int player)
        {
            var cost = Params.BuildingCost(building_type);

            return cost <= PlayerInfo[player].Gold;
        }
    }
}
