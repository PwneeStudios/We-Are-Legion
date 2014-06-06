using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FragSharpHelper;
using FragSharpFramework;

namespace GpuSim
{
    public static class Texture2DExtension
    {
        public static vec2 UnitSize(this Texture2D Texture)
        {
            return new vec2(1, (float)Texture.Height / (float)Texture.Width);
        }

        public static vec2 Size(this Texture2D Texture)
        {
            return new vec2(Texture.Width, Texture.Height);
        }
    }

    public static class ListExtension
    {
        public static void Swap<T>(this List<T> List, int Index, ref T NewElement)
        {
			T temp = List[Index];
            List[Index] = NewElement;
            NewElement = temp;
        }
    }

    public static class RenderTargetExtension
    {
        public static Color[] GetData(this RenderTarget2D RenderTarget)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            Color[] data = new Color[w * h];
            
            RenderTarget.GetData(data);
            
            return data;
        }

        public static Color[] GetData(this RenderTarget2D RenderTarget, vec2 coord)
        {
            return RenderTarget.GetData(coord, new vec2(1, 1));
        }

        public static Color[] GetData(this RenderTarget2D RenderTarget, vec2 coord, vec2 size)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            
            coord = new vec2((int)Math.Floor(coord.x), (int)Math.Floor(coord.y));
            size = new vec2((int)Math.Floor(size.x), (int)Math.Floor(size.y));
            if (coord.x < 0 || coord.y < 0 || coord.x >= w || coord.y >= h) return null;

            int elements = (int)size.x * (int)size.y;
            Color[] data = new Color[elements];
            Rectangle rect = new Rectangle((int)coord.x, (int)coord.y, (int)size.x, (int)size.y);
            RenderTarget.GetData(0, rect, data, 0, elements);

            return data;
        }

        public static T[] GetData<T>(this RenderTarget2D RenderTarget, vec2 coord, vec2 size) where T : Convertible<vec4, T>
        {
            Color[] data = GetData(RenderTarget, coord, size);

            return ConvertArray<T>(data, size);
        }

        public static void SetData(this RenderTarget2D RenderTarget, vec2 coord, vec2 size, Color[] data)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;

            coord = new vec2((int)Math.Floor(coord.x), (int)Math.Floor(coord.y));
            size = new vec2((int)Math.Floor(size.x), (int)Math.Floor(size.y));
            if (coord.x < 0 || coord.y < 0 || coord.x >= w || coord.y >= h) return;

            int elements = (int)size.x * (int)size.y;
            Rectangle rect = new Rectangle((int)coord.x, (int)coord.y, (int)size.x, (int)size.y);

            RenderTarget.SetData(0, rect, data, 0, elements);
        }

        public static T Convert<T>(Color val) where T : Convertible<vec4, T>
        {
            return default(T).ConvertFrom((vec4)(val.ToVector4()));
        }

        public static T[] ConvertArray<T>(Color[] data, vec2 size) where T : Convertible<vec4, T>
        {
            int 
                n = (int)Math.Floor(size.x),
                m = (int)Math.Floor(size.y);

            T[] t = new T[n * m];

            for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                t[i + n * j] = Convert<T>(data[i + n * j]);
            }

            return t;
        }
    }

	public static class RndExtension
	{
		public static float Bit(this System.Random rnd)
		{
			return rnd.NextDouble() > .5 ? 1 : 0;
		}

        public static int IntRange(this System.Random rnd, int min, int max)
        {
            return (int)(rnd.NextDouble() * (max - min) + min);
        }
	}
}
