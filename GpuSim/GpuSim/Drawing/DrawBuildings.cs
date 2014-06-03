using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawBuildings : BaseShader
    {
        protected color Sprite(building u, unit d, vec2 pos, float frame, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(u) ? 3 : 0;

            pos += 255 * vec(u.part_x, u.part_y);
            pos.x += floor(frame);
            pos.y += selected_offset + BuildingSpriteSheet.SubsheetDimY * (255*UnitType.BuildingIndex(d.type));
            pos *= BuildingSpriteSheet.SpriteSize;

            var clr = Texture[pos];

            if (IsNeutralBuilding(d))
                return clr;
            else
                return PlayerColorize(clr, d.player);
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<building> Buildings, Field<unit> Units, PointSampler Texture, float s)
        {
            color output = color.TransparentBlack;

            building building_here = Buildings[Here];
            unit unit_here = Units[Here];

            if (!IsBuilding(unit_here)) return output;

            vec2 subcell_pos = get_subcell_pos(vertex, Buildings.Size);

            if (Something(building_here))
	        {
                float frame = 0;
                output += Sprite(building_here, unit_here, subcell_pos, frame, Texture);
	        }

            return output;
        }
    }
}
