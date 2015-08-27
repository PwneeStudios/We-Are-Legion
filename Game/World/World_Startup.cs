using FragSharpFramework;

namespace Game
{
    public partial class RemoveDragonLordData : SimShader
    {
        [FragmentShader]
        data FragmentShader(VertexOut vertex, Field<unit> Units, Field<data> Data, [Player.Vals] float player)
        {
            unit unit_here = Units[Here];
            data data_here = Data[Here];

            if (Something(data_here) && unit_here.player == player && unit_here.type == UnitType.DragonLord) return data.Nothing;

            return data_here;
        }
    }

    public partial class RemoveDragonLordUnit : SimShader
    {
        [FragmentShader]
        unit FragmentShader(VertexOut vertex, Field<unit> Units, Field<data> Data, [Player.Vals] float player)
        {
            unit unit_here = Units[Here];
            data data_here = Data[Here];

            if (Something(data_here) && unit_here.player == player && unit_here.type == UnitType.DragonLord) return unit.Nothing;

            return unit_here;
        }
    }

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

            int user_count = 0;
            for (int p = 1; p <= 4; p++)
            {
                if (Program.SteamUsers[p - 1] != 0) user_count++;
            }

            if (RemoveComputerDragonLords && user_count > 1)
            {
                for (int player = 1; player <= 4; player++)
                {
                    if (Program.PlayersSteamUser[player] == 0)
                    {
                        RemoveDragonLordData.Apply(DataGroup.CurrentUnits, DataGroup.CurrentData, Player.Vals[player], Output: DataGroup.Temp1);
                        RemoveDragonLordUnit.Apply(DataGroup.CurrentUnits, DataGroup.CurrentData, Player.Vals[player], Output: DataGroup.Temp2);
                        CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.CurrentData);
                        CoreMath.Swap(ref DataGroup.Temp2, ref DataGroup.CurrentUnits);

                        RemoveDragonLordData.Apply(DataGroup.PreviousUnits, DataGroup.PreviousData, Player.Vals[player], Output: DataGroup.Temp1);
                        RemoveDragonLordUnit.Apply(DataGroup.PreviousUnits, DataGroup.PreviousData, Player.Vals[player], Output: DataGroup.Temp2);
                        CoreMath.Swap(ref DataGroup.Temp1, ref DataGroup.PreviousData);
                        CoreMath.Swap(ref DataGroup.Temp2, ref DataGroup.PreviousUnits);
                    }
                }
            }

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

            if (MyPlayerNumber > 0)
            {
                // Focus camera on a dragon lord.
                vec2 pos = DataGroup.DragonLordPos(MyPlayerValue);

                CameraPos = GridToWorldCood(pos);
                CameraZoom = 80f;
            }
            else
            {
                // We're a spectator, so choose netural position.
                CameraPos = vec2.Zero;
                CameraZoom = 1.45f;
            }

            Render.UnsetDevice();
        }
    }
}
