using FragSharpFramework;

namespace GpuSim
{
    public class UnitField : PointSampler
    {
        new public unit this[RelativeIndex index]
        {
            get
            {
                return unit.Nothing;
            }
        }
    }

    [Copy(typeof(vec4))]
    public partial struct unit
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

    public class DataField : PointSampler
    {
        new public data this[RelativeIndex index]
        {
            get
            {
                return data.Nothing;
            }
        }
    }

    [Copy(typeof(vec4))]
    public partial struct data
    {
        [Hlsl("r")]
        public float type { get { return r; } set { r = value; } }

        [Hlsl("g")]
        public float player { get { return g; } set { g = value; } }

        [Hlsl("b")]
        public float team { get { return b; } set { b = value; } }

        [Hlsl("a")]
        public float target_angle { get { return a; } set { a = value; } }
    }

    public class SimShader : GridComputation
    {
        protected readonly vec2 SpriteSize = vec(1.0f / 10.0f, 1.0f / 8.0f);

        protected static bool selected(unit u)
        {
            float val = u.prior_direction_and_select;
            return val >= Dir.Count;
        }

        protected static void set_selected(ref unit u, bool selected)
        {
            u.prior_direction_and_select = prior_direction(u) + (selected ? Dir.Count : _0);
        }

        protected static float prior_direction(unit u)
        {
            float val = u.prior_direction_and_select;
            return val % Dir.Count;
        }

        protected static void set_prior_direction(ref unit u, float dir)
        {
            u.prior_direction_and_select = dir + (selected(u) ? Dir.Count : _0);
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

        public static class Dir
        {
            public const float
                None = _0,
                Right = _1,
                Up = _2,
                Left = _3,
                Down = _4,
                Count = _5,

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
                Count = _3,

                NoChange = _12;
        }

        protected static bool SomethingSelected(unit u)
        {
            return Something(u) && selected(u);
        }

        protected static bool Something(unit u)
        {
            return u.direction > 0;
        }

        protected static bool IsValid(float direction)
        {
            return direction > 0;
        }

        protected static void TurnLeft(ref unit u)
        {
            u.direction += Dir.TurnLeft;
            if (u.direction > Dir.Down)
                u.direction = Dir.Right;
        }

        protected static void TurnRight(ref unit u)
        {
            u.direction += Dir.TurnRight;
            if (u.direction < Dir.Right)
                u.direction = Dir.Down;
        }

        protected static void TurnAround(ref unit u)
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
    }
}