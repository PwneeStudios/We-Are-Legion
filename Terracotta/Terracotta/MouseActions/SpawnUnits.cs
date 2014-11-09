using FragSharpFramework;

namespace Terracotta
{
    public partial class ActionSpawn_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<data> Select)
        {
            data here = Data[Here];
            data select = Select[Here];

            if (Something(select) && !Something(here))
            {
                //if ((int)(vertex.TexCoords.x * Data.Size.x) % 2 == 0 &&
                //    (int)(vertex.TexCoords.y * Data.Size.y) % 2 == 0)
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
        unit FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Units, Field<data> Select, float player, float team)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            data select = Select[Here];

            if (Something(select) && !Something(data_here))
            {
                //if ((int)(vertex.TexCoords.x * Units.Size.x) % 2 == 0 &&
                //    (int)(vertex.TexCoords.y * Units.Size.y) % 2 == 0)
                {
                    unit_here.player = player;
                    unit_here.team = team;
                    unit_here.type = UnitType.DragonLord;
                }
            }

            return unit_here;
        }
    }

    public partial class ActionSpawn_Target : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<vec4> Target, Field<data> Select)
        {
            data data_here = Data[Here];
            data select = Select[Here];

            vec4 target = Target[Here];

            if (Something(select) && !Something(data_here))
            {
                //if ((int)(vertex.TexCoords.x * Data.Size.x) % 2 == 0 &&
                //    (int)(vertex.TexCoords.y * Data.Size.y) % 2 == 0)
                {
                    vec2 pos = vertex.TexCoords * Data.Size;
                    target = pack_vec2(pos);
                }
            }

            return target;
        }
    }    
}
