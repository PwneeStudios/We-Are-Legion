using FragSharpFramework;

namespace Game
{
    public partial class DyingDragonLordGridCoord : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<unit> CurrentUnits)
        {
            vec2 uv = vertex.TexCoords * CurrentUnits.Size;
            unit here = CurrentUnits[Here];

            if (here.anim == Anim.Die && here.type == UnitType.DragonLord)
            {
                return pack_vec2(uv);
            }
            else
            {
                return vec4.Zero;
            }
        }
    }

    public partial class DragonLordGridCoord : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<unit> CurrentUnits, [Player.Vals] float player)
        {
            vec2 uv = vertex.TexCoords * CurrentUnits.Size + vec(.5f, .5f);
            unit here = CurrentUnits[Here];

            if (here.player == player && here.type == UnitType.DragonLord)
            {
                return pack_vec2_corrected(uv);
            }
            else
            {
                return vec4.Zero;
            }
        }
    }

    /// <summary>
    /// Warning: This is not network synchronized. Should only affect local clients fake selection field or the information should be communicated over the network.
    /// </summary>
    public partial class BoundingTr : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data)
        {
            vec2 uv = vertex.TexCoords * Data.Size;

            return SomethingFakeSelected(Data[Here]) ? pack_vec2(uv) : vec(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Warning: This is not network synchronized. Should only affect local clients fake selection field or the information should be communicated over the network.
    /// </summary>
    public partial class BoundingBl : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data)
        {
            vec2 uv = vertex.TexCoords * Data.Size;

            return SomethingFakeSelected(Data[Here]) ? pack_vec2(uv) : vec(1, 1, 1, 1);
        }
    }

    public partial class _BoundingTr : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec2
                TL = unpack_vec2(PreviousLevel[Here]),
                TR = unpack_vec2(PreviousLevel[RightOne]),
                BL = unpack_vec2(PreviousLevel[UpOne]),
                BR = unpack_vec2(PreviousLevel[UpRight]);

            return pack_vec2(max(TL, TR, BL, BR));
        }
    }

    public partial class _BoundingBl : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            vec2
                TL = unpack_vec2(PreviousLevel[Here]),
                TR = unpack_vec2(PreviousLevel[RightOne]),
                BL = unpack_vec2(PreviousLevel[UpOne]),
                BR = unpack_vec2(PreviousLevel[UpRight]);

            return pack_vec2(min(TL, TR, BL, BR));
        }
    }

    public partial class _PreferTl : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> PreviousLevel)
        {
            if (PreviousLevel[Here] != vec4.Zero) return PreviousLevel[Here];
            if (PreviousLevel[RightOne] != vec4.Zero) return PreviousLevel[RightOne];
            if (PreviousLevel[UpOne] != vec4.Zero) return PreviousLevel[UpOne];
            if (PreviousLevel[UpRight] != vec4.Zero) return PreviousLevel[UpRight];

            return vec4.Zero;
        }
    }
}
