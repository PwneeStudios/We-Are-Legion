using System;
using FragSharpFramework;

namespace GpuSim
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
        public float player { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float dist { get { return a; } set { a = value; } }
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

    public class SimShader : GridComputation
    {
        public const float _true = _1, _false = _0;

        public static float Get(PlayerTuple tuple, float player)
        {
            if (player == Player.One) return tuple.PlayerOne;
            if (player == Player.Two) return tuple.PlayerTwo;
            if (player == Player.Three) return tuple.PlayerThree;
            if (player == Player.Four) return tuple.PlayerFour;

            throw new BadPlayerNumberException(player);
            return 0;
        }

        public static float Get(PlayerTuple tuple, int player)
        {
            if (player == 1) return tuple.PlayerOne;
            if (player == 2) return tuple.PlayerTwo;
            if (player == 3) return tuple.PlayerThree;
            if (player == 4) return tuple.PlayerFour;

            throw new BadPlayerNumberException(player);
            return 0;
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

        protected static vec2 geo_pos_id(geo g)
        {
            return unpack_vec2_3byte(g.pos_storage);
        }

        protected static void set_geo_pos_id(ref geo g, vec2 pos)
        {
            g.pos_storage = pack_vec2_3byte(pos);
        }

        const float select_offset = _128;
        protected static bool selected(data u)
        {
            float val = u.prior_direction_and_select;
            return val >= select_offset;
        }

        protected static void set_selected(ref data u, bool selected)
        {
            u.prior_direction_and_select = prior_direction(u) + (selected ? select_offset : _0);
        }

        protected static float prior_direction(data u)
        {
            float val = u.prior_direction_and_select;
            if (val >= select_offset) val -= select_offset;

            // Subtracting select_offset leads to some inaccuracies in the resulting fint value.
            // We fix this problem by rounding to the nearest fint.
            // An alternative solution may be to raise our epsilon tolerance in equality tests.
            val = fint_round(val);

            return val;
        }

        protected static void set_prior_direction(ref data u, float dir)
        {
            u.prior_direction_and_select = dir + (selected(u) ? select_offset : _0);
        }

        protected static bool selected(building u)
        {
            float val = u.prior_direction_and_select;
            return val >= select_offset;
        }

        protected static void set_selected(ref building u, bool selected)
        {
            u.prior_direction_and_select = prior_direction(u) + (selected ? select_offset : _0);
        }

        protected static float prior_direction(building u)
        {
            float val = u.prior_direction_and_select;
            if (val >= select_offset) val -= select_offset;
            return val;
        }

        protected static void set_prior_direction(ref building u, float dir)
        {
            u.prior_direction_and_select = dir + (selected(u) ? select_offset : _0);
        }

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

        protected vec2 get_subcell_pos(VertexOut vertex, vec2 grid_size)
        {
            vec2 coords = vertex.TexCoords * grid_size;
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
        }

        public static class Team
        {
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

        public static class UnitType
        {
            public const float
                FirstUnitType = Footman,
                FirstBuildingType = Barracks,
                FirstNeutralBuildingType = GoldSource,
                FirstBlockingTileType = BlockingTile;

            public static float BuildingIndex(float type)
            {
                return type - FirstBuildingType;
            }

            public const float
                None = _0,

                Footman = _1,

                Barracks = _2,
                GoldMine = _3,

                GoldSource = _4,

                BlockingTile = _5;
        }

        protected static bool IsUnit(unit u)
        {
            return u.type == UnitType.Footman;
        }

        protected static bool IsBuilding(unit u)
        {
            return u.type >= UnitType.FirstBuildingType && u.type < UnitType.FirstBlockingTileType;
        }

        protected static bool IsNeutralBuilding(unit u)
        {
            return u.type >= UnitType.FirstNeutralBuildingType && u.type < UnitType.FirstBlockingTileType;
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
                None = _0 * UnitSpriteSheet.AnimLength,
                Attack = _1 * UnitSpriteSheet.AnimLength,
                Dead = _2 * UnitSpriteSheet.AnimLength;
        }

        public static class Part
        {
            public const float
                Center = _0,
                Right = _1,
                TR = _2,
                Up = _3,
                TL = _4,
                Left = _5,
                BL = _6,
                Down = _7,
                BR = _8,
                Count = _9;
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
            public const int AnimLength = 5;
            public const int NumAnims = 3;
            public const int SheetDimX = NumAnims * AnimLength;
            public const int SheetDimY = 2 /*Selected,Unselected*/ * 4 /*4 Directions*/;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class BuildingSpriteSheet
        {
            public const int BuildingDimX = 3;
            public const int BuildingDimY = 3;
            public static readonly vec2 BuildingDim = vec(BuildingDimX, BuildingDimY);
            public static readonly vec2 BuildingSize = vec(BuildingDimX / (float)SheetDimX, BuildingDimY / (float)SheetDimY);

            public const int UnitTypes = 3;
            public const int SubsheetDimX = 1;
            public const int SubsheetDimY = 2 /*Selected,Unselected*/ * BuildingDimY;
            public static readonly vec2 SubsheetSize = vec(SubsheetDimX / (float)SheetDimX, SubsheetDimY / (float)SheetDimY);

            public const int AnimLength = 1;
            public const int NumAnims = 1;
            public const int SheetDimX = NumAnims * AnimLength * BuildingDimX;
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

        public static class UnitAction
        {
            public const float
                Stopped = _0,
                Moving = _1,
                Attacking = _2,
                Guard = _3,
                Spawning = _4,
                Count = _5,

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

        protected static bool SomethingSelected(data u)
        {
            return Something(u) && selected(u);
        }

        protected static bool Something(data u)
        {
            return u.direction > 0;
        }

        protected static bool Something(corpse u)
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

        protected color PlayerColorize(color clr, float player)
        {
            if (player == Player.One)
            {
            }
            else if (player == Player.Two)
            {
                float r = clr.r;
                clr.r = clr.g;
                clr.g = r;
                clr.rgb *= .5f;
            }
            else if (player == Player.Three)
            {
                float b = clr.b;
                clr.b = clr.g;
                clr.g = b;
            }
            else if (player == Player.Four)
            {
                float r = clr.r;
                clr.r = clr.b;
                clr.b = r;
            }
            else
            {
                clr.rgb *= .1f;
            }

            return clr;
        }

        public class TerritoryColors
        {
            public static readonly color
                Player1 = new color(.7f, .3f, .3f, .5f),
                Player2 = new color(.1f, .5f, .1f, .5f),
                Player3 = new color(.3f, .7f, .55f, .5f),
                Player4 = new color(.3f, .3f, .7f, .5f);

            public static color Get(float player)
            {
                if (player == Player.One) return Player1;
                if (player == Player.Two) return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four) return Player4;

                throw new BadPlayerNumberException(player);
                return color.TransparentBlack;
            }
        }

        public class BuildingMarkerColors
        {
            public static readonly color
                Neutral = new color(248/255.0f, 230/255.0f, 33/255.0f, 1f),
                Player1 = new color(.6f, .2f, .2f, 1f),
                Player2 = new color(.0f, .4f, .0f, 1f),
                Player3 = new color(.2f, .6f, .45f, 1f),
                Player4 = new color(.2f, .2f, .7f, 1f);

            public static color Get(float player)
            {
                if (player == Player.One) return Player1;
                if (player == Player.Two) return Player2;
                if (player == Player.Three) return Player3;
                if (player == Player.Four) return Player4;

                throw new BadPlayerNumberException(player);
                return Neutral;
            }
        }
    }
}