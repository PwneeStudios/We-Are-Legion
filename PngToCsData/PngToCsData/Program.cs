using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PngToCsData
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = new string[] {
                @"C:/Users/Jordan/Desktop/Dir/Pwnee/Games/Terracotta/Terracotta/Terracotta/Terracotta/Drawing/GameColors.cs",
                @"C:/Users/Jordan/Desktop/Dir/Pwnee/Games/Terracotta/Terracotta/Terracotta/TerracottaContent/Art/FarColors.png"
            };
#endif

            string CsPath = args[0];
            string PngPath = args[1];

            var cs = File.ReadAllText(CsPath);

            var img = new Bitmap(Image.FromFile(PngPath));
            for (int i = 0; i < img.Width; i++)
            for (int j = 0; j < img.Height; j++)
            {
                var var_name = string.Format("__{0}_{1}", i, j);
                int start = cs.IndexOf("0x", cs.IndexOf(var_name));
                int end = cs.IndexOf(")", start);

                var pixel = img.GetPixel(i, j);
                string new_val = string.Format("0x{0}{1}{2}, {3}f", pixel.R.ToString("X2"), pixel.G.ToString("X2"), pixel.B.ToString("X2"), (pixel.A / 255.0f).ToString());

                cs = cs.Remove(start, end - start);
                cs = cs.Insert(start, new_val);
            }

            File.WriteAllText(CsPath, cs);
        }
    }
}
