using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FragSharpHelper;
using FragSharpFramework;

namespace Terracotta
{
    public partial class DataGroup : SimShader
    {
        void UpdateGradient_ToOtherTeams()
        {
            Pathfinding_ToOtherTeams.Apply(DistanceToOtherTeams, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToOtherTeams, ref Temp1);
        }

        void UpdateGradient_ToPlayers()
        {
            Pathfinding_ToPlayers.Apply(DistanceToPlayers, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToPlayers, ref Temp1);
        }

        public void UpdateGradient_ToBuildings()
        {
            Pathfinding_ToBuildings.Apply(DistanceToBuildings, CurrentData, CurrentUnits, Output: Temp1);
            Swap(ref DistanceToBuildings, ref Temp1);
        }
    }
}
