using FragSharpFramework;

namespace GpuSim
{
    public class UnitField : Sampler
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
        public float change { get { return r; } set { r = value; } }

        public static readonly unit Nothing = new unit(0, 0, 0, 0);
    }

    public class SimShader : GridComputation
    {
        protected static class Dir
        {
            public const float
                None = _0,
                Right = _1,
                Up = _2,
                Left = _3,
                Down = _4,

                TurnRight = -_1,
                TurnLeft = _1;
        }

        protected static class Change
        {
            public const float
                Moved = _0,
                Stayed = _1;
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

        protected static RelativeIndex dir_to_vec(float direction)
        {
            float angle = (float)((direction * 255 - 1) * (3.1415926 / 2.0));
            return IsValid(direction) ? new RelativeIndex(cos(angle), sin(angle)) : new RelativeIndex(0, 0);
        }
    }
}