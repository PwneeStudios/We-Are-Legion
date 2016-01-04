namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public bool GameInputEnabled = true;
        public bool MinimapEnabled = true;
        public bool UnitDisplayEnabled = true;
        public void DisableGameInput()
        {
            GameInputEnabled = false;
            MinimapEnabled = false;
            UnitDisplayEnabled = false;
        }

        public void EnableGameInput()
        {
            GameInputEnabled = true;
            MinimapEnabled = true;
            UnitDisplayEnabled = true;
        }

        public bool MouseOverHud = false;
        public void OnMouseLeave()
        {
            MouseOverHud = false;
            //Console.WriteLine(MouseOverHud);
        }

        public void OnMouseOver()
        {
            MouseOverHud = true;
            //Console.WriteLine(MouseOverHud);
        }
    }
}
