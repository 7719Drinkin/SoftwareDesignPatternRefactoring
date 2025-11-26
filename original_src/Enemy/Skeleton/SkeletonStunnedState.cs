using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    Enemy_Skeleton enemy;

    public SkeletonStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.StopTemporaryFreeze();
        enemy.CloseCounterAttackWindow();
        enemy.isStunned = true;

        enemy.fx.InvokeRepeating("RedColorBlink", 0, 0.1f);

        stateTimer = enemy.stunnedDuration;

        enemy.rb.velocity = new Vector2(-enemy.facingDir * enemy.stunnedDistance.x, enemy.stunnedDistance.y);

        AudioManager.instance.PlaySFX(19);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.FreezeTime(false);

        enemy.isStunned = false;

        enemy.fx.CancelStunBlink();

        if (enemy.enemyStats != null)
            enemy.enemyStats.currentEndurance = enemy.enemyStats.maxEndurance.GetValue();
    }

    public override void Update()
    {
        base.Update();

        enemy.isStunned = true;

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}
