using FragSharpFramework;

namespace Terracotta
{
    public partial class UpdateMagic : SimShader
    {
        [FragmentShader]
        magic FragmentShader(VertexOut vertex, Field<magic> Magic, Field<data> CurrentData, Field<data> PreviousData, Field<corpse> Corpses, Field<PlayerTuple> Necromancy)
        {
            magic here  = Magic[Here];
            corpse corpse_here = Corpses[Here];
            PlayerTuple necromancy = Necromancy[Here];

            data
                cur_data = CurrentData[Here],
                prev_data = PreviousData[Here];

            // Reset the kill bit
            here.kill = _false;
            here.raising_player = Player.None;

            // Check for resurrection
            if (CorpsePresent(corpse_here) && !Something(cur_data) && !Something(prev_data))
            {
                float player = Player.None;
                float necro = _0;
                if (necromancy.PlayerOne   > necro) { necro = necromancy.PlayerOne;   player = Player.One; }
                if (necromancy.PlayerTwo   > necro) { necro = necromancy.PlayerTwo;   player = Player.Two; }
                if (necromancy.PlayerThree > necro) { necro = necromancy.PlayerThree; player = Player.Three; }
                if (necromancy.PlayerFour  > necro) { necro = necromancy.PlayerFour;  player = Player.Four; }

                here.raising_player = player;
            }

            return here;
        }
    }

    public partial class Kill : SimShader
    {
        [FragmentShader]
        magic FragmentShader(VertexOut vertex, Field<data> Select, Field<magic> Magic)
        {
            magic here  = Magic[Here];
            data select = Select[Here];

            if (Something(select))
            {
                here.kill = _true;
            }

            return here;
        }
    }

    /// <summary>
    /// Propagates necromancy aura per player.
    /// </summary>
    public partial class PropagateNecromancyAuro : SimShader
    {
        [FragmentShader]
        PlayerTuple FragmentShader(VertexOut vertex, Field<PlayerTuple> Necromancy, Field<data> Data, Field<unit> Units)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            PlayerTuple
                right = Necromancy[RightOne],
                up    = Necromancy[UpOne],
                left  = Necromancy[LeftOne],
                down  = Necromancy[DownOne];

            PlayerTuple necromancy = max(right, up, left, down) - vec(_1, _1, _1, _1);

            if (unit_here.type == UnitType.Necromancer)
            {
                if (unit_here.player == Player.One)   necromancy.PlayerOne   = _255;
                if (unit_here.player == Player.Two)   necromancy.PlayerTwo   = _255;
                if (unit_here.player == Player.Three) necromancy.PlayerThree = _255;
                if (unit_here.player == Player.Four)  necromancy.PlayerFour  = _255;
            }

            return necromancy;
        }
    }
}
