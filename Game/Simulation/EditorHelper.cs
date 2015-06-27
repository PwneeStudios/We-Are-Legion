using FragSharpFramework;

namespace Game
{
    public partial class MakeSymmetricBase : SimShader
    {
        protected RelativeIndex QuadMirrorShift(Sampler Info, vec2 pos)
        {
            vec2 shift = vec2.Zero;

            // Quads, no mirroring
            //if (pos.x > Info.Size.x / 2) shift.x = Info.Size.x / 2;
            //if (pos.y > Info.Size.y / 2) shift.y = Info.Size.y / 2;

            // Quads, with mirroring
            if (pos.x > Info.Size.x / 2) shift.x = 2 * pos.x - Info.Size.x;
            if (pos.y > Info.Size.y / 2) shift.y = 2 * pos.y - Info.Size.y;

            return new RelativeIndex(shift.x, shift.y);
        }

        protected vec2 QuadMirrorTarget(Sampler Info, vec2 pos, vec2 target)
        {
            // Quads, with mirroring
            if (pos.x > Info.Size.x / 2) target.x = Info.Size.x - target.x;
            if (pos.y > Info.Size.y / 2) target.y = Info.Size.y - target.y;

            return target;
        }
    }

    public partial class MakeSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Info)
        {
            vec4 info = Info[Here];
            vec2 pos = vertex.TexCoords * Info.Size;

            if (pos < Info.Size / 2) return info;

            vec4 copy = Info[Here - QuadMirrorShift(Info, pos)];

            return copy;
        }
    }

    public partial class MakeUnitsSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units)
        {
            unit info = Units[Here];
            vec2 pos = vertex.TexCoords * Units.Size;

            if (pos < Units.Size / 2) return info;

            unit copy = Units[Here - QuadMirrorShift(Units, pos)];

            if (copy.player == Player.None) return copy;

            if (pos.x > Units.Size.x / 2) { copy.player += _1; copy.team += _1; }
            if (pos.y > Units.Size.y / 2) { copy.player += _2; copy.team += _2; }

            if (copy.player > Player.Four) copy.player -= Player.Four;
            if (copy.team > Team.Four) copy.team -= Team.Four;

            return copy;
        }
    }

    public partial class MakeTargetSymmetric : MakeSymmetricBase
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<vec4> Target)
        {
            vec4 info = Target[Here];
            vec2 pos = vertex.TexCoords * Target.Size;

            if (pos < Target.Size / 2) return info;

            vec4 copy = Target[Here - QuadMirrorShift(Target, pos)];

            vec2 target = unpack_vec2(copy);
            target = QuadMirrorTarget(Target, pos, target);
            copy = pack_vec2(target);

            return copy;
        }
    }

    public partial class FixBuildings : MakeSymmetricBase
    {
        [FragmentShader]
        building FragmentShader(VertexOut vertex, Field<building> Data, Field<unit> Units)
        {
            building here = Data[Here];

            if (IsBuilding(Units[RightOne]) && IsCenter(Data[RightOne])) { here.part_x = _0; here.part_y = _1; }
            if (IsBuilding(Units[LeftOne])  && IsCenter(Data[LeftOne]))  { here.part_x = _2; here.part_y = _1; }
            if (IsBuilding(Units[UpOne])    && IsCenter(Data[UpOne]))    { here.part_x = _1; here.part_y = _0; }
            if (IsBuilding(Units[DownOne])  && IsCenter(Data[DownOne]))  { here.part_x = _1; here.part_y = _2; }

            if (IsBuilding(Units[UpRight])   && IsCenter(Data[UpRight]))  { here.part_x = _0; here.part_y = _0; }
            if (IsBuilding(Units[DownRight]) && IsCenter(Data[DownRight])) { here.part_x = _0; here.part_y = _2; }
            if (IsBuilding(Units[UpLeft])    && IsCenter(Data[UpLeft]))  { here.part_x = _2; here.part_y = _0; }
            if (IsBuilding(Units[DownLeft])  && IsCenter(Data[DownLeft]))  { here.part_x = _2; here.part_y = _2; }

            return here;
        }
    }
}
