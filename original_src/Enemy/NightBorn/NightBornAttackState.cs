using UnityEngine;

public class NightBornAttackState : EnemyState
{
    private Enemy_NightBorn enemy;

    private float attackSpeed;

    public NightBornAttackState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        attackSpeed = Random.Range(enemy.minAttackSpeed, enemy.maxAttackSpeed);

        enemy.anim.speed *= (1 + attackSpeed);

        AudioManager.instance.PlaySFX(47);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsGroundDetected())   
            enemy.ZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.anim.speed = 1;

        enemy.lastTimeAttacked = Time.time;
    }
}