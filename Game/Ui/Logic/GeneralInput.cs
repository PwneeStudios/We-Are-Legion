using Awesomium.Core;

namespace Game
{
    public partial class GameClass : Microsoft.Xna.Framework.Game
    {
        void BindMethods_GeneralInput()
        {
            xnaObj.Bind("OnMouseOver", OnMouseOver);
            xnaObj.Bind("OnMouseLeave", OnMouseLeave);
            xnaObj.Bind("EnableGameInput", EnableGameInput);
            xnaObj.Bind("DisableGameInput", DisableGameInput);
        }

        public bool GameInputEnabled = true;
        public bool MinimapEnabled = true;
        public bool UnitDisplayEnabled = true;
        JSValue DisableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = false;
            MinimapEnabled = false;
            UnitDisplayEnabled = false;
            return JSValue.Null;
        }

        JSValue EnableGameInput(object sender, JavascriptMethodEventArgs e)
        {
            GameInputEnabled = true;
            MinimapEnabled = true;
            UnitDisplayEnabled = true;
            return JSValue.Null;
        }

        public bool MouseOverHud = false;
        JSValue OnMouseLeave(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = false;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }

        JSValue OnMouseOver(object sender, JavascriptMethodEventArgs e)
        {
            MouseOverHud = true;
            //Console.WriteLine(MouseOverHud);
            return JSValue.Null;
        }
    }
}
