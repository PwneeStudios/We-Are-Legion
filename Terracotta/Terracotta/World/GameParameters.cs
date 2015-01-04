using System;

namespace Terracotta
{
    public class BuildingParameters : SimShader
    {
        public int
            Cost, CostIncrease, GoldPerTick, JadePerTick;

        public BuildingParameters(int Cost, int CostIncrease = 0, int GoldPerTick = 0, int JadePerTick = 0)
        {
            this.Cost = Cost;
            this.CostIncrease = CostIncrease;
            this.GoldPerTick = GoldPerTick;
            this.JadePerTick = JadePerTick;
        }
    }

    public class GameParameters : SimShader
    {
        public BuildingParameters
            Barracks = new BuildingParameters(Cost: 250, CostIncrease: 50),
            GoldMine = new BuildingParameters(Cost: 500, CostIncrease: 100, GoldPerTick: 3),
            JadeMine = new BuildingParameters(Cost: 1000, CostIncrease: 200, JadePerTick: 3);

        public int
            StartGold = 750,
            StartJade = 10000;

        public BuildingParameters this[float type]
        {
            get
            {
                if (type == UnitType.Barracks) return Barracks;
                if (type == UnitType.GoldMine) return GoldMine;
                if (type == UnitType.JadeMine) return JadeMine;
                throw new Exception("Invalid building type.");
            }
        }
    }
}
