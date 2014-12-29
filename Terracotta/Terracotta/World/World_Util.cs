using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        public void SubtractGold(int amount, int player)
        {
            if (MapEditorActive) return;

            PlayerInfo[player].Gold -= amount;
        }

        bool CanAffordBuilding(float building_type, int player)
        {
            if (MapEditorActive) return true;

            var cost = Params.BuildingCost(building_type);

            return cost <= PlayerInfo[player].Gold;
        }

        public void SubtractJade(int amount, int player)
        {
            if (MapEditorActive) return;

            PlayerInfo[player].Jade -= amount;
        }

        bool CanAffordSpell(Spell spell, int player)
        {
            if (MapEditorActive) return true;

            return spell.JadeCost <= PlayerInfo[player].Jade;
        }
    }
}
