using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawTerritoryPlayer : BaseShader
    {
        public const float TerritoryCutoff = _16;

        public static readonly color
            Available = new color(.2f, .7f, .2f, .8f),
            Unavailable = new color(.7f, .2f, .2f, .8f);

        public static readonly color
            Controlled = new color(0f, 0f, 0f, 0f),
            Uncontrolled = new color(0f, 0f, 0f, .65f);

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<vec4> Path, [Player.Vals] float player)
        {
            color output = color.TransparentBlack;

            vec4 here = Path[Here];

            float dist = Get(here, player);
            bool controlled = dist < TerritoryCutoff;

            color clr = controlled ? Controlled : Uncontrolled;

            clr.rgb *= clr.a;

            return clr;
        }
    }

    public partial class DrawTerritoryColors : BaseShader
    {
        public static readonly color
            Team1 = new color(.7f, .3f, .3f, .5f),
            Team2 = new color(.1f, .5f, .1f, .5f),
            Team3 = new color(.3f, .7f, .55f, .5f),
            Team4 = new color(.3f, .3f, .7f, .5f);

        public const float TerritoryCutoff = _7;

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<vec4> Path, float blend)
        {
            vec4 dist = Path[Here];

            vec4 enemy_dist = vec(
                min(dist.y, dist.z, dist.w),
                min(dist.x, dist.z, dist.w),
                min(dist.x, dist.y, dist.w),
                min(dist.x, dist.y, dist.z));

            color clr = color.TransparentBlack;
            float _blend = 1;

            //if (dist.x < TerritoryCutoff && dist.x < enemy_dist.x) { clr = Team1; _blend = max(.3f, min(1, (TerritoryCutoff - dist.x) / _3)); }
            //if (dist.y < TerritoryCutoff && dist.y < enemy_dist.y) { clr = Team2; _blend = max(.3f, min(1, (TerritoryCutoff - dist.y) / _3)); }
            //if (dist.z < TerritoryCutoff && dist.z < enemy_dist.z) { clr = Team3; _blend = max(.3f, min(1, (TerritoryCutoff - dist.z) / _3)); }
            //if (dist.w < TerritoryCutoff && dist.w < enemy_dist.w) { clr = Team4; _blend = max(.3f, min(1, (TerritoryCutoff - dist.w) / _3)); }
            if (dist.x < TerritoryCutoff && dist.x < enemy_dist.x) clr = Team1;
            if (dist.y < TerritoryCutoff && dist.y < enemy_dist.y) clr = Team2;
            if (dist.z < TerritoryCutoff && dist.z < enemy_dist.z) clr = Team3;
            if (dist.w < TerritoryCutoff && dist.w < enemy_dist.w) clr = Team4;

            clr *= _blend;
            clr.a *= blend;
            clr.rgb *= clr.a;

            return clr;
        }
    }
}
