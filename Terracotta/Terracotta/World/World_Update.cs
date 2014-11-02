using System;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class World : SimShader
    {
        public void EditorUpdate()
        {
            if (!MapEditor) return;

            if (Keys.P.Pressed()) Editor_TogglePause();
            if (Keys.D0.Pressed()) Editor_SwitchPlayer(0);
            if (Keys.D1.Pressed()) Editor_SwitchPlayer(1);
            if (Keys.D2.Pressed()) Editor_SwitchPlayer(2);
            if (Keys.D3.Pressed()) Editor_SwitchPlayer(3);
            if (Keys.D4.Pressed()) Editor_SwitchPlayer(4);
            if (Keys.OemTilde.Pressed()) Editor_ToggleGridLines();
        }

        void Editor_SwitchPlayer(int player)
        {
            PlayerValue = Fint(player);
            TeamValue = Fint(player);
        }

        void Editor_TogglePause()
        {
            SimulationPaused = !SimulationPaused;
        }

        void Editor_ToggleGridLines()
        {
            DrawGridLines = !DrawGridLines;
        }

        public static float StaticMaxZoomOut = 1;
        float x_edge;
        public void Update()
        {
            EditorUpdate();

            float FpsRateModifier = 1;

            //const float MaxZoomOut = 5.33333f, MaxZoomIn = 200f;
            //const float MaxZoomOut = 3.2f, MaxZoomIn = 200f;
            //const float MaxZoomOut = 1f, MaxZoomIn = 200f; // Full zoom-in/out

            float MaxZoomOut, MaxZoomIn;
            if (MapEditor)
            {
                // Full zoom-in/out
                MaxZoomOut = 1f;
                MaxZoomIn = 200f;
            }
            else
            {
                MaxZoomOut = World.StaticMaxZoomOut;
                MaxZoomIn = 200f; // Full zoom-in, Partial zoom-out
            }
            MaxZoomOut = 1f;
            // Zoom all the way out
            if (Keys.Space.Down())
                CameraZoom = MaxZoomOut;

            // Zoom in/out, into the location of the cursor
            var world_mouse_pos = ScreenToWorldCoord(Input.CurMousePos);
            var hold_camvec = camvec;

            if (GameClass.MouseEnabled)
            {
                float MouseWheelZoomRate = 1.3333f * FpsRateModifier;
                if (Input.DeltaMouseScroll < 0) CameraZoom /= MouseWheelZoomRate;
                else if (Input.DeltaMouseScroll > 0) CameraZoom *= MouseWheelZoomRate;
            }

            float KeyZoomRate = 1.125f * FpsRateModifier;
            if (Buttons.X.Down() || Keys.X.Down() || Keys.E.Down())      CameraZoom /= KeyZoomRate;
            else if (Buttons.A.Down() || Keys.Z.Down() || Keys.Q.Down()) CameraZoom *= KeyZoomRate;

            if (CameraZoom < MaxZoomOut) CameraZoom = MaxZoomOut;
            if (CameraZoom > MaxZoomIn) CameraZoom = MaxZoomIn;

            if (GameClass.MouseEnabled && !(Buttons.A.Pressed() || Buttons.X.Pressed()))
            {
                var shifted = GetShiftedCamera(Input.CurMousePos, camvec, world_mouse_pos);
                CameraPos = shifted;
            }

            // Move the camera via: Click And Drag
            //float MoveRate_ClickAndDrag = .00165f * FpsRateModifier;
            //if (Input.LeftMouseDown)
            //    CameraPos += Input.DeltaMousPos / CameraZoom * MoveRate_ClickAndDrag * new vec2(-1, 1);

            // Move the camera via: Push Edge
            if (GameClass.MouseEnabled)
            {
                float MoveRate_PushEdge = .075f * FpsRateModifier;
                var push_dir = vec2.Zero;
                float EdgeRatio = .1f;
                push_dir.x += -CoreMath.Restrict(0, 1, (EdgeRatio * GameClass.Screen.x - Input.CurMousePos.x) / (EdgeRatio * GameClass.Screen.x));
                push_dir.x += CoreMath.Restrict(0, 1, (Input.CurMousePos.x - (1 - EdgeRatio) * GameClass.Screen.x) / (EdgeRatio * GameClass.Screen.x));
                push_dir.y -= -CoreMath.Restrict(0, 1, (EdgeRatio * GameClass.Screen.y - Input.CurMousePos.y) / (EdgeRatio * GameClass.Screen.y));
                push_dir.y -= CoreMath.Restrict(0, 1, (Input.CurMousePos.y - (1 - EdgeRatio) * GameClass.Screen.y) / (EdgeRatio * GameClass.Screen.y));

                CameraPos += push_dir / CameraZoom * MoveRate_PushEdge;
            }

            // Move the camera via: Keyboard or Gamepad
            var dir = Input.Direction();

            float MoveRate_Keyboard = .07f * FpsRateModifier;
            CameraPos += dir / CameraZoom * MoveRate_Keyboard;


            // Make sure the camera doesn't go too far offscreen
            x_edge = Math.Max(.5f * (CameraAspect / CameraZoom) + .5f * (CameraAspect / MaxZoomOut), 1);
            var TR = ScreenToWorldCoord(new vec2(GameClass.Screen.x, 0));
            if (TR.x > x_edge) CameraPos = new vec2(CameraPos.x - (TR.x - x_edge), CameraPos.y);
            if (TR.y > 1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (TR.y - 1));
            var BL = ScreenToWorldCoord(new vec2(0, GameClass.Screen.y));
            if (BL.x < -x_edge) CameraPos = new vec2(CameraPos.x - (BL.x + x_edge), CameraPos.y);
            if (BL.y < -1) CameraPos = new vec2(CameraPos.x, CameraPos.y - (BL.y + 1));


            // Switch modes
            if (Keys.B.Down())
            {
                CurUserMode = UserMode.PlaceBuilding;
                UnselectAll = true; 
                BuildingType = UnitType.Barracks;
            }

            if (Keys.G.Down())
            {
                CurUserMode = UserMode.PlaceBuilding;
                UnselectAll = true;
                BuildingType = UnitType.GoldMine;
            }

            if (Keys.Escape.Down() || Keys.Back.Down() || CurUserMode == UserMode.PlaceBuilding && Input.RightMousePressed)
            {
                CurUserMode = UserMode.Select;
            }
        }

        void SimulationUpdate()
        {
            DataGroup.DoGoldMineCount(PlayerInfo);
            DoGoldUpdate();

            DataGroup.SimulationUpdate();
        }

        void DoGoldUpdate()
        {
            for (int player = 1; player <= 4; player++)
            {
                PlayerInfo[player].Gold +=
                    PlayerInfo[player].GoldMines * Params.GoldPerMinePerTick +
                    DataGroup.BarracksCount[player] * Params.GoldPerBarracksPerTick;

                if (PlayerInfo[player].Gold < 0)
                {
                    PlayerInfo[player].Gold = 0;
                }
            }
        }
    }
}
