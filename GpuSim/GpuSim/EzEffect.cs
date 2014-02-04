using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CoreEngine
{
    public class EzEffect
    {
        public Effect effect;
		public EffectParameter xTexture, xTexture2, drawTexture, xCameraAspect, xCameraPos, PercentSimStepComplete;
        public EffectTechnique Simplest;

		public EzEffect(ContentManager Content, string file)
		{
			effect = Content.Load<Effect>(file);
			
			xTexture		= effect.Parameters["xTexture"];
			xTexture2		= effect.Parameters["xTexture2"];
			drawTexture		= effect.Parameters["drawTexture"];
			xCameraPos		= effect.Parameters["xCameraPos"];
			xCameraAspect	= effect.Parameters["xCameraAspect"];

			PercentSimStepComplete = effect.Parameters["PercentSimStepComplete"];
		}

		public void Set(Texture2D texture, Texture2D texture2 = null)
		{
			Set(Vector2.Zero, 1, texture, texture2);
		}
		public void Set(Vector2 CameraPos, float CameraZoom, Texture2D texture, Texture2D texture2 = null)
		{
			xTexture.SetValue(texture);
			xTexture2.SetValue(texture2 == null ? texture : texture2);
			xCameraPos.SetValue(new Vector4(CameraPos.X, CameraPos.Y, CameraZoom, CameraZoom));
			xCameraAspect.SetValue(1f);
			effect.CurrentTechnique.Passes[0].Apply();
		}
    }
}