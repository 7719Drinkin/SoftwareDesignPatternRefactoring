using System;

public class SkeletonStateFactory : IEnemyStateFactory<SkeletonStates>
{
    public SkeletonStates CreateStates(Enemy enemy, EnemyStateMachine stateMachine)
    {
        if (!(enemy is Enemy_Skeleton skeleton))
            throw new ArgumentException("SkeletonStateFactory requires an Enemy_Skeleton instance.", nameof(enemy));

        return new SkeletonStates(
            new SkeletonIdleState(skeleton, stateMachine, "Idle", skeleton),
            new SkeletonMoveState(skeleton, stateMachine, "Move", skeleton),
            new SkeletonBattleState(skeleton, stateMachine, "Move", skeleton),
            new SkeletonAttackState(skeleton, stateMachine, "Attack", skeleton),
            new SkeletonStunnedState(skeleton, stateMachine, "Stunned", skeleton),
            new SkeletonDeadState(skeleton, stateMachine, "Die", skeleton),
            new SkeletonBlockedState(skeleton, stateMachine, "Blocked", skeleton)
        );
    }
}

