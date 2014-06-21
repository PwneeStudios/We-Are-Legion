using FragSharpFramework;

namespace GpuSim
{
    public partial class World : SimShader
    {
        vec2 ScreenToGridCoord(vec2 pos)
        {
            var world = ScreenToWorldCoord(pos);
            world.y = -world.y;

            var grid_coord = GameClass.Screen * (world + vec2.Ones) / 2;

            return grid_coord;
        }

        vec2 GridToScreenCoord(vec2 pos)
        {
            pos = 2 * pos / GameClass.Screen - vec2.Ones;
            pos.y = -pos.y;
            return pos;
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
