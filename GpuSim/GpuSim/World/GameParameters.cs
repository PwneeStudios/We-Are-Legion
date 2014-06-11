namespace GpuSim
{
    public class GameParameters : SimShader
    {
        public int
            BarracksCost = 2500,
            GoldMineCost = 5000,

            StartGold = 7500;

        public int BuildingCost(float type)
        {
            if (type == UnitType.Barracks) return BarracksCost;
            if (type == UnitType.GoldMine) return GoldMineCost;
            return int.MaxValue;
        }
    }
}
