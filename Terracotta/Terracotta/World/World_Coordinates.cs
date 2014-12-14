using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        vec2 ScreenToGridCoord(vec2 pos)
        {
            var world = ScreenToWorldCoord(pos);
            world.y = -world.y;

            var grid_coord = DataGroup.GridSize * (world + vec2.Ones) / 2;

            return grid_coord;
        }

        public vec2 CellWorldSize
        {
            get
            {
                return 2 * DataGroup.CellSpacing;
            }
        }

        vec2 GridToWorldCood(vec2 pos)
        {
            pos = 2 * pos / DataGroup.GridSize - vec2.Ones;
            pos.y = -pos.y;
            return pos;
        }

        vec2 GridToWorldSize(vec2 size)
        {
            size = 2 * size / DataGroup.GridSize;

            return size;
        }

        vec2 WorldMousePos
        {
            get
            {
                return ScreenToWorldCoord(Input.CurMousePos);
            }
        }

        vec2 GridMousePos
        {
            get
            {
                return ScreenToGridCoord(Input.CurMousePos);
            }
        }

        vec2 ScreenToWorldCoord(vec2 pos)
        {
            var screen = GameClass.Screen;
            var ScreenCord = (2 * pos - screen) / screen;
            vec2 WorldCord;
            WorldCord.x = CameraAspect * ScreenCord.x / camvec.z + camvec.x;
            WorldCord.y = -ScreenCord.y / camvec.w + camvec.y;
            return WorldCord;
        }

        vec2 ScreenToUiCoord(vec2 pos)
        {
            var screen = GameClass.Screen;
            var ScreenCord = (2 * pos - screen) / screen;
            vec2 WorldCord;
            WorldCord.x = CameraAspect * ScreenCord.x;
            WorldCord.y = -ScreenCord.y;
            return WorldCord;
        }

        vec2 GetShiftedCamera(vec2 pos, vec4 prev_camvec, vec2 prev_worldcoord)
        {
            var screen = GameClass.Screen;
            var ScreenCord = (2 * pos - screen) / screen;

            vec2 shifted_cam;
            shifted_cam.x = prev_worldcoord.x - CameraAspect * ScreenCord.x / prev_camvec.z;
            shifted_cam.y = prev_worldcoord.y + ScreenCord.y / prev_camvec.w;

            return shifted_cam;
        }
    }
}
