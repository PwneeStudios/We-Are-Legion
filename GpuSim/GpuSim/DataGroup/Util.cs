using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FragSharpFramework;

namespace GpuSim
{
    public partial class DataGroup : SimShader
    {
        Color[] ReducedData = new Color[1];
        color MultigridReduce(Action<Texture2D, RenderTarget2D> ReductionShader)
        {
            int n = Multigrid[0].Width;
            int level = 0;
            while (n >= 2)
            {
                ReductionShader(Multigrid[level], Multigrid[level + 1]);

                n /= 2;
                level++;
            }
            GraphicsDevice.SetRenderTarget(null);

            Multigrid.Last().GetData(ReducedData);
            return (color)ReducedData[0];
        }

        void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
