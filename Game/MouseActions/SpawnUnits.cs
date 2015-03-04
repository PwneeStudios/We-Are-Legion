using System;
using FragSharpFramework;

namespace Game
{
    public class UnitDistribution : BaseShader
    {
        [FragSharpFramework.Vals(Full, EveryOther, OnCorpses)]
        public class ValsAttribute : Attribute { }

        public static readonly float[] Vals = new float[] { Full, EveryOther, OnCorpses, Single };

        public const float
            None = 0,

            Full = 1,
            EveryOther = 2,
            OnCorpses = 3,
            Single = 4,
            
            First = 1,
            Last = 5;

        public static bool Contains(float distribution, vec2 v, Field<corpse> Corpses)
        {
            if (distribution == Full)
            {
                return true;
            }

            if (distribution == EveryOther)
            {
                return (int)(v.x) % 2 == 0 && (int)(v.y) % 2 == 0;
            }

            if (distribution == OnCorpses)
            {
                return CorpsePresent(Corpses[Here]);
            }

            return false;
        }

        public static string Name(float distribution)
        {
            if (distribution == None) return "None";

            if (distribution == Full) return "Full";
            if (distribution == EveryOther) return "Every Other";
            if (distribution == OnCorpses) return "On Corpses";
            if (distribution == Single) return "Single";

            return "Invalid Distribution Type";
        }
    }

    public partial class ActionSpawn_Filter : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Select, Field<data> Data, Field<unit> Units, Field<corpse> Corpses, Field<TeamTuple> AntiMagic,
            [UnitDistribution.Vals] float distribution, [Team.Vals] float AntiMagicTeam)
        {
            data select = Select[Here];
            data here = Data[Here];
            
            TeamTuple antimagic = AntiMagic[Here];
            if (antimagic.TeamOne > _0 && AntiMagicTeam != Team.One)     return data.Nothing;
            if (antimagic.TeamTwo > _0 && AntiMagicTeam != Team.Two)     return data.Nothing;
            if (antimagic.TeamThree > _0 && AntiMagicTeam != Team.Three) return data.Nothing;
            if (antimagic.TeamFour > _0 && AntiMagicTeam != Team.Four)   return data.Nothing;

            if (Something(select) && !Something(here))
            {
                if (UnitDistribution.Contains(distribution, vertex.TexCoords * Select.Size, Corpses))
                {
                    return select;
                }
            }

            return data.Nothing;
        }
    }

    public partial class ActionSpawn_Data : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Select, Field<data> Data)
        {
            data select = Select[Here];
            data here = Data[Here];

            if (Something(select))
            {
                here.direction = Dir.Right;
                here.action = UnitAction.Guard;
                here.change = Change.Stayed;
                set_prior_direction(ref here, here.direction);
            }

            return here;
        }
    }

    public partial class ActionSpawn_Unit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<data> Select, Field<unit> Units, float player, float team, float type, [Vals.Bool] bool raising)
        {
            data select = Select[Here];
            unit here = Units[Here];

            if (Something(select))
            {
                here.player = player;
                here.team = team;
                here.type = type;

                if (raising)
                {
                    here.anim = Anim.DoRaise;
                }
                else
                {
                    here.anim = Anim.Stand;
                }
            }

            return here;
        }
    }

    public partial class ActionSpawn_Target : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Select, Field<vec4> Target)
        {
            data select = Select[Here];
            vec4 here = Target[Here];

            if (Something(select))
            {
                vec2 pos = vertex.TexCoords * Target.Size;
                here = pack_vec2(pos);
            }

            return here;
        }
    }

    public partial class ActionSpawn_Corpse : SimShader
    {
        [FragmentShader]
        corpse FragmentShader(VertexOut vertex, Field<data> Select, Field<corpse> Corpses)
        {
            data select = Select[Here];
            corpse here = Corpses[Here];

            if (Something(select))
            {
                here = corpse.Nothing;
            }

            return here;
        }
    }    
}
