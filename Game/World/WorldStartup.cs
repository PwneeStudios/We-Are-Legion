using FragSharpFramework;

namespace Game
{
    public partial class SetTeams : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units, PlayerTuple Teams)
        {
            unit unit_here = Units[Here];

            if (unit_here.player == Player.None) return unit_here;

            unit_here.team = GetPlayerVal(Teams, unit_here.player);

            return unit_here;
        }
    }

    public partial class World : SimShader
    {
        public void Startup()
        {
            Render.UnsetDevice();

            SetTeams.Apply(DataGroup.CurrentUnits, PlayerTeamVals, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            SetTeams.Apply(DataGroup.PreviousUnits, PlayerTeamVals, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);

            Render.UnsetDevice();
        }
    }
}
