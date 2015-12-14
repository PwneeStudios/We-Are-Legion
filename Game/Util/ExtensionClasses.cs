using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FragSharpFramework;
using FragSharpHelper;

namespace Game
{
    public static class Parse
    {
        public static int MaybeInt(this string s, int fallback = 0)
        {
            try
            {
                return int.Parse(s);
            }
            catch
            {
                return fallback;
            }
        }
    }

    public static class InputHelper
    {
        public static bool ShiftDown()
        {
            return Keys.LeftShift.Down() || Keys.RightShift.Down();
        }

        public static bool CtrlDown()
        {
            return Keys.LeftControl.Down() || Keys.RightControl.Down();
        }

        public static bool AltDown()
        {
            return Keys.LeftAlt.Down() || Keys.RightAlt.Down();
        }

        public static bool SomethingPressed()
        {
            return Input.CurKeyboard.GetPressedKeys().Length > 0 && Input.PrevKeyboard.GetPressedKeys().Length == 0 ||
                   Keys.Enter.Pressed() || Keys.Space.Pressed() || Keys.Escape.Pressed() ||
                   Input.LeftMousePressed;
        }

        public static bool SomethingDown()
        {
            return Input.CurKeyboard.GetPressedKeys().Length > 0 && Input.PrevKeyboard.GetPressedKeys().Length == 0 ||
                   Keys.Enter.Down() || Keys.Space.Down() || Keys.Escape.Down() ||
                   Input.LeftMouseDown;
        }
    }

    public enum Toggle { Off, On, Flip };
    public static class ToggleExtension
    {
        public static bool ToBool(this Toggle value)
        {
            if (value == Toggle.On) return true;
            if (value == Toggle.Off) return false;

            throw new Exception("Cannot cast value [" + value + "] to boolean; not true or false.");
        }
        public static bool Apply(this Toggle toggle, ref bool value)
        {
            value = toggle == Toggle.Flip ? !value : toggle.ToBool();
            return value;
        }
    }

    public static class RectangleQuadExtension
    {
        public static bool Contains(this RectangleQuad quad, vec2 pos)
        {
            return pos > quad.Bl && pos < quad.Tr;
        }
    }

    public static class RenderTarget2DExtension
    {
        public static void Clear(this RenderTarget2D RenderTarget)
        {
            GridHelper.GraphicsDevice.SetRenderTarget(RenderTarget);
            GridHelper.GraphicsDevice.Clear(Color.Transparent);
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

    public static class Texture2dExtension
    {
        public static bool CheckForNonZero(this Texture2D Texture)
        {
            var data = Texture.GetData();
            for (int i = 0; i < Texture.Width; i++)
            {
                for (int j = 0; j < Texture.Height; j++)
                {
                    var pixel = data[i * Texture.Height + j];
                    if (pixel.R != 0 || pixel.G != 0 || pixel.B != 0 || pixel.A != 0)
                    {
                        Console.WriteLine($"Non-zero value at ({i}, {j}) = {pixel}");
                        return true;
                    }
                }
            }

            return false;
        }

        public static vec2 UnitSize(this Texture2D Texture)
        {
            return new vec2(1, (float)Texture.Height / (float)Texture.Width);
        }

        public static vec2 Size(this Texture2D Texture)
        {
            return new vec2(Texture.Width, Texture.Height);
        }

        public static Texture2D PremultiplyAlpha(this Texture2D texture)
        {
            Color[] data = texture.GetData();
            
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(new Vector4(data[i].ToVector3() * (data[i].A / 255f), (data[i].A / 255f)));
            }

            texture.SetData(data);

            return texture;
        }

        static bool BoundsCheck(Rectangle rect, int w, int h)
        {
            return rect.Right <= w && rect.Bottom <= h && rect.Left >= 0 && rect.Top >= 0;
        }

        public static Color[] GetData(this Texture2D RenderTarget)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            Color[] data = new Color[w * h];
            
            RenderTarget.GetData(data);
            
            return data;
        }

        public static Color GetData(this Texture2D RenderTarget, vec2 coord)
        {
            var array = RenderTarget.GetData(coord, new vec2(1, 1));
            return array[0];
        }

        public static Color[] GetData(this Texture2D RenderTarget, vec2 coord, vec2 size)
        {
            int w = RenderTarget.Width, h = RenderTarget.Height;
            
            coord = new vec2((int)Math.Floor(coord.x), (int)Math.Floor(coord.y));
            size = new vec2((int)Math.Floor(size.x), (int)Math.Floor(size.y));
            if (coord.x < 0 || coord.y < 0 || coord.x >= w || coord.y >= h) return null;

            int elements = (int)size.x * (int)size.y;
            Color[] data = new Color[elements];
            Rectangle rect = new Rectangle((int)coord.x, (int)coord.y, (int)size.x, (int)size.y);
            
            if (!BoundsCheck(rect, w, h)) return null;
            
            RenderTarget.GetData(0, rect, data, 0, elements);

            return data;
        }

        public static T[] GetData<T>(this Texture2D RenderTarget, vec2 coord, vec2 size) where T : Convertible<vec4, T>
        {
            Color[] data = GetData(RenderTarget, coord, size);

            return ConvertArray<T>(data, size);
        }

        public static void SetData(this Texture2D RenderTarget, vec2 coord, vec2 size, Color[] data)
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
            if (data == null) return null;

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
