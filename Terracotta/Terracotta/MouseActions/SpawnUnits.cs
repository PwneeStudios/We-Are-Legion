using System;
using FragSharpFramework;

namespace Terracotta
{
    public class UnitDistribution
    {
        [FragSharpFramework.Vals(Full, EveryOther)]
        public class ValsAttribute : Attribute { }

        public static readonly float[] Vals = new float[] { Full, EveryOther };

        public const float
            Full = 1,
            EveryOther = 2;

        public static bool Contains(float distribution, vec2 v)
        {
            if (distribution == Full)
            {
                return true;
            }

            if (distribution == EveryOther)
            {
                return (int)(v.x) % 2 == 0 && (int)(v.y) % 2 == 0;
            }

            return false;
        }
    }

    public partial class ActionSpawn_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<data> Select, [UnitDistribution.Vals] float distribution)
        {
            data here = Data[Here];
            data select = Select[Here];

            if (Something(select) && !Something(here))
            {
                if (UnitDistribution.Contains(distribution, vertex.TexCoords * Select.Size))
                {
                    here.direction = Dir.Right;
                    here.action = UnitAction.Guard;
                }
            }

            return here;
        }
    }

    public partial class ActionSpawn_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, Field<data> Select, float player, float team, float type, [UnitDistribution.Vals] float distribution)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            data select = Select[Here];

            if (Something(select) && !Something(data_here))
            {
                if (UnitDistribution.Contains(distribution, vertex.TexCoords * Select.Size))
                {
                    unit_here.player = player;
                    unit_here.team = team;
                    unit_here.type = type;
                }
            }

            return unit_here;
        }
    }

    public partial class ActionSpawn_Target : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<vec4> Target, Field<data> Select, [UnitDistribution.Vals] float distribution)
        {
            data data_here = Data[Here];
            data select = Select[Here];

            vec4 target = Target[Here];

            if (Something(select) && !Something(data_here))
            {
                if (UnitDistribution.Contains(distribution, vertex.TexCoords * Select.Size))
                {
                    vec2 pos = vertex.TexCoords * Data.Size;
                    target = pack_vec2(pos);
                }
            }

            return target;
        }
    }    
}
