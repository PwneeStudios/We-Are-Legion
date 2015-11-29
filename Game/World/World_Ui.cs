using System;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;

using FragSharpHelper;
using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        RectangleQuad q = new RectangleQuad();
        string unit_count = "";
        vec2 count_text_pos;

        int NumUnitTypesSelected()
        {
            return DataGroup.UnitSummary.Count(b => b);
        }

        void DrawSelectedInfo()
        {
            color clr = rgba(0x888888, .5f).Premultiplied;

            bool building_selected = UnitType.BuildingVals.Any(type => DataGroup.UnitSummary[Int(type) - 1]);

            vec2 size = vec(.145f, .145f);
            float building_scale = 1.4f;
            float building_shift = building_selected ? (building_scale - 1) * 2 * size.x : 0;
            vec2 shift = vec(-size.x, 0);
            vec2 start;
            if (MapEditor)
            {
                start = vec(CameraAspect, -1) + vec(-.1f, .46f);
            }
            else
            {
                start = vec(CameraAspect, -1) + vec(-.1f, .21f);
            }
            
            vec2 cur_pos = start - vec(building_shift, 0);

            for (int i = Int(UnitType.Count) - 1; i>= 0 ; i--)
            {
                if (DataGroup.UnitSummary[i])
                {
                    float type = _[i + 1];

                    vec2 pos = cur_pos + -2 * size.FlipY();

                    vec2 s = size;
                    if (IsBuilding(type))
                    {
                        s *= building_scale;
                        pos.y += s.y * .2f;
                    }

                    SetUnitQuad(pos, s, type, MyPlayerNumber, (GameClass.World.DrawCount / 7) % UnitSpriteSheet.AnimLength, Dir.Left, q);

                    bool FilteredOut = SelectionFilter.FilterHasUnit(CurSelectionFilter, type);
                    color Shade = FilteredOut ? rgb(0xffffff) : rgb(0x000000);

                    DrawColoredTexture.Using(vec(0, 0, 1, 1), CameraAspect, q.Texture, Shade);
                    q.Draw(GameClass.Graphics);

                    cur_pos += shift;
                }
            }
        }

        public void DrawGridCell()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos);

            color clr = CanPlaceItem ? DrawTerritoryPlayer.Available : DrawTerritoryPlayer.Unavailable;
            DrawSolid.Using(camvec, CameraAspect, clr);

            vec2 gWorldCord = GridToWorldCood(floor(GridCoord));
            vec2 size = 1 / DataGroup.GridSize;
            RectangleQuad.Draw(GameClass.Graphics, gWorldCord + new vec2(size.x, -size.y), size);
        }

        void DrawAvailabilityGrid()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            int _w = 3, _h = 3;

            color clr = color.TransparentBlack;

            CanPlaceItem = true;
            for (int i = 0; i < _w; i++)
            for (int j = 0; j < _h; j++)
            {
                clr = CanPlace[i + j * _h] ? DrawTerritoryPlayer.Available : DrawTerritoryPlayer.Unavailable;
                DrawSolid.Using(camvec, CameraAspect, clr);

                vec2 gWorldCord = GridToWorldCood(new vec2((float)Math.Floor(GridCoord.x + i), (float)Math.Floor(GridCoord.y + j)));
                vec2 size = 1 / DataGroup.GridSize;
                RectangleQuad.Draw(GameClass.Graphics, gWorldCord + new vec2(size.x, -size.y), size);
            }
        }

        void DrawPotentialBuilding()
        {
            vec2 GridCoord = ScreenToGridCoord(Input.CurMousePos) - new vec2(1, 1);

            DrawTexture.Using(camvec, CameraAspect, Assets.BuildingTexture_1);

            vec2 WorldCord = GridToWorldCood(new vec2((float)Math.Floor(GridCoord.x), (float)Math.Floor(GridCoord.y)));
            vec2 size = 3 * 1 / DataGroup.GridSize;

            var building = SetBuildingQuad(WorldCord, size, BuildingUserIsPlacing, MyPlayerNumber);
            building.SetColor(new color(1, 1, 1, .7f));
            building.Draw(GameClass.Graphics);
        }

        RectangleQuad SetUnitQuad(vec2 pos, vec2 size, float type, int player, int frame, float dir, RectangleQuad quad = null)
        {
            if (IsBuilding(type)) return SetBuildingQuad(pos, size, type, player, quad);

            vec2 uv_size = UnitSpriteSheet.SpriteSize;
            
            vec2 uv_offset;
            uv_offset.x = frame;
            uv_offset.y = (Float(dir) - 1) + 4 * (player - 1) + 4 * 4 * Float(UnitType.UnitIndex(type));
            uv_offset *= UnitSpriteSheet.SpriteSize;

            if (quad == null)
            {
                quad = new RectangleQuad();
            }

            quad.SetupVertices(pos, pos + 2 * new vec2(size.x, -size.y), new vec2(0, uv_size.y) + uv_offset, new vec2(uv_size.x, 0.005f * uv_size.y) + uv_offset);
            quad.Texture = Assets.UnitTexture_1;

            return quad;
        }

        RectangleQuad SetBuildingQuad(vec2 pos, vec2 size, float type, int player, RectangleQuad quad = null)
        {
            vec2 uv_size = BuildingSpriteSheet.BuildingSize;
            
            vec2 uv_offset;
            uv_offset.x = player * BuildingSpriteSheet.BuildingDimX;
            uv_offset.y = BuildingSpriteSheet.SubsheetDimY * Float(UnitType.BuildingIndex(type));
            uv_offset *= BuildingSpriteSheet.SpriteSize;

            if (quad == null)
            {
                quad = new RectangleQuad();
            }

            quad.SetupVertices(pos, pos + 2 * new vec2(size.x, -size.y), new vec2(0, uv_size.y) + uv_offset, new vec2(uv_size.x, 0) + uv_offset);
            quad.Texture = Assets.BuildingTexture_1;

            return quad;
        }


        public void DrawCursor(Texture2D Texture, vec2 Size, float Angle = 0)
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTextureSmooth.Using(camvec, CameraAspect, Texture);
            RectangleQuad.Draw(GameClass.Graphics, WorldCord, Size, Angle);
        }

        bool LineSelect = false;
        public void DrawCircleCursor(bool AfterUi)
        {
            if (AfterUi) return;

            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTextureSmooth.Using(camvec, CameraAspect, Assets.SelectCircle);
            RectangleQuad.Draw(GameClass.Graphics, WorldCord, SelectSize);

            DrawTextureSmooth.Using(camvec, CameraAspect, Assets.SelectDot);
            RectangleQuad.Draw(GameClass.Graphics, WorldCord, .0075f * vec2.Ones / CameraZoom);
        }

        vec2 BoxSelectStart = vec2.Zero, BoxSelectEnd = vec2.Zero;
        vec2 BoxSelectGridStart = vec2.Zero, BoxSelectGridEnd = vec2.Zero;
        public bool BoxSelecting = false;
        public void DrawBoxSelect(bool AfterUi)
        {
            if (AfterUi)
            {
                DrawArrowCursor();
                return;
            }

            if (!LeftMouseDown)
            {
                BoxSelecting = false;
                return;
            }

            if (!BoxSelecting && LeftMousePressed && MouseInGame)
            {
                BoxSelecting = true;
            }

            //BoxSelecting = true;
            if (!BoxSelecting) return;

            vec2 pos = ScreenToWorldCoord(Input.CurMousePos);
            vec2 grid_pos = ScreenToGridCoord(Input.CurMousePos);

            if (LeftMousePressed)
            {
                BoxSelectStart = pos; BoxSelectEnd = pos; BoxSelectGridStart = grid_pos; BoxSelectGridEnd = grid_pos;
            }
            else
            {
                BoxSelectEnd = pos; BoxSelectGridEnd = grid_pos;
            }

            DrawSolid.Using(camvec, CameraAspect, new color(.6f, .6f, .6f, .5f));
            DrawBox(BoxSelectEnd, BoxSelectStart, 2f / (GameClass.Screen.x * CameraZoom));
        }

        public void DrawArrowCursor()
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTextureSmooth.Using(camvec, CameraAspect, Assets.Cursor);

            vec2 size = .02f * Assets.Cursor.UnitSize() / CameraZoom;
            RectangleQuad.Draw(GameClass.Graphics, WorldCord + new vec2(size.x, -size.y), size);
        }

        void AddAttackMarker()
        {
            vec2 pos = ScreenToWorldCoord(Input.CurMousePos);

            if (MouseOverMinimap)
            {
                pos = MinimapWorldPos();
            }

            vec2 cell_size = 2 * (1 / DataGroup.GridSize);

            float bigger = 1 / (CameraZoom / 30);
            if (bigger < 1) bigger = 1;
            vec2 size = cell_size * bigger;

            Markers.Add(new Marker(this, pos, size, Assets.AttackMarker, -1f));
        }

        public vec2 DragonLordDeathPos;
        public void DragonLordDeath(vec2 GridCoord, int PlayerNum)
        {
            vec2 Pos = GridToWorldCood(GridCoord + vec(1, 1));
            vec2 Pos2 = GridToWorldCood(GridCoord - vec(0, 0));
            DragonLordDeathPos = Pos;

            AddSummonUnitEffect(GridCoord);
            AddExplosion(Pos2, vec(5, 5));
            Message_PlayerDefeated(PlayerNum);
        }

        void AddExplosion(vec2 Pos, vec2 Size)
        {
            vec2 size = vec(1.266f, 1.35f) * Size * CellWorldSize;

            Markers.Add(new Marker(this, Pos, size, ExsplosionSprite, -1f, frames : ExplosionSpriteSheet.AnimLength, DrawOrder : DrawOrder.AfterTiles, frame_length : .066f));
            Markers.Add(new Marker(this, Pos, size, ExsplosionSprite, -1f, frames : ExplosionSpriteSheet.AnimLength, DrawOrder : DrawOrder.AfterUnits, frame_length : .066f, alpha : 1));
        }

        void AddBuildBuildingEffect(vec2 GridCoord)
        {
            vec2 size = 6 * vec(1.266f, 1.35f) * CellWorldSize;
            vec2 pos = GridToWorldCood(floor(GridCoord)) + new vec2(CellWorldSize.x, -CellWorldSize.y) / 2;

            Markers.Add(new Marker(this, pos, size, Assets.SmokeTexture, -1f, frames: 4, DrawOrder: DrawOrder.AfterUnits, alpha: 1, dsize_dt: vec(.01f, .01f)));
        }

        void AddSummonUnitEffect(vec2 GridCoord)
        {
            vec2 size = vec(1.266f, 1.35f) * 2 * CellWorldSize;
            vec2 pos = GridToWorldCood(floor(GridCoord)) + new vec2(CellWorldSize.x, -CellWorldSize.y) / 2;

            Markers.Add(new Marker(this, pos, size, Assets.MagicTexture, -1f, frames: 4, DrawOrder: DrawOrder.AfterTiles, dsize_dt: vec(.002f, .002f)));
            Markers.Add(new Marker(this, pos, size, Assets.MagicTexture, -1f, frames: 4, DrawOrder: DrawOrder.AfterUnits, dsize_dt: vec(.002f, .002f), alpha: 1));
        }

        void AddSummonAreaEffect(vec2 pos, vec2 area)
        {
            vec2 cell_size = 2 * (1 / DataGroup.GridSize);
            vec2 size = vec(1.266f, 1.35f) * area * cell_size;

            Markers.Add(new Marker(this, pos, size, Assets.MagicTexture, alpha_fade: -1.5f, frames: 4, frame_length: .1375f, DrawOrder: DrawOrder.AfterTiles, dsize_dt: .65f * size));
            Markers.Add(new Marker(this, pos, size, Assets.MagicTexture, alpha_fade: -1.5f, frames: 4, frame_length: .1375f, DrawOrder: DrawOrder.AfterUnits, dsize_dt: .65f * size, alpha: 1));
        }

        void AddUserMessage(string Message, params object[] p)
        {
            var FormattedMessage = string.Format(Message, p);
            UserMessages.Add(new UserMessage(this, FormattedMessage));
        }

        void AddUserMessage(int ExtraLife, string Message, params object[] p)
        {
            var FormattedMessage = string.Format(Message, p);
            UserMessages.Add(new UserMessage(this, FormattedMessage, extra_life:ExtraLife));
        }
    }
}
