using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace Game
{
    public partial class DataGroup : SimShader
    {
        Color[] ReducedData = new Color[1];
        public color MultigridReduce(Action<Texture2D, RenderTarget2D> ReductionShader, bool Debug=false)
        {
            int n = Multigrid[0].Width;
            int level = 0;
            while (n >= 2)
            {
                ReductionShader(Multigrid[level], Multigrid[level + 1]);

                if (Debug)
                {
                    Console.WriteLine($"Did multigrid reduce from level {level} to level {level + 1}, n == {n}");
                    Multigrid[level].CheckForNonZero();
                    var result = Multigrid[level + 1].CheckForNonZero();

                    if (n == 2 && result)
                    {
                        Console.WriteLine("Made it to the end of the line!");
                    }
                }

                n /= 2;
                level++;
            }
            GraphicsDevice.SetRenderTarget(null);

            Multigrid.Last().GetData(ReducedData);
            return (color)ReducedData[0];
        }

        public void CopyFromTo(RenderTarget2D Source, ref RenderTarget2D Destination)
        {
            Identity.CopyFromTo(Source, ref Destination, ref Temp1);
        }

        public void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
