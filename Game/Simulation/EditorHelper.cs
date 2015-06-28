using System;
using FragSharpFramework;

namespace Game
{
    public class SymmetryType : BaseShader
    {
        [FragSharpFramework.Vals(Quad, Octo)]
        public class ValsAttribute : Attribute { }

        public static readonly float[] Vals = new float[] { Quad, Octo };

        public const float
            Quad = 0,
            Octo = 1;
    }

    public partial class MakeSymmetricBase : SimShader
    {
        protected bool DoNothing(Sampler Units, vec2 pos, float type)
        {
            // Quads, with mirroring
            if (type == SymmetryType.Quad)
            {
                if (pos < Units.Size / 2) return true;
                else return false;
            }

            // Octos, with mirroring
            if (type == SymmetryType.Octo)
            {
                if (pos < Units.Size / 4) return true;
                else return false;
            }

            return true;
        }

        protected RelativeIndex QuadMirrorShift(Sampler Info, vec2 pos, float type)
        {
            vec2 shift = vec2.Zero;

            // Quads, with mirroring
            if (type == SymmetryType.Quad)
            {
                if (pos.x > Info.Size.x / 2) shift.x = 2 * pos.x - Info.Size.x;
                if (pos.y > Info.Size.y / 2) shift.y = 2 * pos.y - Info.Size.y;
            }

            // Octos, with mirroring
            if (type == SymmetryType.Octo)
            {
                if (pos.x > Info.Size.x / 4) shift.x = 2 * pos.x - Info.Size.x / 2;
                if (pos.y > Info.Size.y / 4) shift.y = 2 * pos.y - Info.Size.y / 2;
            }

            return new RelativeIndex(shift.x, shift.y);
        }

        protected vec2 QuadMirrorTarget(Sampler Info, vec2 pos, vec2 target, float type)
        {
            // Quads, with mirroring
            if (type == SymmetryType.Quad)
            {
                if (pos.x > Info.Size.x / 2) target.x = Info.Size.x - target.x;
                if (pos.y > Info.Size.y / 2) target.y = Info.Size.y - target.y;
            }

            // Octos, with mirroring
            if (type == SymmetryType.Octo)
            {
                if (pos.x > Info.Size.x / 4) target.x = Info.Size.x / 2 - target.x;
                if (pos.y > Info.Size.y / 4) target.y = Info.Size.y / 2 - target.y;
            }

            return target;
        }
    }

    public partial class MakeSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Info, [SymmetryType.Vals] float type)
        {
            vec4 info = Info[Here];
            vec2 pos = vertex.TexCoords * Info.Size;

            if (DoNothing(Info, pos, type)) return info;

            vec4 copy = Info[Here - QuadMirrorShift(Info, pos, type)];

            return copy;
        }
    }

    public partial class MakeUnitsSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units, [SymmetryType.Vals] float type, bool convert_dragonlords)
        {
            unit info = Units[Here];
            vec2 pos = vertex.TexCoords * Units.Size;

            if (DoNothing(Units, pos, type)) return info;

            unit copy = Units[Here - QuadMirrorShift(Units, pos, type)];

            if (copy.player == Player.None) return copy;

            if (pos.x > Units.Size.x / 2) { copy.player += _1; copy.team += _1; }
            if (pos.y > Units.Size.y / 2) { copy.player += _2; copy.team += _2; }

            if (copy.player > Player.Four) copy.player -= Player.Four;
            if (copy.team > Team.Four) copy.team -= Team.Four;

            if (convert_dragonlords && copy.type == UnitType.DragonLord)
            {
                copy.type = UnitType.Footman;
            }

            return copy;
        }
    }

    public partial class MakeTargetSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Target, [SymmetryType.Vals] float type)
        {
            vec4 info = Target[Here];
            vec2 pos = vertex.TexCoords * Target.Size;

            if (DoNothing(Target, pos, type)) return info;

            vec4 copy = Target[Here - QuadMirrorShift(Target, pos, type)];

            vec2 target = unpack_vec2(copy);
            target = QuadMirrorTarget(Target, pos, target, type);
            copy = pack_vec2(target);

            return copy;
        }
    }

    public partial class FixBuildings_1 : MakeSymmetricBase
    {
        [FragmentShader]
        building FragmentShader(VertexOut vertex, Field<building> Data, Field<unit> Units)
        {
            building here = Data[Here];

            if (IsBuilding(Units[RightOne]) && IsCenter(Data[RightOne])) { here.part_x = _0; here.part_y = _1; }
            if (IsBuilding(Units[LeftOne])  && IsCenter(Data[LeftOne]))  { here.part_x = _2; here.part_y = _1; }
            if (IsBuilding(Units[UpOne])    && IsCenter(Data[UpOne]))    { here.part_x = _1; here.part_y = _0; }
            if (IsBuilding(Units[DownOne])  && IsCenter(Data[DownOne]))  { here.part_x = _1; here.part_y = _2; }

            if (IsBuilding(Units[UpRight])   && IsCenter(Data[UpRight]))   { here.part_x = _0; here.part_y = _0; }
            if (IsBuilding(Units[DownRight]) && IsCenter(Data[DownRight])) { here.part_x = _0; here.part_y = _2; }
            if (IsBuilding(Units[UpLeft])    && IsCenter(Data[UpLeft]))    { here.part_x = _2; here.part_y = _0; }
            if (IsBuilding(Units[DownLeft])  && IsCenter(Data[DownLeft]))  { here.part_x = _2; here.part_y = _2; }

            return here;
        }
    }

    public partial class FixBuildings_2 : MakeSymmetricBase
    {
        [FragmentShader]
        building FragmentShader(VertexOut vertex, Field<building> Data, Field<unit> Units)
        {
            building here = Data[Here];
            unit unit_here = Units[Here];

            if (IsBuilding(unit_here) && !IsCenter(here) &&
                !IsCenter(Data[RightOne]) && !IsCenter(Data[LeftOne]) && !IsCenter(Data[UpOne]) && !IsCenter(Data[DownOne]) &&
                !IsCenter(Data[UpRight]) && !IsCenter(Data[UpLeft]) && !IsCenter(Data[DownRight]) && !IsCenter(Data[DownLeft]))
            {
                return building.Nothing;
            }
            else
            {
                return here;
            }
        }
    }
}
