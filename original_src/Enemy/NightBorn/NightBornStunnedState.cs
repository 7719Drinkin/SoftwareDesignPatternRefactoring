using UnityEngine;

public class NightBornStunnedState : EnemyState
{
    private Enemy_NightBorn enemy;

    public NightBornStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.StopTemporaryFreeze();
        enemy.CloseCounterAttackWindow();
        enemy.isStunned = true;

        stateTimer = enemy.stunnedDuration;

        enemy.rb.velocity = new Vector2(-enemy.facingDir * enemy.stunnedDistance.x, enemy.stunnedDistance.y);

        AudioManager.instance.PlaySFX(53);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.FreezeTime(false);

        enemy.isStunned = false;

        if (enemy.enemyStats != null)
            enemy.enemyStats.currentEndurance = enemy.enemyStats.maxEndurance.GetValue();
    }

    public override void Update()
    {
        base.Update();

        enemy.isStunned = true;

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.battleState);
    }
}
