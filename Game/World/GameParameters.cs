using System;
using System.Collections.Generic;

namespace Game
{
    public class BuildingParameters : SimShader
    {
        public int
            Cost, CostIncrease, GoldPerTick, JadePerTick;

        public float
            UnitType;

        public string
            Name;

        public BuildingParameters(float UnitType, string Name, int Cost, int CostIncrease = 0, int GoldPerTick = 0, int JadePerTick = 0)
        {
            this.UnitType = UnitType;
            this.Name = Name;

            this.Cost = Cost;
            this.CostIncrease = CostIncrease;
            this.GoldPerTick = GoldPerTick;
            this.JadePerTick = JadePerTick;
        }
    }

    public class GameParameters : SimShader
    {
        public Dictionary<string, BuildingParameters> Buildings = new Dictionary<string, BuildingParameters>();

        public BuildingParameters
            Barracks = new BuildingParameters(UnitType.Barracks, "Barracks", Cost: 250, CostIncrease: 50),
            GoldMine = new BuildingParameters(UnitType.GoldMine, "GoldMine", Cost: 500, CostIncrease: 100, GoldPerTick: 3),
            JadeMine = new BuildingParameters(UnitType.JadeMine, "JadeMine", Cost: 1000, CostIncrease: 200, JadePerTick: 3);

        public int
            StartGold = 750,
            StartJade = 10000;

        public GameParameters()
        {
            Add(Barracks, GoldMine, JadeMine);
        }

        void Add(params BuildingParameters[] buildings)
        {
            foreach (var building in buildings)
            {
                Buildings.Add(building.Name, building);
            }
        }

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
