using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        void SubtractGold(int amount, int player)
        {
            if (MapEditor) return;

            PlayerInfo[player].Gold -= amount;
        }

        bool CanAffordBuilding(float building_type, int player)
        {
            if (MapEditor) return true;

            var cost = Params.BuildingCost(building_type);

            return cost <= PlayerInfo[player].Gold;
        }
    }
}
