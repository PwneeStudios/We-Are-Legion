using System;
using System.Linq;

using FragSharpFramework;

namespace Game
{
    [Copy(typeof(vec4))]
    public partial struct corpse
    {
        [Hlsl("r")]
        public float direction { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float type { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float player { get { return b; } set { b = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct tile
    {
        [Hlsl("r")]
        public float type { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float i { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float j { get { return b; } set { b = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct magic
    {
        [Hlsl("r")]
        public float kill { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float raising_player { get { return g; } set { g = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct geo
    {
        [Hlsl("r")]
        public float dir { get { return r; } set { r = value; } }

        [Hlsl("a")]
        public float bad { get { return a; } set { a = value; } }

        [Hlsl("gba")]
        public vec3 pos_storage { get { return gba; } set { gba = value; } }

        [Hlsl("ba")]
        public vec2 geo_id { get { return ba; } set { ba = value; } }

        [Hlsl("g")]
        public float dist { get { return g; } set { g = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct dirward
    {
        [Hlsl("rg")]
        public vec2 geo_id { get { return rg; } set { rg = value; } }

        [Hlsl("g")]
        public float polarity_set { get { return g; } set { g = value; } }

        [Hlsl("g")]
        public float importance { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float dist_to_wall { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float polarity { get { return a; } set { a = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct geo_info
    {
        [Hlsl("rg")]
        public vec2 polar_dist { get { return rg; } set { rg = value; } }

        [Hlsl("ba")]
        public vec2 circumference { get { return ba; } set { ba = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct unit
    {
        [Hlsl("r")]
        public float type { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float player { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float team { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float anim { get { return a; } set { a = value; } }

        [Hlsl("a")]
        public float hit_count { get { return a; } set { a = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct data
    {
        [Hlsl("r")]
        public float direction { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float change { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float prior_direction_and_select { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float action { get { return a; } set { a = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct building
    {
        [Hlsl("r")]
        public float direction { get { return r; } set { r = value; } }

        [Hlsl("b")]
        public float prior_direction_and_select { get { return b; } set { b = value; } }

        [Hlsl("g")]
        public float part_x { get { return g; } set { g = value; } }

        [Hlsl("a")]
        public float part_y { get { return a; } set { a = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct extra
    {
        [Hlsl("rg")]
        public vec2 geo_id { get { return rg; } set { rg = value; } }

        [Hlsl("b")]
        public float polarity_set { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float polarity { get { return a; } set { a = value; } }
    }

    [Copy(typeof(vec4))]
    public partial struct BuildingDist
    {
        [Hlsl("rg")]
        public vec2 diff { get { return rg; } set { rg = value; } }

        [Hlsl("r")]
        public float diff_x { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float diff_y { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float player_and_type { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float dist { get { return a; } set { a = value; } }
    }

    /// <summary>
    /// A 4-tuple storying an 8-bit value unit count for moving, attacking, dying, building exploding.
    /// </summary>
    [Copy(typeof(vec4), CastStyle.ImplicitCast)]
    public partial struct ActionCount
    {
        [Hlsl("r")]
        public float UnitsMoving { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float UnitsAttacking { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float UnitsDying { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float BuildingsExploding { get { return a; } set { a = value; } }
    }

    /// <summary>
    /// A 4-tuple storying an 8-bit value for Player One, Player Two, Player Three, and Player Four.
    /// </summary>
    [Copy(typeof(vec4), CastStyle.ImplicitCast)]
    public partial struct PlayerTuple
    {
        [Hlsl("r")]
        public float PlayerOne { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float PlayerTwo { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float PlayerThree { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float PlayerFour { get { return a; } set { a = value; } }
    }

    /// <summary>
    /// A 4-tuple storying an 8-bit value for Team One, Team Two, Team Three, and Team Four.
    /// </summary>
    [Copy(typeof(vec4), CastStyle.ImplicitCast)]
    public partial struct TeamTuple
    {
        [Hlsl("r")]
        public float TeamOne { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float TeamTwo { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float TeamThree { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float TeamFour { get { return a; } set { a = value; } }
    }

    public class BadPlayerNumberException : Exception
    {
        float player;
        public BadPlayerNumberException(float player) 
            : base(string.Format("Incorrect player number {0}", player))
        {
            this.player = player;
        }
    }

    public class BadTeamNumberException : Exception
    {
        float team;
        public BadTeamNumberException(float team)
            : base(string.Format("Incorrect team number {0}", team))
        {
            this.team = team;
        }
    }

    public partial class SimShader : GridComputation
    {
        public static int[] IntArray(params float[] vals)
        {
            return vals.Select(v => Int(v)).ToArray();
        }

        public const float eps = .001f;

        public const float _true = _1, _false = _0;

        public static float GetPlayerVal(PlayerTuple tuple, float player)
        {
            if (player == Player.One) return tuple.PlayerOne;
            if (player == Player.Two) return tuple.PlayerTwo;
            if (player == Player.Three) return tuple.PlayerThree;
            if (player == Player.Four) return tuple.PlayerFour;

            throw new BadPlayerNumberException(player);
            return 0;
        }

        public static float GetPlayerVal(PlayerTuple tuple, int player)
        {
            if (player == 1) return tuple.PlayerOne;
            if (player == 2) return tuple.PlayerTwo;
            if (player == 3) return tuple.PlayerThree;
            if (player == 4) return tuple.PlayerFour;

            //throw new BadPlayerNumberException(player);
            return 0;
        }

        public static void SetPlayerVal(ref PlayerTuple tuple, float player, float value)
        {
            if (player == Player.One) tuple.PlayerOne = value;
            if (player == Player.Two) tuple.PlayerTwo = value;
            if (player == Player.Three) tuple.PlayerThree = value;
            if (player == Player.Four) tuple.PlayerFour = value;

            throw new BadPlayerNumberException(player);
        }

        public static void SetPlayerVal(ref PlayerTuple tuple, int player, float value)
        {
            if (player == 1) tuple.PlayerOne = value;
            if (player == 2) tuple.PlayerTwo = value;
            if (player == 3) tuple.PlayerThree = value;
            if (player == 4) tuple.PlayerFour = value;

            throw new BadPlayerNumberException(player);
        }

        public static float GetTeamVal(TeamTuple tuple, float team)
        {
            if (team == Team.One) return tuple.TeamOne;
            if (team == Team.Two) return tuple.TeamTwo;
            if (team == Team.Three) return tuple.TeamThree;
            if (team == Team.Four) return tuple.TeamFour;

            throw new BadTeamNumberException(team);
            return 0;
        }

        public static float GetTeamVal(TeamTuple tuple, int team)
        {
            if (team == 1) return tuple.TeamOne;
            if (team == 2) return tuple.TeamTwo;
            if (team == 3) return tuple.TeamThree;
            if (team == 4) return tuple.TeamFour;

            //throw new BadTeamNumberException(team);
            return 0;
        }

        public static void SetTeamVal(ref TeamTuple tuple, float team, float value)
        {
            if (team == Team.One) tuple.TeamOne = value;
            if (team == Team.Two) tuple.TeamTwo = value;
            if (team == Team.Three) tuple.TeamThree = value;
            if (team == Team.Four) tuple.TeamFour = value;

            throw new BadTeamNumberException(team);
        }

        public static void SetTeamVal(ref TeamTuple tuple, int team, float value)
        {
            if (team == 1) tuple.TeamOne = value;
            if (team == 2) tuple.TeamTwo = value;
            if (team == 3) tuple.TeamThree = value;
            if (team == 4) tuple.TeamFour = value;

            throw new BadTeamNumberException(team);
        }

        [Hlsl("float4")]
        protected static PlayerTuple PlayerTuple(float x, float y, float z, float w)
        {
            return vec(x, y, z, w);
        }

        [Hlsl("float4")]
        protected static TeamTuple TeamTuple(float x, float y, float z, float w)
        {
            return vec(x, y, z, w);
        }

        protected static building to_building(data d)
        {
            return (building)(vec4)d;
        }

        protected static float polar_dist(geo_info info)
        {
            return unpack_val(info.polar_dist);
        }

        protected static void set_polar_dist(ref geo_info info, float polar_dist)
        {
            info.polar_dist = pack_val_2byte(polar_dist);
        }

        protected static float circumference(geo_info info)
        {
            return unpack_val(info.circumference);
        }

        protected static void set_circumference(ref geo_info info, float circumference)
        {
            info.circumference = pack_val_2byte(circumference);
        }

        protected static vec2 geo_pos_id(geo g)
        {
            return unpack_vec2_3byte(g.pos_storage);
        }

        protected static void set_geo_pos_id(ref geo g, vec2 pos)
        {
            g.pos_storage = pack_vec2_3byte(pos);
        }


        const float type_offset = 16;
        protected static float get_type(BuildingDist u)
        {
            return fint_round(u.player_and_type / type_offset);
        }

        protected static void set_type(ref BuildingDist u, float type)
        {
            u.player_and_type = get_player(u) + type * type_offset;
        }

        protected static float get_player(BuildingDist u)
        {
            return u.player_and_type - get_type(u) * type_offset;
        }

        protected static void set_player(ref BuildingDist u, float player)
        {
            u.player_and_type = player + get_type(u) * type_offset;
        }



        public static class SelectState
        {
            public const float
                NotSelected_NoShow = 0 * Shift,
                NotSelected_Show1 = 1 * Shift,
                NotSelected_Show2 = 2 * Shift,
                Selected_Show = 3 * Shift,
                Selected_NoShow1 = 4 * Shift,
                Selected_NoShow2 = 5 * Shift,

                Selected = Selected_Show,
                FirstShow = NotSelected_Show1,
                LastShow = Selected_Show + Shift,

                Shift = _32;
        }

        protected static bool fake_selected(building u) { return fake_selected((data)(vec4)u); }
        protected static bool fake_selected(data u)
        {
            float val = u.prior_direction_and_select;
            return SelectState.FirstShow <= val && val < SelectState.LastShow;
        }

        protected static void set_selected_fake(ref building u, bool selected) { data d = (data)(vec4)u; set_selected_fake(ref d, selected); u = (building)(vec4)d; }
        protected static void set_selected_fake(ref data u, bool fake_selected)
        {
            bool is_selected = selected(u);
            float prior_dir = prior_direction(u);

            float select_state;
            if (fake_selected)
            {
                select_state = is_selected ? SelectState.Selected_Show : SelectState.NotSelected_Show2;
            }
            else
            {
                select_state = is_selected ? SelectState.Selected_NoShow2 : SelectState.NotSelected_NoShow;
            }

            u.prior_direction_and_select = prior_dir + select_state;
        }

        protected static bool selected(building u) { return selected((data)(vec4)u); }
        protected static bool selected(data u)
        {
            float val = u.prior_direction_and_select;
            return val >= SelectState.Selected;
        }

        protected static void set_selected(ref building u, bool selected) { data d = (data)(vec4)u; set_selected(ref d, selected); u = (building)(vec4)d; }
        protected static void set_selected(ref data u, bool selected)
        {
            float state = select_state(u);
            if (selected)
            {
                state = fake_selected(u) ? SelectState.Selected_Show : SelectState.Selected_NoShow2;
            }
            else
            {
                state = fake_selected(u) ? SelectState.NotSelected_Show2 : SelectState.NotSelected_NoShow;
            }

            u.prior_direction_and_select = prior_direction(u) + state;
        }

        protected static float prior_direction(building u) { return prior_direction((data)(vec4)u); }
        protected static float prior_direction(data u)
        {
            float val = u.prior_direction_and_select;
            val = fmod(val, SelectState.Shift);

            // Subtracting SelectState.Selected leads to some inaccuracies in the resulting fint value.
            // We fix this problem by rounding to the nearest fint.
            // An alternative solution may be to raise our epsilon tolerance in equality tests.
            val = fint_round(val);

            return val;
        }

        protected static float select_state(building u) { return select_state((data)(vec4)u); }
        protected static float select_state(data u)
        {
            return u.prior_direction_and_select - prior_direction(u);
        }

        protected static void set_select_state(ref data u, float state)
        {
            u.prior_direction_and_select = prior_direction(u) + state;
        }

        protected static void set_prior_direction(ref data u, float dir)
        {
            u.prior_direction_and_select = select_state(u) + dir;
        }

        /*
        protected static bool selected(building u)
        {
            float val = u.prior_direction_and_select;
            return val >= SelectState.Selected;
        }

        protected static void set_selected(ref building u, bool selected)
        {
            u.prior_direction_and_select = prior_direction(u) + (selected ? SelectState.Selected : _0);
        }

        protected static float prior_direction(building u)
        {
            float val = u.prior_direction_and_select;
            if (val >= SelectState.Selected) val -= SelectState.Selected;
            return val;
        }

        protected static void set_prior_direction(ref building u, float dir)
        {
            u.prior_direction_and_select = dir + (selected(u) ? SelectState.Selected : _0);
        }
        */

        protected static bool ValidDirward(dirward d)
        {
            return d != dirward.Nothing;
        }

        protected static vec2 ReducedGeoId(vec2 p)
        {
            return vec(
                ((int)(round(p.x)) % 256) / 256.0f,
                ((int)(round(p.y)) % 256) / 256.0f
            );
        }

        readonly vec2 lookup_shift = new vec2(-0.25f, -0.25f);
        protected vec2 get_subcell_pos(VertexOut vertex, vec2 grid_size)
        {
            vec2 coords = vertex.TexCoords * grid_size + lookup_shift;
            float i = floor(coords.x);
            float j = floor(coords.y);

            return coords - vec(i, j);
        }

        protected vec2 get_subcell_pos(VertexOut vertex, vec2 grid_size, vec2 grid_shift)
        {
            vec2 coords = vertex.TexCoords * grid_size + grid_shift + lookup_shift;
            float i = floor(coords.x);
            float j = floor(coords.y);

            return coords - vec(i, j);
        }

        protected vec2 direction_to_vec(float direction)
        {
            float angle = (direction * 255 - 1) * (3.1415926f / 2.0f);
            return IsValid(direction) ? vec(cos(angle), sin(angle)) : vec2.Zero;
        }

        public static class TileType
        {
            [FragSharpFramework.Vals(Grass, Dirt, Trees)]
                public class ValsAttribute : Attribute { }

            public const float
                FirstBlockingTileType = Water;

            public const float
                None = _0,
                Grass = _1,
                Dirt = _2,
                Water = _3,
                Rocks = _4,
                Trees = _5;

            public static string Name(float tile)
            {
                if (tile == None) return "None";

                if (tile == Grass) return "Grass";
                if (tile == Dirt) return "Dirt";
                if (tile == Water) return "Water";
                if (tile == Rocks) return "Rocks";
                if (tile == Trees) return "Trees";

                return "Invalid Tile Type";
            }
        }

        public static class Team
        {
            [FragSharpFramework.Vals(None, One, Two, Three, Four)]
                public class ValsAttribute : Attribute { }

            public static readonly float[] Vals = new float[] { None, One, Two, Three, Four };

            public static float Get(int index)
            {
                return Vals[index];
            }

            public const float
                None = _0,
                One = _1,
                Two = _2,
                Three = _3,
                Four = _4;
        }

        public static class Player
        {
            public static float Num(unit u)
            {
                return Float(u.player) - 1;
            }

            [FragSharpFramework.Vals(None, One, Two, Three, Four)]
                public class ValsAttribute : Attribute { }

            public static readonly float[] Vals = new float[] { None, One, Two, Three, Four };

            public static float Get(int index)
            {
                return Vals[index];
            }

            public const float
                None = _0,
                One = _1,
                Two = _2,
                Three = _3,
                Four = _4;
        }

        public class SelectionFilter : BaseShader
        {
            [FragSharpFramework.Vals(All, Units, Buildings, Soldiers, Special)]
            public class ValsAttribute : Attribute { }

            public static readonly float[] Vals = new float[] { All, Units, Buildings, Soldiers, Special };

            public const float
                All = 0,
                Units = 1,
                Buildings = 2,
                Soldiers = 3,
                Special = 4,

                Count = 5,
                First = All;

            public static bool FilterHasUnit(float filter, float type)
            {
                if (filter == All)
                {
                    return true;
                }

                if (filter == Units)
                {
                    return IsUnit(type);
                }

                if (filter == Buildings)
                {
                    return IsBuilding(type);
                }

                if (filter == Soldiers)
                {
                    return IsSoldierUnit(type);
                }

                if (filter == Special)
                {
                    return IsSpecialUnit(type);
                }

                return false;
            }
        }

        public static class UnitType
        {
            [FragSharpFramework.Vals(Barracks, GoldMine, JadeMine)]
                public class BuildingsAttribute : Attribute { }

            [FragSharpFramework.Vals(Footman, DragonLord, Necromancer, Skeleton, ClaySoldier, Barracks, GoldMine, JadeMine)]
                public class ValsAttribute : Attribute { }

            public const float
                FirstUnitType = Footman,
                FirstBuildingType = Barracks,
                FirstBlockingTileType = BlockingTile;

            public static float UnitIndex(unit u)
            {
                return Float(u.type - FirstUnitType);
            }

            public static float UnitIndex(float type)
            {
                return type - FirstUnitType;
            }

            public static float BuildingIndex(float type)
            {
                return type - FirstBuildingType;
            }

            public const float
                None = _0,

                Footman     = _1,
                DragonLord  = _2,
                Necromancer = _3,
                Skeleton    = _4,
                ClaySoldier = _5,

                Barracks = _6,
                GoldMine = _7,
                JadeMine = _8,
                DragonLordIcon  = _9,
                NecromancerIcon = _10,

                BlockingTile = _20,
                
                Last = JadeMine,
                Count = Last;

            public static readonly float[] Vals = new float[] {
                Footman, DragonLord, Necromancer, Skeleton, ClaySoldier, Barracks, GoldMine, JadeMine,
            };

            public static readonly float[] BuildingVals = new float[] {
                Barracks, GoldMine, JadeMine,
            };

            public static string Name(float unit)
            {
                if (unit == None) return "None";

                if (unit == Footman) return "Soldier";
                if (unit == DragonLord) return "Dragon Lord";
                if (unit == Necromancer) return "Necromancer";
                if (unit == Skeleton) return "Skeleton Warrior";
                if (unit == ClaySoldier) return "Terracotta Soldier";

                if (unit == Barracks) return "Barracks";
                if (unit == GoldMine) return "Gold Mine";
                if (unit == JadeMine) return "Jade Mine";

                return "Invalid Unit Type";
            }
        }

        protected static bool LeavesCorpse(unit u)
        {
            return IsUnit(u) && u.type != UnitType.Skeleton;
        }

        protected static bool IsSpecialUnit(float type)
        {
            return type == UnitType.DragonLord || type == UnitType.Necromancer;
        }

        protected static bool IsSoldierUnit(float type)
        {
            return IsUnit(type) && !IsSpecialUnit(type);
        }

        protected static bool IsUnit(unit u)
        {
            return IsUnit(u.type);
        }

        protected static bool IsUnit(float type)
        {
            return type >= UnitType.FirstUnitType && type < UnitType.FirstBuildingType;
        }

        protected static bool IsBuilding(unit u)
        {
            return IsBuilding(u.type);
        }

        protected static bool IsBuilding(float type)
        {
            return type >= UnitType.FirstBuildingType && type < UnitType.FirstBlockingTileType;
        }

        protected static bool BlockingTileHere(unit u)
        {
            return u.type >= UnitType.FirstBlockingTileType;
        }

        protected static bool IsBlockingTile(tile t)
        {
            return t.type >= TileType.FirstBlockingTileType ||
                t.type == TileType.Grass && t.j != _31;
        }

        protected static bool IsCenter(building b)
        {
            return b.part_x == _1 && b.part_y == _1;
        }

        protected static bool IsStationary(data d)
        {
            return d.direction >= Dir.Stationary;
        }

        protected static bool IsMobile(data d)
        {
            return d.direction < Dir.Stationary;
        }

        protected static bool UnitIsFireImmune(unit u)
        {
            return u.type == UnitType.Necromancer || u.type == UnitType.DragonLord;
        }

        protected static float RndFint(float rnd, float f1, float f2)
        {
            f2 += _1;     // Add _1 to make the range inclusive.
            f2 -= .0006f; // Fudge downard slightly so that the fint_round doesn't decide to round up (due to epsilon equality)
            float val = rnd * (f2 - f1) + f1;
            return fint_floor(val);
            
            //float val = rnd * (f2 - f1) + f1;
            //return fint_round(val);
        }

        public static class Anim
        {
            public const float
                Stand  = _0 * UnitSpriteSheet.AnimLength,
                Walk   = _1 * UnitSpriteSheet.AnimLength,
                Attack = _2 * UnitSpriteSheet.AnimLength,
                Die    = _3 * UnitSpriteSheet.AnimLength,
                Dead   = _4 * UnitSpriteSheet.AnimLength,
                
                StartRaise = _10 * UnitSpriteSheet.AnimLength,
                DoRaise    = _11 * UnitSpriteSheet.AnimLength;
        }

        public static class Part
        {
            public const float
                Center = _0,
                Right  = _1,
                TR     = _2,
                Up     = _3,
                TL     = _4,
                Left   = _5,
                BL     = _6,
                Down   = _7,
                BR     = _8,
                Count  = _9;
        }

        public static class DebugArrowsSpriteSheet
        {
            public const int SheetDimX = 32;
            public const int SheetDimY = 4;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class DebugNumSpriteSheet
        {
            public const int SheetDimX = 128;
            public const int SheetDimY = 4;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class TileSpriteSheet
        {
            public const int SheetDimX = 32;
            public const int SheetDimY = 32;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class UnitSpriteSheet
        {
            public const int AnimLength = 6;
            public const int NumAnims = 4;
            //public const int SheetDimX = NumAnims * AnimLength;
            //public const int SheetDimY = 2 /*Selected,Unselected*/ * 4 /*4 Directions*/;
            public const int SheetDimX = 32;
            public const int SheetDimY = 3 * 32;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class BuildingSpriteSheet
        {
            public const int BuildingDimX = 3;
            public const int BuildingDimY = 3;
            public static readonly vec2 BuildingDim = vec(BuildingDimX, BuildingDimY);
            public static readonly vec2 BuildingSize = vec(BuildingDimX / (float)SheetDimX, BuildingDimY / (float)SheetDimY);

            public const int UnitTypes = 5;
            public const int SubsheetDimX = 1;
            public const int SubsheetDimY = 2 /*Selected,Unselected*/ * BuildingDimY;
            public static readonly vec2 SubsheetSize = vec(SubsheetDimX / (float)SheetDimX, SubsheetDimY / (float)SheetDimY);

            public const int AnimLength = 1;
            public const int NumAnims = 1;
            public const int SheetDimX = 5 * NumAnims * AnimLength * BuildingDimX;
            public const int SheetDimY = UnitTypes * SubsheetDimY;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class ExplosionSpriteSheet
        {
            public static float ExplosionFrame(float s, building building_here)
            {
                return (s + 255 * (building_here.direction - Dir.StationaryDead)) * 6;
            }

            public const int DimX = 3;
            public const int DimY = 3;
            public static readonly vec2 Dim = vec(DimX, DimY);
            public static readonly vec2 Size = vec(DimX / (float)SheetDimX, DimY / (float)SheetDimY);

            public const int AnimLength = 16;
            public const int SheetDimX = AnimLength * DimX;
            public const int SheetDimY = DimY;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class Dir
        {
            public static float Num(data d)
            {
                return Float(d.direction) - 1;
            }

            [FragSharpFramework.Vals(Right, Up, Left, Down)]
                public class ValsAttribute : Attribute { }

            public static readonly float[] Vals = new float[] { Right, Up, Left, Down };

            public const float
                None = _0,
                Right = _1,
                Up = _2,
                Left = _3,
                Down = _4,
                
                Stationary = _5,
                
                StationaryDying = _6,
                StationaryDead = _7,
                
                Count = _8,

                TurnRight = -_1,
                TurnLeft = _1;
        }

        protected static class Change
        {
            public const float
                Moved = _0,
                Stayed = _1;
        }

        protected static class SetPolarity
        {
            public const float
                Clockwise = _10,
                Counterclockwise = _100;
        }

        protected static class Polarity
        {
            public const float
                Clockwise = 0,
                Counterclockwise = 1,

                Undefined = -1;
        }

        public static class UnitAction
        {
            public const float
                Stopped = _0,
                Moving = _1,
                Attacking = _2,
                Guard = _3,
                Spawning = _4,
                Raising = _5,
                Count = _6,

                NoChange = _12;
        }

        protected static bool Stayed(data u)
        {
            return IsStationary(u) || u.change == Change.Stayed;
        }

        protected static bool Moved(data u)
        {
            return !IsStationary(u) && u.change == Change.Moved;
        }

        protected static bool SomethingFakeSelected(data u)
        {
            return Something(u) && fake_selected(u);
        }

        protected static bool SomethingSelected(data u)
        {
            return Something(u) && selected(u);
        }

        protected static bool Something(data u)
        {
            return u.direction > 0;
        }

        protected static bool CorpsePresent(corpse u)
        {
            return u.direction > 0;
        }

        protected static bool Something(building u)
        {
            return u.direction > 0;
        }

        protected static bool IsValid(float direction)
        {
            return direction > 0;
        }

        protected static void TurnLeft(ref data u)
        {
            u.direction += Dir.TurnLeft;
            if (u.direction > Dir.Down)
                u.direction = Dir.Right;
        }

        protected static void TurnRight(ref data u)
        {
            u.direction += Dir.TurnRight;
            if (u.direction < Dir.Right)
                u.direction = Dir.Down;
        }

        protected static void TurnAround(ref data u)
        {
            u.direction += 2 * Dir.TurnLeft;
            if (u.direction > Dir.Down)
                u.direction -= 4 * Dir.TurnLeft;
        }

        protected static float RotateLeft(float dir)
        {
            dir += Dir.TurnLeft;
            if (dir > Dir.Down)
                dir = Dir.Right;
            return dir;
        }

        protected static float RotateRight(float dir)
        {
            dir += Dir.TurnRight;
            if (dir < Dir.Right)
                dir = Dir.Down;
            return dir;
        }

        protected static float Reverse(float dir)
        {
            dir += 2 * Dir.TurnLeft;
            if (dir > Dir.Down)
                dir -= 4 * Dir.TurnLeft;
            return dir;
        }        

        protected static RelativeIndex dir_to_vec(float direction)
        {
            float angle = (float)((direction * 255 - 1) * (3.1415926 / 2.0));
            return IsValid(direction) ? new RelativeIndex(cos(angle), sin(angle)) : new RelativeIndex(0, 0);
        }

        protected static RelativeIndex center_dir(building b)
        {
            vec2 part = vec(b.part_x, b.part_y);
            part = -255 * (part - vec(_1, _1));

            return (RelativeIndex)part;
        }

        public static vec3 pack_coord_3byte(float x)
        {
            vec3 packed = vec3.Zero;

            packed.x = floor(x / (255.0f * 255.0f));
            packed.y = floor((x - packed.x * (255.0f * 255.0f)) / 255.0f);
            packed.z = x - packed.x * (255.0f * 255.0f) - packed.y * 255.0f;

            return packed / 255.0f;
        }

        public static vec3 pack_val_3byte(float x)
        {
            vec3 packed = vec3.Zero;

            packed.x = floor(x / (255.0f * 255.0f));
            packed.y = floor((x - packed.x * (255.0f * 255.0f)) / 255.0f);
            packed.z = x - packed.x * (255.0f * 255.0f) - packed.y * 255.0f;

            return packed / 255.0f;
        }

        public static float unpack_val(vec3 packed)
        {
            float coord = 0;

            coord = (255 * 255 * packed.x + 255 * packed.y + packed.z) * 255;

            return coord;
        }

        public static vec2 pack_val_2byte(float x)
        {
            vec2 packed = vec2.Zero;

            packed.x = floor(x / 256.0f);
            packed.y = x - packed.x * 256.0f;

            return packed / 255.0f;
        }

        public static float unpack_val(vec2 packed)
        {
            float coord = 0;

            packed = floor(255.0f * packed + vec(.5f, .5f));
            coord = 256 * packed.x + packed.y;

            return coord;
        }

        public static vec4 pack_vec2(vec2 v)
        {
            vec2 packed_x = pack_val_2byte(v.x);
            vec2 packed_y = pack_val_2byte(v.y);
            return vec(packed_x.x, packed_x.y, packed_y.x, packed_y.y);
        }

        public static vec2 unpack_vec2(vec4 packed)
        {
            vec2 v = vec2.Zero;
            v.x = unpack_val(packed.rg);
            v.y = unpack_val(packed.ba);
            return v;
        }


        public static vec2 pack_val_2byte_corrected(float x)
        {
            vec2 packed = vec2.Zero;

            packed.x = floor(x / 256.0f);
            packed.y = x - packed.x * 256.0f;

            return packed / 256.0f;
        }

        public static float unpack_val_corrected(vec2 packed)
        {
            float coord = 0;

            packed = floor(256.0f * packed + vec(.5f, .5f));
            coord = 256 * packed.x + packed.y;

            return coord;
        }

        public static vec4 pack_vec2_corrected(vec2 v)
        {
            vec2 packed_x = pack_val_2byte_corrected(v.x);
            vec2 packed_y = pack_val_2byte_corrected(v.y);
            return vec(packed_x.x, packed_x.y, packed_y.x, packed_y.y);
        }

        public static vec2 unpack_vec2_corrected(vec4 packed)
        {
            vec2 v = vec2.Zero;
            v.x = unpack_val_corrected(packed.rg);
            v.y = unpack_val_corrected(packed.ba);
            return v;
        }

        /// <summary>
        /// Packs a vec2 into a 3-byte vec3.
        /// </summary>
        /// <param name="x">The value to pack. Each component of the vector should be between 0 and 2^12 - 1.</param>
        /// <returns></returns>
        public static vec3 pack_vec2_3byte(vec2 v)
        {
            vec2 packed_x = pack_val_2byte(v.x);
            vec2 packed_y = pack_val_2byte(v.y);
            return vec(packed_x.y, packed_y.y, packed_x.x + 16 * packed_y.x);
        }

        public static vec2 unpack_vec2_3byte(vec3 packed)
        {
            float extra_bits = packed.z;
            //float extra_y = (extra_bits / 16);
            //float extra_x = fint_round(extra_bits - _16 * floor(extra_y / _1));
            float extra_y = fint_floor(extra_bits / 16);
            float extra_x = fint_floor(extra_bits - 16 * extra_y);

            vec2 v = vec2.Zero;
            v.x = unpack_val(vec(extra_x, packed.x));
            v.y = unpack_val(vec(extra_y, packed.y));
            return v;
        }
   }
}