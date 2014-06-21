using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    [Flags]
    public enum Alignment
    {
        Center  = 0,
        
        LeftJusitfy = Left | Top,
        RightJusitfy = Right | Top, 

        Left    = 1 << 0,
        Right   = 1 << 1,
        Top     = 1 << 2,
        Bottom  = 1 << 3
    }

    public static class Render
    {
        static SpriteBatch MySpriteBatch;
        static SpriteFont DefaultFont;

        public static void Initialize()
        {
            MySpriteBatch = new SpriteBatch(GameClass.Graphics);
            DefaultFont = GameClass.ContentManager.Load<SpriteFont>("Default");
        }

        public static void StandardRenderSetup()
        {
            GameClass.Graphics.RasterizerState = RasterizerState.CullNone;
            GameClass.Graphics.BlendState = BlendState.AlphaBlend;
            GameClass.Graphics.DepthStencilState = DepthStencilState.DepthRead;
        }

        public static void UnsetDevice()
        {
            GameClass.Graphics.Textures[0] = null;
            GameClass.Graphics.Textures[1] = null;
            GameClass.Graphics.Textures[2] = null;
            GameClass.Graphics.Textures[3] = null;
            GameClass.Graphics.Textures[4] = null;
            GameClass.Graphics.Textures[5] = null;
            GameClass.Graphics.SetRenderTarget(null);
        }

        static bool TextStarted = false;
        public static void StartText()
        {
            if (TextStarted) return;

            TextStarted = true;
            MySpriteBatch.Begin();
        }

        public static void EndText()
        {
            if (!TextStarted) return;

            TextStarted = false;
            MySpriteBatch.End();
        }

        public static void DrawText(string text, vec2 pos, Alignment align = Alignment.LeftJusitfy)
        {
            DrawText(DefaultFont, text, pos, align, new color(1f, 1f, 1f, 1f));
        }

        public static void DrawText(string text, vec2 pos, color clr, Alignment align = Alignment.LeftJusitfy)
        {
            DrawText(DefaultFont, text, pos, align, clr);
        }

        public static void DrawText(SpriteFont font, string text, vec2 pos, Alignment align, color clr)
        {
            vec2 size = (vec2)font.MeasureString(text);
            vec2 origin = size * 0.5f;

            if (align.HasFlag(Alignment.Left))
                origin.x -= size.x / 2;

            if (align.HasFlag(Alignment.Right))
                origin.x += size.x / 2;

            if (align.HasFlag(Alignment.Top))
                origin.y -= size.y / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.y += size.y / 2;

            MySpriteBatch.DrawString(font, text, pos, (Color)clr.Premultiplied, 0, origin, 1, SpriteEffects.None, 0);
        }
    }
}
