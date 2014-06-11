using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    public partial class DataGroup : SimShader
    {
        public int[] UnitCount = new int[] { 0, 0, 0, 0, 0 };
        public int SelectedCount = 0;

        public void DoGoldMineCount(PlayerInfo[] PlayerInfo)
        {
            CountGoldMines.Apply(CurrentData, CurrentUnits, Output: Multigrid[0]);

            color count = MultigridReduce(CountReduce_4x1byte.Apply);

            PlayerInfo[1].GoldMines = (int)(255 * count.x + .5f);
            PlayerInfo[2].GoldMines = (int)(255 * count.y + .5f);
            PlayerInfo[3].GoldMines = (int)(255 * count.z + .5f);
            PlayerInfo[4].GoldMines = (int)(255 * count.w + .5f);
        }

        public int DoUnitCount(float player, bool only_selected)
        {
            CountUnits.Apply(CurrentData, CurrentUnits, player, only_selected, Output: Multigrid[0]);

            color count = MultigridReduce(CountReduce_3byte.Apply);

            int result = (int)(SimShader.unpack_coord(count.xyz) + .5f);
            return result;
        }
    }
}
