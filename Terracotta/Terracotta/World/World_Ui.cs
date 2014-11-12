using System;

using Microsoft.Xna.Framework.Graphics;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        void DrawUi_TopInfo()
        {
            if (MapEditorActive)
            {
                string header = "Map editor"
                    + (", Player " + PlayerNumber.ToString())
                    + (SimulationPaused ? ", Paused" : "");
                
                Render.DrawText(header, vec(10, 0), 1);
            }
            else
            {
                var top_ui_gold = string.Format("Gold {0:#,##0}", PlayerInfo[PlayerNumber].Gold);
                var top_ui_units = string.Format("Units {0:#,##0}", DataGroup.UnitCount[PlayerNumber]);

                Render.DrawText(top_ui_gold, vec(10, 0), 1);
                Render.DrawText(top_ui_units, vec(170, 0), 1);
            }
        }

        void DrawUi_PlayerGrid()
        {
            float y = 0;
            float spacing = 20;

            float row4 = 1024, row3 = 924, row2 = 824, row1 = 724, row0 = 624;

            Render.DrawText("Raxes", vec(row1, y), 1, align: Alignment.RightJusitfy);
            Render.DrawText("Units", vec(row2, y), 1, align: Alignment.RightJusitfy);
            Render.DrawText("Mines", vec(row3, y), 1, align: Alignment.RightJusitfy);
            Render.DrawText("Gold", vec(row4, y), 1, align: Alignment.RightJusitfy);

            for (int player = 1; player <= 4; player++)
            {
                y += spacing;

                var gold = string.Format("{0:#,##0}", PlayerInfo[player].Gold);
                var units = string.Format("{0:#,##0}", DataGroup.UnitCount[player]);
                var mines = string.Format("{0:#,##0}", PlayerInfo[player].GoldMines);
                var raxes = string.Format("{0:#,##0}", DataGroup.BarracksCount[player]);

                Render.DrawText("Player " + player.ToString(), vec(row0, y), 1, align: Alignment.RightJusitfy);
                Render.DrawText(raxes, vec(row1, y), 1, align: Alignment.RightJusitfy);
                Render.DrawText(units, vec(row2, y), 1, align: Alignment.RightJusitfy);
                Render.DrawText(mines, vec(row3, y), 1, align: Alignment.RightJusitfy);
                Render.DrawText(gold, vec(row4, y), 1, align: Alignment.RightJusitfy);
            }
        }

        RectangleQuad q = new RectangleQuad();
        vec2 unit_count_pos, building_count_pos;
        void DrawCursorInfo()
        {
            switch (CurUserMode)
            {
                case UserMode.Select:
                    vec2 mouse = ScreenToUiCoord(Input.CurMousePos);
                    color clr = rgba(0x888888c, .5f).Premultiplied;

                    vec2 size = vec(.125f, .024f);

                    if (DataGroup.SelectedUnits > 0)
                    {
                        vec2 pos = mouse + vec(.16f, .215f) + vec(size.x, 0);
                        vec2 unit = pos + vec(.01f, .025f) - vec(size.x, 0);
                        vec2 unit_size = vec(.025f, .025f);
                        unit_count_pos = pos + vec(.075f, .0285f) - vec(size.x, 0);

                        DrawSolid.Using(vec(0, 0, 1, 1), CameraAspect, clr);
                        RectangleQuad.Draw(GameClass.Graphics, pos, size);

                        SetUnitQuad(unit, unit_size, 1, 0, Dir.Right, q);
                        DrawTexture.Using(vec(0, 0, 1, 1), CameraAspect, q.Texture);
                        q.Draw(GameClass.Graphics);
                    }

                    //if (DataGroup.SelectedBarracks > 0)
                    //{
                    //    vec2 b_size = vec(.125f, .024f);
                    //    vec2 b_pos = mouse + vec(.19f, .155f) + vec(size.x, 0);
                    //    vec2 b_unit = b_pos + vec(.01f, .034f) - vec(size.x, 0);
                    //    vec2 b_unit_size = vec(.035f, .035f);
                    //    building_count_pos = b_pos + vec(.095f, .0285f) - vec(size.x, 0);

                    //    DrawSolid.Using(vec(0, 0, 1, 1), CameraAspect, clr);
                    //    RectangleQuad.Draw(GameClass.Graphics, b_pos, b_size);

                    //    SetBuildingQuad(b_unit, b_unit_size, UnitType.Barracks, 1, q);
                    //    DrawTexture.Using(vec(0, 0, 1, 1), CameraAspect, q.Texture);
                    //    q.Draw(GameClass.Graphics);
                    //}

                    break;
            }
        }

        void DrawUi_CursorText()
        {
            if (CurUserMode != UserMode.Select) return;

            string unit_count = string.Empty;
            string building_count = string.Empty;

            vec2 mouse = ScreenToUiCoord(Input.CurMousePos);
            vec2 pos = mouse + vec(.23f, .2435f);

            if (DataGroup.SelectedUnits > 0)
            {
                unit_count = string.Format("{0:#,##0}", DataGroup.SelectedUnits);
            }
            //if (DataGroup.SelectedBarracks > 0)
            //{
            //    building_count = string.Format("{0:#,##0}", DataGroup.SelectedBarracks);
            //}

            Render.DrawText(unit_count, ToBatchCoord(unit_count_pos), .5f);
            Render.DrawText(building_count, ToBatchCoord(building_count_pos), .5f);

            /*
            if (CurUserMode != UserMode.Select) return;

            string selected_count = string.Empty;
            if (DataGroup.SelectedUnits > 0 && DataGroup.SelectedBarracks > 0)
                selected_count = string.Format("[{0:#,##0} : {1:#,##0}]", DataGroup.SelectedUnits, DataGroup.SelectedBarracks);
            else if (DataGroup.SelectedUnits > 0)
                selected_count = string.Format("[{0:#,##0}]", DataGroup.SelectedUnits);
            else if (DataGroup.SelectedBarracks > 0)
                selected_count = string.Format("[{0:#,##0}]", DataGroup.SelectedBarracks);
            else
                selected_count = "[0]";
            
            Render.DrawText(selected_count, Input.CurMousePos + new vec2(30, -130), 1);
             */
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

            var building = SetBuildingQuad(WorldCord, size, BuildingType, PlayerNumber);
            building.SetColor(new color(1, 1, 1, .7f));
            building.Draw(GameClass.Graphics);
        }

        RectangleQuad SetUnitQuad(vec2 pos, vec2 size, int player, int frame, float dir, RectangleQuad quad = null)
        {
            vec2 uv_size = UnitSpriteSheet.SpriteSize;
            
            vec2 uv_offset;
            uv_offset.x = frame;
            uv_offset.y = (Float(dir) - 1) + 4 * (player - 1);
            uv_offset *= UnitSpriteSheet.SpriteSize;

            if (quad == null)
            {
                quad = new RectangleQuad();
            }

            quad.SetupVertices(pos, pos + 2 * new vec2(size.x, -size.y), new vec2(0, uv_size.y) + uv_offset, new vec2(uv_size.x, 0) + uv_offset);
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

        public void DrawCircleCursor()
        {
            vec2 WorldCord = ScreenToWorldCoord(Input.CurMousePos);
            DrawTextureSmooth.Using(camvec, CameraAspect, Assets.SelectCircle);
            RectangleQuad.Draw(GameClass.Graphics, WorldCord, SelectSize);

            DrawTextureSmooth.Using(camvec, CameraAspect, Assets.SelectDot);
            RectangleQuad.Draw(GameClass.Graphics, WorldCord, .0075f * vec2.Ones / CameraZoom);
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
            vec2 cell_size = 2 * (1 / DataGroup.GridSize);

            float bigger = 1 / (CameraZoom / 30);
            if (bigger < 1) bigger = 1;
            vec2 size = cell_size * bigger;

            Markers.Add(new Marker(this, pos, size, Assets.AttackMarker, -1f));
        }

        void AddExplosion()
        {
            vec2 pos = ScreenToWorldCoord(Input.CurMousePos);

            vec2 cell_size = 2 * (1 / DataGroup.GridSize);
            vec2 size = vec(1.266f, 1.35f) * 30 * cell_size;

            Markers.Add(new Marker(this, pos, size, ExsplosionSprite, -1f, frames : ExplosionSpriteSheet.AnimLength, DrawOrder : DrawOrder.AfterTiles));
            Markers.Add(new Marker(this, pos, size, ExsplosionSprite, -1f, frames : ExplosionSpriteSheet.AnimLength, DrawOrder : DrawOrder.AfterUnits, alpha : 1));
        }

        void AddUserMessage(string Message)
        {
            UserMessages.Add(new UserMessage(this, Message));
        }
    }
}
