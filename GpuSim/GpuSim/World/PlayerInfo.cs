namespace GpuSim
{
    public class PlayerInfo
    {
        public int
            Gold = 0,
            GoldMines = 0;

        public PlayerInfo(GameParameters Params)
        {
            Gold = Params.StartGold;
        }
    }
}
