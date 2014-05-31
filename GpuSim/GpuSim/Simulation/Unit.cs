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
        [Hlsl("a")]
        public float target_angle { get { return a; } set { a = value; } }
    }

    public class SimShader : GridComputation
    {
        const float select_offset = _8;
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

        public static class Team
        {
            public const float
                None = _0,
                One = _1,
                Two = _2,
                Three = _3,
                Four = _4;
        }

        public static class Player
        {
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
                None = _0,
                Footman = _1,
                Barracks = _2;
        }

        protected static bool IsUnit(unit u)
        {
            return u.type == UnitType.Footman;
        }

        protected static bool IsBuilding(unit u)
        {
            return u.type == UnitType.Barracks;
        }

        protected static bool IsCenter(building b)
        {
            return b.part_x == _1 && b.part_y == _1;
        }

        protected static bool IsStationary(data u)
        {
            return u.direction == Dir.Stationary;
        }

        protected static bool IsMobile(data u)
        {
            return u.direction != Dir.Stationary;
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

            public const int AnimLength = 1;
            public const int NumAnims = 1;
            public const int SheetDimX = NumAnims * AnimLength * BuildingDimX;
            public const int SheetDimY = 2 /*Selected,Unselected*/ * BuildingDimY;
            public static readonly vec2 SheetDim = vec(SheetDimX, SheetDimY);
            public static readonly vec2 SpriteSize = vec(1f / SheetDimX, 1f / SheetDimY);
        }

        public static class Dir
        {
            public const float
                None = _0,
                Right = _1,
                Up = _2,
                Left = _3,
                Down = _4,
                Stationary = _5,
                Count = _6,

                TurnRight = -_1,
                TurnLeft = _1;
        }

        protected static class Change
        {
            public const float
                Moved = _0,
                Stayed = _1;
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

        public static vec2 pack_coord(float x)
        {
            vec2 packed = vec2.Zero;

            packed.x = floor(x / 255.0f);
            packed.y = x - packed.x * 255.0f;

            return packed / 255.0f;
        }

        public static float unpack_coord(vec2 packed)
        {
            float coord = 0;

            coord = (255 * packed.x + packed.y) * 255;

            return coord;
        }

        public static vec4 pack_vec2(vec2 v)
        {
            vec2 packed_x = pack_coord(v.x);
            vec2 packed_y = pack_coord(v.y);
            return vec(packed_x.x, packed_x.y, packed_y.x, packed_y.y);
        }

        public static vec2 unpack_vec2(vec4 packed)
        {
            vec2 v = vec2.Zero;
            v.x = unpack_coord(packed.rg);
            v.y = unpack_coord(packed.ba);
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
    }
}