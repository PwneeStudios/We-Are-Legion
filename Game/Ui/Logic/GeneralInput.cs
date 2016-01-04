namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        public bool GameInputEnabled = true;
        public bool MinimapEnabled = true;
        public bool UnitDisplayEnabled = true;
        void DisableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = false;
            MinimapEnabled = false;
            UnitDisplayEnabled = false;
        }

        void EnableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = true;
            MinimapEnabled = true;
            UnitDisplayEnabled = true;
        }

        public bool MouseOverHud = false;
        void OnMouseLeave(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = false;
            //Console.WriteLine(MouseOverHud);
        }

        void OnMouseOver(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = true;
            //Console.WriteLine(MouseOverHud);
        }
    }
}
