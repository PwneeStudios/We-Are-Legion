namespace Terracotta
{
    public class GameParameters : SimShader
    {
        public int
            BarracksCost = 250,
            GoldMineCost = 500,
            JadeMineCost = 1000,

            GoldPerBarracksPerTick = 0,
            GoldPerMinePerTick = 3,

            StartGold = 750;

        public int BuildingCost(float type)
        {
            if (type == UnitType.Barracks) return BarracksCost;
            if (type == UnitType.GoldMine) return GoldMineCost;
            if (type == UnitType.JadeMine) return JadeMineCost;

            return int.MaxValue;
        }
    }
}
