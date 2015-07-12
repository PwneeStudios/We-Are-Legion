using FragSharpFramework;

namespace Game
{
    public partial class World
    {
        int CountSinceDragonLordEngaged = 9999;
        void UpdateDragonLordEngaged()
        {
            DragonLordEngaged.Apply(DataGroup.CurrentUnits, DataGroup.CurrentData, DataGroup.DistanceToOtherTeams, MyPlayerValue, Output: DataGroup.Multigrid[0]);
            var _engaged = (vec4)DataGroup.MultigridReduce(CountReduce_4x1byte.Apply);
            bool engaged = _engaged.r > .5f;

            if (engaged && SimStep > 20)
            {
                if (CountSinceDragonLordEngaged > 20)
                {
                    Message_DragonLordUnderAttack();
                }

                CountSinceDragonLordEngaged = 0;
            }
            else
            {
                CountSinceDragonLordEngaged++;
            }
        }
    }

    public partial class DragonLordEngaged : SimShader
    {
        [FragmentShader]
        vec4 FragmentShader(VertexOut vertex, Field<unit> Units, Field<data> Data, Field<vec4> PathToOtherTeams, [Player.Vals] float player)
        {
            unit unit_here = Units[Here];
            data data_here = Data[Here];
            vec4 path = PathToOtherTeams[Here];

            float value = 1;
            if (unit_here.team == Team.One)
            {
                value = path.x;
            }
            else if (unit_here.team == Team.Two)
            {
                value = path.y;
            }
            else if (unit_here.team == Team.Three)
            {
                value = path.z;
            }
            else if (unit_here.team == Team.Four)
            {
                value = path.w;
            }

            if (Something(data_here) && unit_here.player == player &&
                unit_here.type == UnitType.DragonLord && value < _44) return vec(1,1,1,1);

            return vec4.Zero;
        }
    }
}
