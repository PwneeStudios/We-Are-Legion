using FragSharpFramework;

namespace Game
{
    public partial class SetSelectedAction : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, float action, [Player.Vals] float player)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];

            if (unit_here.player == player && Something(data_here) && IsUnit(unit_here) && selected(data_here) && action < UnitAction.NoChange)
            {
                data_here.action = action;
            }

            return data_here;
        }
    }

    public partial class ActionAttackSquare : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<data> TargetData, vec2 Destination_BL, vec2 Selection_BL, vec2 ConversionRatio, [Player.Vals] float player, [SelectionFilter.Vals] float filter)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];
            vec4 target = vec4.Zero;

            if (player == unit_here.player && selected(data_here) && SelectionFilter.FilterHasUnit(filter, unit_here.type))
            {
                vec2 pos = vertex.TexCoords * Data.Size;

                pos = (pos - Selection_BL);
                pos = pos * ConversionRatio + Destination_BL;
                pos = round(pos);

                target = pack_vec2(pos);
            }
            else
            {
                target = (vec4)TargetData[Here];
            }

            return target;
        }
    }

    public partial class ActionAttackPoint : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<data> TargetData, vec2 Destination, [Player.Vals] float player, [SelectionFilter.Vals] float filter)
        {
            data data_here = Data[Here];
            unit unit_here = Unit[Here];
            vec4 target = vec4.Zero;

            if (player == unit_here.player && selected(data_here) && SelectionFilter.FilterHasUnit(filter, unit_here.type))
            {
                vec2 dest = Destination;

                target = pack_vec2(dest);
            }
            else
            {
                target = (vec4)TargetData[Here];
            }
            
            return target;
        }
    }

    public partial class ActionAttack2 : SimShader
    {
        [FragmentShader]
        extra FragmentShader(VertexOut vertex, Field<data> Data, Field<unit> Unit, Field<extra> Extra, vec2 Destination, [Player.Vals] float player, [SelectionFilter.Vals] float filter)
        {
            data  data_here  = Data[Here];
            unit  unit_here  = Unit[Here];
            extra extra_here = Extra[Here];

            if (player == unit_here.player && selected(data_here) && SelectionFilter.FilterHasUnit(filter, unit_here.type))
            {
                extra_here = extra.Nothing;
            }

            return extra_here;
        }
    }
}
