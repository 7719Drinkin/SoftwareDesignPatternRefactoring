using System;

public class NightBornStates : IEnemyStates
{
    public NightBornMoveState Move { get; }
    public NightBornBattleState Battle { get; }
    public NightBornAttackState Attack { get; }
    public NightBornBlockedState Blocked { get; }
    public NightBornStunnedState Stunned { get; }
    public NightBornDeadState Dead { get; }
    public NightBornGatherState Gather { get; }
    public NightBornWaveState Wave { get; }

    public NightBornStates(
        NightBornMoveState move,
        NightBornBattleState battle,
        NightBornAttackState attack,
        NightBornBlockedState blocked,
        NightBornStunnedState stunned,
        NightBornDeadState dead,
        NightBornGatherState gather,
        NightBornWaveState wave)
    {
        Move = move ?? throw new ArgumentNullException(nameof(move));
        Battle = battle ?? throw new ArgumentNullException(nameof(battle));
        Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        Blocked = blocked ?? throw new ArgumentNullException(nameof(blocked));
        Stunned = stunned ?? throw new ArgumentNullException(nameof(stunned));
        Dead = dead ?? throw new ArgumentNullException(nameof(dead));
        Gather = gather ?? throw new ArgumentNullException(nameof(gather));
        Wave = wave ?? throw new ArgumentNullException(nameof(wave));
    }

    public EnemyState InitialState => Move;
}
