using System;

using Microsoft.Xna.Framework.Input;

using FragSharpHelper;
using FragSharpFramework;

namespace Game
{
    public partial class World : SimShader
    {
        public void EditorUpdate()
        {
            if (MapEditor && Keys.OemTilde.Pressed()) Editor_ToggleMapEditor();
            if (MapEditor && Keys.P.Pressed()) Editor_ToggleMapEditor();

            if (!MapEditorActive) return;

            if (Keys.D0.Pressed()) Editor_SwitchPlayer(0);
            if (Keys.D1.Pressed()) Editor_SwitchPlayer(1);
            if (Keys.D2.Pressed()) Editor_SwitchPlayer(2);
            if (Keys.D3.Pressed()) Editor_SwitchPlayer(3);
            if (Keys.D4.Pressed()) Editor_SwitchPlayer(4);
        }

        public void Editor_SwitchPlayer(int player)
        {
            MyPlayerValue = Fint(player);
            MyTeamValue = Fint(player);
            GameClass.Game.UpdateEditorJsData();
        }

        void Editor_TogglePause()
        {
            SimulationPaused = !SimulationPaused;
        }

        void Editor_ToggleGridLines()
        {
            DrawGridLines = !DrawGridLines;
        }

        public void Editor_ToggleMapEditor()
        {
            MapEditorActive = !MapEditorActive;

            if (MapEditorActive && MyPlayerNumber == 0)
            {
                MyPlayerNumber = 1;
            }

            GameClass.Game.UpdateEditorJsData();
        }

        bool LeftMouseDown
        {
            get
            {
                return Input.LeftMouseDown && (!GameClass.Game.MouseDownOverUi || BoxSelecting);
            }
        }

        bool LeftMousePressed
        {
            get
            {
                return Input.LeftMousePressed && !GameClass.Game.MouseDownOverUi;
            }
        }

        public static float StaticMaxZoomOut = .7333f;
        float x_edge, y_edge;
        int ChatInhibitor = 0;
        public void Update()
        {
            if (!GameClass.Game.GameInputEnabled || DesyncPause) return;

            EditorUpdate();

            float FpsRateModifier = 1;

            //const float MaxZoomOut = 5.33333f, MaxZoomIn = 200f;
            //const float MaxZoomOut = 3.2f, MaxZoomIn = 200f;
            //const float MaxZoomOut = 1f, MaxZoomIn = 200f; // Full zoom-in/out

            float MaxZoomOut, MaxZoomIn;
            if (MapEditorActive)
            {
                // Full zoom-in/out
                MaxZoomOut = .733f;
                MaxZoomIn = 200f;
            }
            else
            {
                MaxZoomOut = World.StaticMaxZoomOut;
                MaxZoomIn = 200f; // Full zoom-in, Partial zoom-out
            }

            // Focus on player's dragon lord.
            if (!GameClass.Game.ShowChat && Keys.Space.Down())
            {
                var dl_pos = PrevDragonLordPos[MyPlayerNumber];

                if (dl_pos > vec(1,1))
                {
                    CameraPos = GridToWorldCood(dl_pos);
                    CameraZoom = 24;
                }
            }

            // Zoom all the way out
            //if (!GameClass.Game.ShowChat && Keys.Space.Down())
            //{
            //    CameraZoom = MaxZoomOut;
            //}

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
            if (!GameClass.Game.ShowChat && (Buttons.X.Down() || Keys.X.Down() || Keys.E.Down())) CameraZoom /= KeyZoomRate;
            else if (!GameClass.Game.ShowChat && (Buttons.A.Down() || Keys.Z.Down() || Keys.Q.Down())) CameraZoom *= KeyZoomRate;

            if (CameraZoom < MaxZoomOut) CameraZoom = MaxZoomOut;
            if (CameraZoom > MaxZoomIn) CameraZoom = MaxZoomIn;

            if (GameClass.MouseEnabled && !(Buttons.A.Pressed() || Buttons.X.Pressed()))
            {
                if (MouseOverMinimap)
                {
                    //var zoom_center = (UiMousePos - MinimapQuad.pos) / MinimapQuad.size;

                    //var shifted = GetShiftedCameraMinimap(Input.CurMousePos, camvec, zoom_center);
                    //CameraPos = shifted;
                }
                else
                {
                    vec2 zoom_center = world_mouse_pos;

                    var shifted = GetShiftedCamera(Input.CurMousePos, camvec, zoom_center);
                    CameraPos = shifted;
                }
            }

            // Move the camera via: Click And Drag
            //float MoveRate_ClickAndDrag = .00165f * FpsRateModifier;
            //if (Input.LeftMouseDown)
            //    CameraPos += Input.DeltaMousPos / CameraZoom * MoveRate_ClickAndDrag * new vec2(-1, 1);

            // Move the camera via: Push Edge
            if (GameClass.MouseEnabled &&
               (GameClass.Game.CurrentConfig.Fullscreen || BoxSelecting) &&
               (!Program.DisableScreenEdge || BoxSelecting))
            {
                float MoveRate_PushEdge = .075f * FpsRateModifier;
                var push_dir = vec2.Zero;
                float EdgeRatio = .005f;
                push_dir.x += -CoreMath.Restrict(0, 1, (EdgeRatio * GameClass.Screen.x - Input.CurMousePos.x) / (EdgeRatio * GameClass.Screen.x));
                push_dir.x += CoreMath.Restrict(0, 1, (Input.CurMousePos.x - (1 - EdgeRatio) * GameClass.Screen.x) / (EdgeRatio * GameClass.Screen.x));
                push_dir.y -= -CoreMath.Restrict(0, 1, (EdgeRatio * GameClass.Screen.y - Input.CurMousePos.y) / (EdgeRatio * GameClass.Screen.y));
                push_dir.y -= CoreMath.Restrict(0, 1, (Input.CurMousePos.y - (1 - EdgeRatio) * GameClass.Screen.y) / (EdgeRatio * GameClass.Screen.y));

                CameraPos += push_dir / CameraZoom * MoveRate_PushEdge;
            }

            // Move the camera via: Keyboard or Gamepad
            if (!GameClass.Game.ShowChat && !(MapEditorActive && InputHelper.CtrlDown()))
            {
                var dir = Input.Direction();

                float MoveRate_Keyboard = .07f * FpsRateModifier;
                CameraPos += dir / CameraZoom * MoveRate_Keyboard;                
            }

            // Move the camera via: Minimap
            if ((LeftMouseDown || Input.DeltaMouseScroll != 0) && !BoxSelecting && MouseOverMinimap)
            {
                CameraPos = MinimapWorldPos();
            }


            // Make sure the camera doesn't go too far offscreen
            y_edge = 1 + 0.433f / CameraZoom;
            x_edge = Math.Max(.5f * (CameraAspect / CameraZoom) + .5f * (CameraAspect / MaxZoomOut), 1); // Old style zoom out bounds.
            x_edge = Math.Min(x_edge, y_edge * CameraAspect);
            x_edge = CoreMath.LogLerpRestrict(MaxZoomIn, 1 + .35f / CameraZoom, MaxZoomOut, x_edge, CameraZoom);

            if (CameraZoom == MaxZoomOut) CameraPos = vec(0, -0.07f);

            var TR = ScreenToWorldCoord(new vec2(GameClass.Screen.x, 0));
            if (TR.x > x_edge) CameraPos = new vec2(CameraPos.x - (TR.x - x_edge), CameraPos.y);
            if (TR.y > y_edge) CameraPos = new vec2(CameraPos.x, CameraPos.y - (TR.y - y_edge));
            var BL = ScreenToWorldCoord(new vec2(0, GameClass.Screen.y));
            if (BL.x < -x_edge) CameraPos = new vec2(CameraPos.x - (BL.x + x_edge), CameraPos.y);
            if (BL.y < -y_edge) CameraPos = new vec2(CameraPos.x, CameraPos.y - (BL.y + y_edge));

            
            // Switch to chat
            if (!GameClass.Game.ShowChat && Keys.Enter.Pressed() && ChatInhibitor <= 0)
            {
                GameClass.Game.ChatGlobal = !(Keys.LeftShift.Down() || Keys.RightShift.Down());
                GameClass.Game.ToggleChatViaFlag();
            }
            if (GameClass.Game.ShowChat) { ChatInhibitor = 5; return; }
            if (ChatInhibitor > 0 && !Keys.Enter.Down()) ChatInhibitor--;

            // Switch input modes

            // Switch to spells (must be playing, not in editor)
            if (!SimulationPaused)
            {
                if (Keys.D1.Pressed()) StartSpell(Spells.Fireball);
                if (Keys.D2.Pressed()) StartSpell(Spells.SkeletonArmy);
                if (Keys.D3.Pressed()) StartSpell(Spells.Necromancer);
                if (Keys.D4.Pressed()) StartSpell(Spells.TerracottaArmy);
            }

            // Switch to building placement
            if (Keys.B.Down()) StartPlacingBuilding(UnitType.Barracks);
            if (Keys.G.Down()) StartPlacingBuilding(UnitType.GoldMine);
            if (Keys.J.Down()) StartPlacingBuilding(UnitType.JadeMine);

            // Switch to standard select
            if (Keys.Escape.Down() || Keys.Back.Down() || Input.RightMousePressed)
            {
                SetModeToSelect();
            }

            // Switch to unit placement (editor only)
            if (MapEditorActive)
            {
                if (Keys.R.Down()) StartUnitPaint(UnitType.Footman);
                if (Keys.T.Down()) StartUnitPaint(UnitType.DragonLord);
                if (Keys.Y.Down()) StartUnitPaint(UnitType.Necromancer);
                if (Keys.U.Down()) StartUnitPaint(UnitType.Skeleton);
                if (Keys.I.Down()) StartUnitPaint(UnitType.ClaySoldier);

                if (Keys.C.Down()) StartTilePaint(TileType.Dirt);
                if (Keys.V.Down()) StartTilePaint(TileType.Grass);
                if (Keys.N.Down()) StartTilePaint(TileType.Trees);

                if (Keys.Tab.Pressed())
                {
                    SetUnitPlaceStyle((int)Math.Round(UnitPlaceStyle) + 1);
                }
            }
        }

        public void SetModeToSelect()
        {
            if (CurUserMode == UserMode.PlaceBuilding || CurUserMode == UserMode.CastSpell)
                SkipNextSelectionUpdate = true;

            CurUserMode = UserMode.Select;
            SkipDeselect = true;
        }

        public void SetUnitPlaceStyle(int style)
        {
            UnitPlaceStyle = style;
            
            if (UnitPlaceStyle < UnitDistribution.First || UnitPlaceStyle >= UnitDistribution.Last)
            {
                UnitPlaceStyle = UnitDistribution.First;
            }

            if (UnitPlaceStyle == UnitDistribution.OnCorpses)
                UnitPlaceStyle = UnitDistribution.Single;

            GameClass.Game.UpdateEditorJsData();
        }

        public void SetUnitPaint(int type)
        {
            StartUnitPaint(_[type]);
        }
        public void StartUnitPaint(float type)
        {
            TileUserIsPlacing = TileType.None;
            UnitUserIsPlacing = type;
            CurUserMode = UserMode.Painting;
            UnselectAll = true;
            GameClass.Game.UpdateEditorJsData();
        }

        public void SetTilePaint(int type)
        {
            StartTilePaint(_[type]);
        }
        public void StartTilePaint(float type)
        {
            UnitUserIsPlacing = UnitType.None;
            TileUserIsPlacing = type;
            CurUserMode = UserMode.Painting;
            UnselectAll = true;
            GameClass.Game.UpdateEditorJsData();
        }


        public void Start(string name)
        {
            StartPlacingBuilding(name);
            StartSpell(name);
        }

        public void StartPlacingBuilding(string name)
        {
            if (!Params.Buildings.ContainsKey(name)) return;
            StartPlacingBuilding(Params.Buildings[name].UnitType);
        }
        public void StartPlacingBuilding(float building)
        {
            CurUserMode = UserMode.PlaceBuilding;
            UnselectAll = true;
            BuildingUserIsPlacing = building;
        }

        public void StartSpell(string name)
        {
            if (!Spells.SpellDict.ContainsKey(name)) return;
            StartSpell(Spells.SpellDict[name]);
        }
        public void StartSpell(Spell spell)
        {
            CurSpell = spell;
            CurUserMode = UserMode.CastSpell;
            UnselectAll = false;
        }

        public int
            /// The simulation step the GPU sim is currently on.
            SimStep = 0,
            /// The simulation step the server is on or is nearly on.
            ServerSimStep = 0,
            /// The simulation step that any acknowledged actions should occur on.
            AckSimStep = 0,
            /// The minimum simulation step of any client/server.
            MinClientSimStep = 0;


        /// <summary>
        /// After the simulation updates there are additional updates that must occur.
        /// These are broken into phases. This variable trackes which phase we are currently in.
        /// </summary>
        int PostUpdateStep = 0;

        /// <summary>
        /// After the simulation updates there are additional updates that must occur.
        /// This flag is true once these additional updates have finished.
        /// </summary>
        bool PostUpdateFinished = false;

        void SimulationUpdate()
        {
            SimStep++;
            PostUpdateFinished = false;
            PostUpdateStep = 0;

            PostSimulationUpdate();
        }

        void FullUpdate()
        {
            SimStep++;
            PostUpdateFinished = false;
            PostUpdateStep = 0;

            while (!PostUpdateFinished)
                PostSimulationUpdate();
        }

        void PostSimulationUpdate()
        {
            //Render.UnsetDevice();

            switch (PostUpdateStep)
            {
                case 0:
                    DataGroup.UpdateSelect();

                    if (MapEditorActive)
                        DataGroup.EditorSimulationUpdate();
                    else
                        DataGroup.SimulationUpdate();

                    UpdateDragonLordTracking();

                    break;

                case 1:
                    DataGroup.UpdateIcons();
                    DataGroup.DoDragonLordCount(PlayerInfo); // This should happen soon after CurrentUnit.anim is updated, so it can catch the death switch with low latency.
                    DragonLordDeathCheck();
                    EndOfGameCheck();
                    AddDragonLordDeathEffects();

                    break;

                case 2:
                    if (SimStep % 2 == 0)
                        DataGroup.DoGoldMineCount(PlayerInfo);
                    else
                        DataGroup.DoJadeMineCount(PlayerInfo);

                    DoGoldUpdate();
                    DoJadeUpdate();

                    break;

                case 3:
                    UpdateAllPlayerUnitCounts();
                    UpdateFoley();
                    break;

                case 4:
                    DataGroup.UpdateGradients();
                    break;

                case 5:
                    DataGroup.UpdateMagicFields();
                    DataGroup.UpdateMagicAuras();
                    break;

                case 6:
                    DataGroup.UpdateRnd();
                    DataGroup.UpdateMagicAuras(); // 2nd auro update

                    break;

                case 7:
                    if (SimStep % 2 == 0)
                        UpdateDragonLordEngaged();
                    else
                        UpdateMinimap();

                    break;

                default:
                    //HashCheck();
                    PostUpdateFinished = true;
                    break;
            }

            PostUpdateStep++;
        }

        private void HashCheck(bool Send = false)
        {
            string hash_string = "";
            int hash = 0;

            //string curdata_hash = DataGroup.DoHash(DataGroup.CurrentData, DataHash.Apply);
            //string prevdata_hash = DataGroup.DoHash(DataGroup.PreviousData, DataHash.Apply);
            //string curunit_hash = DataGroup.DoHash(DataGroup.CurrentUnits);
            //string prevunit_hash = DataGroup.DoHash(DataGroup.PreviousUnits);
            //string target_hash = DataGroup.DoHash(DataGroup.TargetData);
            //string extra_hash = DataGroup.DoHash(DataGroup.Extra);
            //hash_string = string.Format("Hash = {0} {1} {2} {3} {4} {5}", curdata_hash, prevdata_hash, curunit_hash, prevunit_hash, target_hash, extra_hash, target_hash, extra_hash);
            //hash_string = curdata_hash;
            //hash_string = curunit_hash;
            //for (int i = 1; i <= 4; i++) hash_string += PlayerInfo[i].ToString();
            for (int i = 1; i <= 4; i++) hash_string += PlayerInfo[i].Units.ToString() + '|';
            
            hash = hash_string.GetHashCode();

            if (Log.Hash)
            {
                Console.WriteLine("Hash = {0}", hash_string);
                Console.WriteLine("Hash code = {0}", hash);
            }

            if (Send)
            {
                //Hashes[SimStep] = hash;
                //Networking.ToServer(new MessageHash(SimStep, hash));

                StringHashes[SimStep] = hash_string;
                Networking.ToServer(new MessageStringHash(SimStep, hash_string));
            }
        }

        void AddDragonLordDeathEffects()
        {
            for (int p = 1; p <= 4; p++)
            {
                var player = PlayerInfo[p];

                if (player.CreateDragonLordDeathEffect)
                {
                    player.DragonLordDeathPos = PrevDragonLordPos[p];
                    DragonLordDeath(player.DragonLordDeathPos, p);

                    if (!GameOver) Sounds.DyingDragonLord.MaybePlay();

                    player.CreateDragonLordDeathEffect = false;
                }
            }
        }

        void DragonLordDeathCheck()
        {
            for (int t = 1; t <= 4; t++)
            {
                TeamInfo[t].DragonLordCount = 0;
            }

            for (int p = 1; p <= 4; p++)
            {
                var player = PlayerInfo[p];
                var team = TeamInfo[PlayerTeams[p]];

                if (player.Defeated && !player.DestroyedAllBarracks && player.DefeatedFrameStamp + 6 < SimStep)
                {
                    player.DestroyedAllBarracks = true;
                    DestroyAllPlayerBuildings(Player.Vals[p]);
                }

                if (player.DragonLordAlive)
                {
                    if (player.DragonLords == 0)
                    {
                        player.DragonLordAlive = false;
                        player.Defeated = true;
                        player.DefeatedFrameStamp = SimStep;

                        if (!MapEditorActive)
                        {
                            player.CreateDragonLordDeathEffect = true;
                            player.DragonLordDeathPos = CurDragonLordPos[p];
                        }
                    }
                }
                else
                {
                    if (player.DragonLords > 0)
                    {
                        player.DragonLordAlive = true;
                    }
                }

                team.DragonLordCount += player.DragonLords;
            }
        }

        public bool GameOver = false;
        public double GameOverTime = 0;
        public vec2 GameOverPos;
        public float GameOverZoom;
        void EndOfGameCheck()
        {
            if (GameOver || MapEditor) return;

            int alive_count = 0;
            int winning_team = -1;
            for (int t = 1; t <= 4; t++)
            {
                var team = TeamInfo[t];

                if (team.DragonLordCount == 0)
                {
                    team.Defeated = true;
                }
                else
                {
                    winning_team = t;
                    team.Defeated = false;
                    alive_count++;
                }
            }

            if (alive_count <= 1)
            {
                GameOver = true;
                GameOverTime = GameClass.T;
                GameOverPos = CameraPos;
                GameOverZoom = CameraZoom;

                if (MyTeamInfo == null)
                {
                    GameClass.Game.GameOver(winning_team);
                }
                else if (MyTeamInfo.Defeated)
                {
                    GameClass.Game.Defeat(winning_team);
                }
                else
                {
                    GameClass.Game.Victory(winning_team);
                }
            }
        }

        void DoGoldUpdate()
        {
            if (MapEditorActive) return;

            for (int player = 1; player <= 4; player++) PlayerInfo[player].GoldUpdate();
        }

        void DoJadeUpdate()
        {
            if (MapEditorActive) return;

            for (int player = 1; player <= 4; player++) PlayerInfo[player].JadeUpdate();
        }

        bool TrackDragonLord = true;
        vec2[] CurDragonLordPos = new vec2[] { vec2.Zero, vec2.Zero, vec2.Zero, vec2.Zero, vec2.Zero };
        vec2[] PrevDragonLordPos = new vec2[] { vec2.Zero, vec2.Zero, vec2.Zero, vec2.Zero, vec2.Zero };

        private void UpdateDragonLordTracking()
        {
            if (TrackDragonLord)
            {
                for (int player = 1; player <= 4; player++)
                {
                    PrevDragonLordPos[player] = CurDragonLordPos[player];
                    CurDragonLordPos[player] = DataGroup.DragonLordPos(_[player]);
                    if (PrevDragonLordPos[player] == vec2.Zero) PrevDragonLordPos[player] = CurDragonLordPos[player];
                }
            }
        }

        void UpdateFoley()
        {
            var count = DataGroup.DoActionCount(this);

            float zoom = (float)Math.Pow(CameraZoom / 80f, 1.25f);

            ThreeLevelPlay(
                AmbientSounds.SwordFight_Level1, _6,
                AmbientSounds.SwordFight_Level2, _30,
                AmbientSounds.SwordFight_Level3,
                count.UnitsAttacking, count.UnitsAttacking * zoom);

            ThreeLevelPlay(
                AmbientSounds.Walking_Level1, _6,
                AmbientSounds.Walking_Level2, _30,
                AmbientSounds.Walking_Level3,
                count.UnitsMoving, count.UnitsMoving * zoom);

            if (count.BuildingsExploding > 0)
            {
                Sounds.BuildingExplode.MaybePlay(1.25f * zoom);
            }

            if (count.UnitsDying > 0 && count.UnitsDying < _3)
            {
                Sounds.DyingUnit.MaybePlay(1.25f * zoom);
            }
        }

        private static void ThreeLevelPlay(
            AmbientSound s1, float l1,
            AmbientSound s2, float l2,
            AmbientSound s3,
            float count, float volume)
        {
            if (count < _6)
            {
                s1.EaseIntoVolume(volume);
                s2.EaseIntoVolume(0);
                s3.EaseIntoVolume(0);
            }
            else if (count < _30)
            {
                s1.EaseIntoVolume(0);
                s2.EaseIntoVolume(volume);
                s3.EaseIntoVolume(0);
            }
            else
            {
                s1.EaseIntoVolume(0);
                s2.EaseIntoVolume(0);
                s3.EaseIntoVolume(volume);
            }
        }
    }
}
