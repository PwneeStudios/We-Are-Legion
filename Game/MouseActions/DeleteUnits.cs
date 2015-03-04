using FragSharpFramework;

namespace Game
{
    public partial class ActionDelete_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data)
        {
            data here = Data[Here];

            if (SomethingSelected(here))
            {
                return data.Nothing;
            }

            return here;
        }
    }
}
