using FragSharpFramework;

namespace Game
{
    public partial class SetTeams : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units, Field<data> Data, PlayerTuple Teams)
        {
            unit unit_here = Units[Here];
            data data_here = Data[Here];

            if (!Something(data_here) && unit_here.player == Player.None) return unit_here;

            unit_here.team = GetPlayerVal(Teams, unit_here.player);

            return unit_here;
        }
    }

    public partial class World : SimShader
    {
        public void Startup()
        {
            Render.UnsetDevice();

            // Set datagroup team data.
            SetTeams.Apply(DataGroup.CurrentUnits, DataGroup.CurrentData, PlayerTeamVals, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentUnits);

            SetTeams.Apply(DataGroup.PreviousUnits, DataGroup.PreviousData, PlayerTeamVals, Output: DataGroup.Temp1);
            CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousUnits);

            DataGroup.DistanceToOtherTeams.Clear();
            for (int i = 0; i < 24; i++)
            {
                DataGroup.UpdateGradient_ToOtherTeams();
            }
        
            // Focus camera on a dragon lord.
            vec2 pos = DataGroup.DragonLordPos(MyPlayerValue);
            
            //{0.01248588,0.004402504}
            CameraPos = GridToWorldCood(pos + vec(.375f, 1.5f));
            CameraZoom = 80f;

            Render.UnsetDevice();
        }
    }
}
