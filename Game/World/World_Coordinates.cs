using FragSharpHelper;
using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        vec2 ScreenToGridCoord(vec2 pos)
        {
            var world = ScreenToWorldCoord(pos);
            world.y = -world.y;

            var grid_coord = DataGroup.GridSize * (world + vec2.Ones) / 2 - vec(.25f, .27f);

            return grid_coord;
        }

        public vec2 CellWorldSize
        {
            get
            {
                return 2 * DataGroup.CellSpacing;
            }
        }

        public vec2 GridToWorldCood(vec2 pos)
        {
            pos = 2 * (pos + vec(.25f, .27f)) / DataGroup.GridSize - vec2.Ones;
            pos.y = -pos.y;
            return pos;
        }

        public vec2 WorldToGridCood(vec2 pos)
        {
            pos.y = -pos.y;
            pos = DataGroup.GridSize * (pos + vec2.Ones) / 2;
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

        vec2 UiMousePos
        {
            get
            {
                return ScreenToUiCoord(Input.CurMousePos);
            }
        }

        bool MouseInGame
        {
            get
            {
                return !MouseOverMinimap;
            }
        }

        bool MouseOverMinimap
        {
            get
            {
                return MinimapQuad.Contains(UiMousePos);
            }
        }

        private vec2 MinimapWorldPos()
        {
            return (UiMousePos - MinimapQuad.pos) / MinimapQuad.size;
        }

        private vec2 MinimapGridPos()
        {
            return WorldToGridCood(MinimapWorldPos());
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

        vec2 UiSizeToScreenSize(vec2 size)
        {
            return size / GameClass.Screen * vec(GameClass.ScreenAspect, 1);
        }

        vec2 ScreenToUiCoord(vec2 pos)
        {
            var screen = GameClass.Screen;
            var ScreenCord = (2 * pos - screen) / screen;
            vec2 UiCoord;
            UiCoord.x = CameraAspect * ScreenCord.x;
            UiCoord.y = -ScreenCord.y;
            return UiCoord;
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

        vec2 GetShiftedCameraMinimap(vec2 pos, vec4 prev_camvec, vec2 prev_worldcoord)
        {
            vec2 shifted_cam;
            shifted_cam.x = prev_worldcoord.x - pos.x / prev_camvec.z;
            shifted_cam.y = prev_worldcoord.y + pos.y / prev_camvec.w;

            return shifted_cam;
        }

        public bool GridPointInView(vec2 pos)
        {
            return InView(GridToWorldCood(pos));
        }

        public bool InView(vec2 pos)
        {
            vec2 tr = CameraPos + vec(CameraAspect, 1) / CameraZoom;
            vec2 bl = CameraPos - vec(CameraAspect, 1) / CameraZoom;

            return pos > bl && pos < tr;
        }
    }
}
