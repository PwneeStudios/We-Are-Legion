using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawTerritoryPlayer : BaseShader
    {
        public static readonly color
            Available = new color(.2f, .7f, .2f, .8f),
            Unavailable = new color(.7f, .2f, .2f, .8f);

        public static readonly color
            Controlled = new color(.2f, .7f, .2f, .125f),
            Uncontrolled = new color(0f, 0f, 0f, .65f);
            
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<vec4> Path)
        {
            color output = color.TransparentBlack;

            vec4 here = Path[Here];

            float total_min = min(here.x, here.y, here.z, here.w);
            bool controlled = total_min < here.x && total_min < _20;

            color clr = controlled ? Controlled : Uncontrolled;
            clr.rgb *= clr.a;

            return clr;
        }
    }

    public partial class DrawTerritoryColors : BaseShader
    {
        public static readonly color
            Available = new color(.2f, .7f, .2f, .8f),
            Unavailable = new color(.7f, .2f, .2f, .8f);

        public static readonly color
            //Controlled = new color(0f, 0f, 0f, 0f),
            //Controlled = new color(.2f, .7f, .2f, .5f),
            //Uncontrolled = new color(0f, 0f, 0f, .5f);
            Controlled = new color(.7f, .3f, .3f, .5f),
            Uncontrolled = new color(.1f, .5f, .1f, .5f);

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<vec4> Path, float blend)
        {
            color output = color.TransparentBlack;

            vec4 here = Path[Here];

            float total_min = min(here.x, here.y, here.z, here.w);
            bool controlled = total_min < here.x && total_min < _20;
            bool controlled2 = total_min < here.y && total_min < _20;

            //color clr = controlled ? Controlled : Uncontrolled;
            color clr = controlled ? Controlled : (controlled2 ? Uncontrolled : color.TransparentBlack);
            clr.a *= blend;
            clr.rgb *= clr.a;

            return clr;
        }
    }
}
