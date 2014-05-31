using FragSharpFramework;

namespace GpuSim
{
    public partial class Building_SelectCenterIfSelected_SetDirecion : SimShader
    {
        [FragmentShader]
        building FragmentShader(VertexOut vertex, Field<unit> Unit, Field<building> Building)
        {
            building building_here = Building[Here];
            unit unit_here = Unit[Here];

            if (Something(building_here) && IsBuilding(unit_here) && IsCenter(building_here) && !selected(building_here))
            {
                // Select this center if any part of the building is selected
                bool is_selected =
                    selected(Building[RightOne]) ||
                    selected(Building[LeftOne]) ||
                    selected(Building[UpOne]) ||
                    selected(Building[DownOne]) ||
                    selected(Building[UpRight]) ||
                    selected(Building[UpLeft]) ||
                    selected(Building[DownRight]) ||
                    selected(Building[DownLeft]);

                set_selected(ref building_here, is_selected);
            }

            return building_here;
        }
    }

    public partial class BuildingDiffusion_Data : SimShader
    {
        [FragmentShader]
        building FragmentShader(VertexOut vertex, Field<unit> Unit, Field<building> Building)
        {
            building building_here = Building[Here];
            unit unit_here = Unit[Here];

            if (Something(building_here) && IsBuilding(unit_here))
            {
                building center = Building[center_dir(building_here)];
                
                building_here.prior_direction_and_select = center.prior_direction_and_select;
                building_here.direction = center.direction;
            }

            return building_here;
        }
    }

    public partial class BuildingDiffusion_Target : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<unit> Unit, Field<building> Building, Field<vec4> TargetData)
        {
            building building_here = Building[Here];
            unit unit_here = Unit[Here];
            vec4 target = TargetData[Here];

            if (Something(building_here) && IsBuilding(unit_here))
            {
                target = TargetData[center_dir(building_here)];
            }

            return target;
        }
    }
}
