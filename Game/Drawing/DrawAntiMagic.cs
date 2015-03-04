using FragSharpFramework;

namespace Game
{
    public partial class DrawAntiMagic : BaseShader
    {
        public static readonly color
            Active = new color(.3f, .3f, .3f, .3f);

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<TeamTuple> AntiMagic, [Team.Vals] float casting_team)
        {
            color output = color.TransparentBlack;

            TeamTuple here = AntiMagic[Here];

            float max_val = max(here.TeamOne, here.TeamTwo, here.TeamThree, here.TeamFour);

            return max_val > _0 ? Active : color.TransparentBlack;
        }
    }
}
