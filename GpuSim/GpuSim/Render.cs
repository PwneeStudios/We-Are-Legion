using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
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
            GameClass.Graphics.SetRenderTarget(null);
        }

        public static void StartText()
        {
            MySpriteBatch.Begin();
        }

        public static void EndText()
        {
            MySpriteBatch.End();
        }

        public static void DrawText(string text, vec2 pos)
        {
            MySpriteBatch.DrawString(DefaultFont, text, pos, Color.White);
        }

        public static void DrawText(string text, vec2 pos, color clr)
        {
            MySpriteBatch.DrawString(DefaultFont, text, pos, (Color)clr);
        }
	}
}
