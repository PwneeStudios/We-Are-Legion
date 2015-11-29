using System.Collections.Generic;
using FragSharpFramework;

namespace Game
{
    public class Ui : SimShader
    {
        public Ui()
        {
            ActiveUi = this;
        }

        public Dictionary<string, RectangleQuad> Elements = new Dictionary<string, RectangleQuad>();
        public List<RectangleQuad> Order = new List<RectangleQuad>();

        public void Add(string name, RectangleQuad e)
        {
            e.Texture = Assets.White;
            Elements.Add(name, e);

            if (!name.Contains("[Text]"))
            {
                Order.Add(e);
            }
        }

        public void Draw()
        {
            foreach (var e in Order)
            {
                DrawElement(e);
            }
        }

        // Static

        public static Ui ActiveUi;
        public static RectangleQuad e;

        public static void Element(string name)
        {
            if (Ui.ActiveUi.Elements.ContainsKey(name))
            {
                Ui.e = Ui.ActiveUi.Elements[name];
                return;
            }

            var e = new RectangleQuad(vec2.Zero, vec2.Zero, vec2.Zero, vec2.Ones);

            Ui.ActiveUi.Add(name, e);
            Ui.e = e;
        }

        static void DrawElement(RectangleQuad e)
        {
            DrawTextureSmooth.Using(vec(0, 0, 1, 1), GameClass.ScreenAspect, e.Texture);
            e.Draw(GameClass.Graphics);
        }
    }
}
