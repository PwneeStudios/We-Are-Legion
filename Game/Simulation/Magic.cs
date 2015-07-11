using FragSharpFramework;

namespace Game
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
        magic FragmentShader(VertexOut vertex, Field<data> Select, Field<magic> Magic, Field<TeamTuple> AntiMagic)
        {
            magic here  = Magic[Here];
            data select = Select[Here];
            
            if (Something(select))
            {
                TeamTuple antimagic = AntiMagic[Here];
                bool block_kill = antimagic.TeamOne > _0 || antimagic.TeamTwo > _0 || antimagic.TeamThree > _0 || antimagic.TeamFour > _0;

                if (!block_kill)
                {
                    here.kill = _true;
                }
            }

            return here;
        }
    }

    /// <summary>
    /// Propagates necromancy aura per player.
    /// </summary>
    public partial class PropagateNecromancyAuro : SimShader
    {
        const float NecromancyRange = _20;

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

            if (unit_here.type == UnitType.Necromancer) SetPlayerVal(ref necromancy, unit_here.player, NecromancyRange);

            return necromancy;
        }
    }

    /// <summary>
    /// Propagates anti-magic aura per player.
    /// </summary>
    public partial class PropagateAntiMagicAuro : SimShader
    {
        const float AntiMagicRange = _42;

        [FragmentShader]
        TeamTuple FragmentShader(VertexOut vertex, Field<TeamTuple> AntiMagic, Field<data> Data, Field<unit> Units)
        {
            data data_here = Data[Here];
            unit unit_here = Units[Here];

            TeamTuple
                right = AntiMagic[RightOne],
                up    = AntiMagic[UpOne],
                left  = AntiMagic[LeftOne],
                down  = AntiMagic[DownOne];

            TeamTuple antimagic = max(right, up, left, down) - vec(_1, _1, _1, _1);

            if (unit_here.type == UnitType.DragonLord) SetTeamVal(ref antimagic, unit_here.team, AntiMagicRange);

            return antimagic;
        }
    }
}
