using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public class MarkerList
    {
        public bool Hide = false;

        List<Marker> Markers = new List<Marker>();

        public void Add(Marker marker)
        {
            Markers.Add(marker);
        }

        public void Draw(DrawOrder CurrentDrawOrder)
        {
            if (Hide) return;

            foreach (var marker in Markers) if (marker.MyDrawOrder == CurrentDrawOrder) marker.Draw();
        }

        public void Update()
        {
            if (Hide) return;

            foreach (var marker in Markers) marker.Update();
            Markers.RemoveAll(marker => marker.Dead);
        }
    }

    public enum DrawOrder { AfterTiles, AfterUnits, AfterMouse }

    public class Marker : BaseShader
    {
        public DrawOrder MyDrawOrder;

        float alpha;
        float dalpha_dt;
        
        vec2 pos, size, dsize_dt;

        float frame_length = 1;
        float t = 0;
        int frame = 0, frames = 1;

        RectangleQuad quad;
        Texture2D texture;

        World world;

        public Marker(World world, vec2 pos, vec2 size, Texture2D texture, float alpha_fade = -1, int frames = 1, float frame_length = .1f, DrawOrder DrawOrder = DrawOrder.AfterTiles, float alpha = 1, vec2 dsize_dt = default(vec2))
        {
            this.world = world;

            this.alpha = alpha;
            this.dalpha_dt = alpha_fade;
            
            this.pos = pos;
            this.size = size;
            this.dsize_dt = dsize_dt;

            this.frames = frames;
            this.frame_length = frame_length;

            quad = new RectangleQuad(pos - size / 2, pos + size / 2, vec2.Zero, vec(1.0f / frames, 1));
            this.texture = texture;

            this.MyDrawOrder = DrawOrder;
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
            float dt = (float)GameClass.DeltaT;

            alpha += dalpha_dt * dt;
            
            t += dt;
            frame = (int)(t / frame_length);
            if (frame >= frames) frame = frames - 1;

            size += dsize_dt * dt;
            quad.SetupVertices(pos - size / 2, pos + size / 2, vec(frame / (float)frames, 0), vec((frame + 1) / (float)frames, 1));
        }
    }
}
