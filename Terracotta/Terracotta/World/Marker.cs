using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Terracotta
{
    public class MarkerList
    {
        List<Marker> Markers = new List<Marker>();

        public void Add(Marker marker)
        {
            Markers.Add(marker);
        }

        public void Draw()
        {
            foreach (var marker in Markers) marker.Draw();
        }

        public void Update()
        {
            foreach (var marker in Markers) marker.Update();
            Markers.RemoveAll(marker => marker.Dead);
        }
    }

    public class Marker : BaseShader
    {
        float alpha;
        float alpha_fade;

        float frame_length = 1;
        float t = 0;
        int frame = 0, frames = 1;

        RectangleQuad quad;
        Texture2D texture;

        World world;

        public Marker(World world, vec2 pos, vec2 size, Texture2D texture, float alpha_fade, int frames = 1, float frame_length = .1f)
        {
            this.world = world;

            alpha = 1;
            this.alpha_fade = alpha_fade;

            this.frames = frames;
            this.frame_length = frame_length;

            quad = new RectangleQuad(pos - size / 2, pos + size / 2, vec2.Zero, vec(1.0f / frames, 1));
            this.texture = texture;
        }

        public bool Dead { get { return alpha <= 0; } }

        public void Draw()
        {
            DrawTexture.Using(world.camvec, world.CameraAspect, texture);
            quad.SetColor(new color(1, 1, 1, alpha));
            quad.Draw(GameClass.Game.GraphicsDevice);
        }

        public void Update()
        {
            float delta_t = (float)GameClass.Time.ElapsedGameTime.TotalSeconds;

            alpha += delta_t * alpha_fade;
            
            t += delta_t;
            frame = (int)(t / frame_length);
            if (frame >= frames) frame = frames;

            quad.SetupUv(vec(frame / (float)frames, 0), vec((frame + 1) / (float)frames, 1));
        }
    }
}
