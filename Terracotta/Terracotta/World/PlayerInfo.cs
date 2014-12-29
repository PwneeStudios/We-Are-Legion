namespace Terracotta
{
    public class PlayerInfo
    {
        public int
            Gold = 0,
            GoldMines = 0,
            Jade = 0,
            JadeMines = 0;

        public PlayerInfo(GameParameters Params)
        {
            Gold = Params.StartGold;
            Jade = Params.StartJade;
        }
    }
}
