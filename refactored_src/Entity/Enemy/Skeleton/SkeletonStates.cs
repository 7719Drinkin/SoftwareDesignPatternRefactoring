using System;

public class SkeletonStates : IEnemyStates
{
    public SkeletonIdleState Idle { get; }
    public SkeletonMoveState Move { get; }
    public SkeletonBattleState Battle { get; }
    public SkeletonAttackState Attack { get; }
    public SkeletonStunnedState Stunned { get; }
    public SkeletonDeadState Dead { get; }
    public SkeletonBlockedState Blocked { get; }

    public SkeletonStates(
        SkeletonIdleState idle,
        SkeletonMoveState move,
        SkeletonBattleState battle,
        SkeletonAttackState attack,
        SkeletonStunnedState stunned,
        SkeletonDeadState dead,
        SkeletonBlockedState blocked)
    {
        Idle = idle ?? throw new ArgumentNullException(nameof(idle));
        Move = move ?? throw new ArgumentNullException(nameof(move));
        Battle = battle ?? throw new ArgumentNullException(nameof(battle));
        Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        Stunned = stunned ?? throw new ArgumentNullException(nameof(stunned));
        Dead = dead ?? throw new ArgumentNullException(nameof(dead));
        Blocked = blocked ?? throw new ArgumentNullException(nameof(blocked));
    }

    public EnemyState InitialState => Idle;
}

