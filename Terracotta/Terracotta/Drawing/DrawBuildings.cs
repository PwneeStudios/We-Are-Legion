using FragSharpFramework;

namespace GpuSim
{
    public partial class DrawBuildingsIcons : BaseShader
    {
        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<BuildingDist> BuildingDistancess, float blend, float radius)
        {
            BuildingDist info = BuildingDistancess[Here];

            if (info.dist > _15) return color.TransparentBlack;

            vec2 subcell_pos = get_subcell_pos(vertex, BuildingDistancess.Size);

            var v = 255 * (info.diff - Pathfinding_ToBuildings.CenterOffset) - (subcell_pos - vec(.5f, .5f));
            if (length(v) < radius)
            {
                color clr = BuildingMarkerColors.Get(get_player(info), get_type(info));

                return clr * blend;
            }

            return color.TransparentBlack;
        }
    }

    public partial class DrawBuildings : BaseShader
    {
        protected color Sprite(building u, unit d, vec2 pos, float frame, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            float selected_offset = selected(u) ? 3 : 0;

            pos += Float(vec(u.part_x, u.part_y));
            pos.x += Float(d.player) * BuildingSpriteSheet.BuildingDimX;
            pos.y += selected_offset + BuildingSpriteSheet.SubsheetDimY * Float(UnitType.BuildingIndex(d.type));
            pos *= BuildingSpriteSheet.SpriteSize;

            return Texture[pos];
        }

        protected color ExplosionSprite(building u, unit d, vec2 pos, float frame, PointSampler Texture)
        {
            if (pos.x > 1 || pos.y > 1 || pos.x < 0 || pos.y < 0)
                return color.TransparentBlack;

            pos += 255 * vec(u.part_x, u.part_y);
            pos.x += floor(frame) * ExplosionSpriteSheet.DimX;
            pos *= ExplosionSpriteSheet.SpriteSize;

            return Texture[pos];
        }

        [FragmentShader]
        color FragmentShader(VertexOut vertex, Field<building> Buildings, Field<unit> Units, PointSampler Texture, PointSampler Explosion, float s)
        {
            color output = color.TransparentBlack;

            building building_here = Buildings[Here];
            unit unit_here = Units[Here];

            if (!IsBuilding(unit_here)) return output;

            vec2 subcell_pos = get_subcell_pos(vertex, Buildings.Size);

            if (Something(building_here))
            {
                if (building_here.direction >= Dir.StationaryDead)
                {
                    float frame = ExplosionSpriteSheet.ExplosionFrame(s, building_here);
                    if (frame < ExplosionSpriteSheet.AnimLength)
                    {
                        output += ExplosionSprite(building_here, unit_here, subcell_pos, frame, Explosion);
                    }
                }
                else
                {
                    float frame = 0;
                    output += Sprite(building_here, unit_here, subcell_pos, frame, Texture);
                }
            }

            return output;
        }
    }
}
