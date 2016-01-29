using System;

using Microsoft.Xna.Framework.Graphics;
using FragSharpFramework;

namespace Game
{
    public partial class DataGroup : SimShader
    {
        public int[] UnitCount = new int[] { 0, 0, 0, 0, 0 };
        public int[] BarracksCount = new int[] { 0, 0, 0, 0, 0 };
        public int SelectedUnits = 0, UnitCountUi = 0;

        public void DoGoldMineCount(PlayerInfo[] PlayerInfo)
        {
            CountUnitTypeForAllPlayers.Apply(CurrentData, CurrentUnits, UnitType.GoldMine, Output: Multigrid[0]);

            var count = (PlayerTuple)MultigridReduce(CountReduce_4x1byte.Apply);

            PlayerInfo[1][UnitType.GoldMine].Count = Int(count.PlayerOne);
            PlayerInfo[2][UnitType.GoldMine].Count = Int(count.PlayerTwo);
            PlayerInfo[3][UnitType.GoldMine].Count = Int(count.PlayerThree);
            PlayerInfo[4][UnitType.GoldMine].Count = Int(count.PlayerFour);

            Render.UnsetDevice();
        }

        public void DoJadeMineCount(PlayerInfo[] PlayerInfo)
        {
            CountUnitTypeForAllPlayers.Apply(CurrentData, CurrentUnits, UnitType.JadeMine, Output: Multigrid[0]);

            var count = (PlayerTuple)MultigridReduce(CountReduce_4x1byte.Apply);

            PlayerInfo[1][UnitType.JadeMine].Count = Int(count.PlayerOne);
            PlayerInfo[2][UnitType.JadeMine].Count = Int(count.PlayerTwo);
            PlayerInfo[3][UnitType.JadeMine].Count = Int(count.PlayerThree);
            PlayerInfo[4][UnitType.JadeMine].Count = Int(count.PlayerFour);
        }

        public void DoDragonLordCount(PlayerInfo[] PlayerInfo)
        {
            CountUnitTypeForAllPlayers.Apply(CurrentData, CurrentUnits, UnitType.DragonLord, Output: Multigrid[0]);

            var count = (PlayerTuple)MultigridReduce(CountReduce_4x1byte.Apply);

            PlayerInfo[1].DragonLords = Int(count.PlayerOne);
            PlayerInfo[2].DragonLords = Int(count.PlayerTwo);
            PlayerInfo[3].DragonLords = Int(count.PlayerThree);
            PlayerInfo[4].DragonLords = Int(count.PlayerFour);
        }

        public bool[]
            UnitSummary = new bool[Int(UnitType.Last)],
            PrevUnitSummary = new bool[Int(UnitType.Last)];

        public void DoUnitSummary(float player, bool only_selected)
        {
            Swap(ref UnitSummary, ref PrevUnitSummary);

            DoUnitSummary_1.Apply(CurrentData, CurrentUnits, player, only_selected, Output: Multigrid[0]);
            CopySummary(0);

            DoUnitSummary_2.Apply(CurrentData, CurrentUnits, player, only_selected, Output: Multigrid[0]);
            CopySummary(4);

            var selected = DoTotalUnitCount(player, true);
            SelectedUnits = selected;
            UnitCountUi = SelectedUnits;
        }

        void CopySummary(int offset)
        {
            vec4 count = MultigridReduce(CountReduce_4x1byte.Apply);
            for (int i = 0; i < 4; i++) UnitSummary[i + offset] = (count[i] > 0);
        }

        public Tuple<int, int> DoUnitCount(float player, bool only_selected)
        {
            CountUnits.Apply(CurrentData, CurrentUnits, player, only_selected, Output: Multigrid[0]);

            color count = MultigridReduce(CountReduce_3byte1byte.Apply);

            int unit_count = (int)(SimShader.unpack_val(count.xyz) + .5f);
            int barracks_count = Int(count.w);
            
            return new Tuple<int,int>(unit_count, barracks_count);
        }

        public int DoTotalUnitCount(float player, bool only_selected)
        {
            CountAllUnits.Apply(CurrentData, CurrentUnits, player, only_selected, Output: Multigrid[0]);

            color count = MultigridReduce(CountReduce_3byte1byte.Apply);

            return (int)(SimShader.unpack_val(count.xyz) + .5f);
        }

        public ActionCount DoActionCount(World world)
        {
            CountMovingAttackingDyingExploding.Using(CurrentData, CurrentUnits, Output: Multigrid[0]);

            world.DrawVisibleGrid(scale:1.5f);

            ActionCount count = (ActionCount)MultigridReduce(CountReduce_4x1byte.Apply);

            //Console.WriteLine("moving: {0}, attacking: {1}, dying: {2}, exploding: {3})", count.UnitsMoving, count.UnitsAttacking, count.UnitsDying, count.BuildingsExploding);

            return count;
        }
        
        public delegate void HashFunc(Texture2D F, Texture2D Noise, RenderTarget2D Output);
        public string DoHash(RenderTarget2D input) { return DoHash(input, Hash.Apply); }
        public string DoHash(RenderTarget2D input, HashFunc hash_func)
        {
            hash_func(input, HashField, Output: Multigrid[0]);

            vec4 hash = MultigridReduce((tx, rt) => HashReduce.Apply(tx, HashField, rt));

            string s = string.Format("{0}{1}{2}{3}", hash.x, hash.y, hash.z, hash.w);
            return s.GetHashCode().ToString();
        }
    }
}
