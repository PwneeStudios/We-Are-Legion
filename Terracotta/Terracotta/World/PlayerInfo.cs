namespace Terracotta
{
    public class PlayerInfo
    {
        public int
            Gold = 0,
            GoldMines = 0,
            Jade = 0,
            JadeMines = 0,
            DragonLords = -1;

        public bool DragonLordAlive = false;

        public PlayerInfo(GameParameters Params)
        {
            Gold = Params.StartGold;
            Jade = Params.StartJade;
        }

        public override string ToString()
        {
            return string.Format("Gold {0}({1}), Jade {2}({3}), DragonLords {4}", Gold, GoldMines, Jade, JadeMines, DragonLords);
        }
    }
}
