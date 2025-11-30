using System;

public class SlimeStateFactory : IEnemyStateFactory<SlimeStates>
{
    public SlimeStates CreateStates(Enemy enemy, EnemyStateMachine stateMachine)
    {
        if (!(enemy is Enemy_Slime slime))
            throw new ArgumentException("SlimeStateFactory requires an Enemy_Slime instance.", nameof(enemy));

        return new SlimeStates(
            new SlimeMoveState(slime, stateMachine, "Move", slime),
            new SlimeAttackState(slime, stateMachine, "Attack", slime),
            new SlimeDeadState(slime, stateMachine, "Die", slime),
            new SlimeBattleState(slime, stateMachine, "Move", slime)
        );
    }
}

