using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        public void DrawVisibleGrid(float scale=1)
        {
            vec2 center = CameraPos, size = vec(CameraAspect, 1) * scale;

            vec2 uv_center = WorldToGridCood(CameraPos) / GridSize;
            vec2 uv_size = abs((WorldToGridCood(vec(CameraAspect, 1) / CameraZoom) - WorldToGridCood(vec(0, 0))) / GridSize);

            vec2
                uv_bl = uv_center - uv_size,
                uv_tr = uv_center + uv_size,
                bl = CameraPos - vec(CameraAspect, 1) / CameraZoom,
                tr = CameraPos + vec(CameraAspect, 1) / CameraZoom;

            RectangleQuad q = new RectangleQuad(bl, tr, uv_bl, uv_tr);
            q.Draw(GameClass.Graphics);
        }
    }
}
