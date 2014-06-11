using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace GpuSim
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
            Markers.RemoveAll(marker => marker.alpha <= 0);
        }
    }

    public class Marker
    {
        public float alpha;
        float alpha_fade;

        RectangleQuad quad;
        Texture2D texture;

        World world;

        public Marker(World world, vec2 pos, vec2 size, Texture2D texture, float alpha_fade)
        {
            this.world = world;

            alpha = 1;
            this.alpha_fade = alpha_fade;

            quad = new RectangleQuad(pos - size / 2, pos + size / 2, vec2.Zero, vec2.Ones);
            this.texture = texture;
        }

        public void Draw()
        {
            DrawTexture.Using(world.camvec, world.CameraAspect, texture);
            quad.SetColor(new color(1, 1, 1, alpha));
            quad.Draw(GameClass.Game.GraphicsDevice);
        }

        public void Update()
        {
            alpha += (float)GameClass.Time.ElapsedGameTime.TotalSeconds * alpha_fade;
        }
    }
}
