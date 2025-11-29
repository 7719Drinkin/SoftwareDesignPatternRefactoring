using UnityEngine;

public class SkeletonAttackState : EnemyState
{
    private Enemy_Skeleton enemy;

    private float attackSpeed;

    public SkeletonAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        attackSpeed = Random.Range(enemy.minAttackSpeed, enemy.maxAttackSpeed);

        enemy.anim.speed *= (1 + attackSpeed);

        AudioManager.instance.PlaySFX(11);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.anim.speed = 1;

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsGroundDetected())
            enemy.ZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
