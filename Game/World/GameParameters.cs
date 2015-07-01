using System;
using System.Collections.Generic;

namespace Game
{
    public class BuildingParameters : SimShader
    {
        public int
            GoldCost, CostIncrease, GoldPerTick, JadePerTick, CurrentGoldCost;

        public float
            UnitType;

        public string
            Name;

        public BuildingParameters(float UnitType, string Name, int GoldCost, int CostIncrease = 0, int GoldPerTick = 0, int JadePerTick = 0)
        {
            this.UnitType = UnitType;
            this.Name = Name;

            this.GoldCost = GoldCost;
            this.CostIncrease = CostIncrease;
            this.GoldPerTick = GoldPerTick;
            this.JadePerTick = JadePerTick;
        }
    }

    public class GameParameters : SimShader
    {
        public Dictionary<string, BuildingParameters> Buildings = new Dictionary<string, BuildingParameters>();

        public BuildingParameters
            Barracks = new BuildingParameters(UnitType.Barracks, "Barracks", GoldCost: 250, CostIncrease: 50),
            GoldMine = new BuildingParameters(UnitType.GoldMine, "GoldMine", GoldCost: 250, CostIncrease: 250, GoldPerTick: 3),
            JadeMine = new BuildingParameters(UnitType.JadeMine, "JadeMine", GoldCost: 1000, CostIncrease: 500, JadePerTick: 3);

        public int
            StartGold = 1250,
            StartJade = 500;

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
