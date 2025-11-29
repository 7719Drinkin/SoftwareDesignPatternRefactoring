using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDeadState : EnemyState
{
    private Enemy_Slime enemy;

    public SlimeDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.FreezeTime(false);

        enemy.isDead = true;
        enemy.CloseCounterAttackWindow();
        enemy.GetComponentInChildren<UI_HealthBar>().DestroyMe();
        enemy.PlayerGetCurrency();
    }

    public override void Update()
    {
        base.Update();

        enemy.rb.velocity = new UnityEngine.Vector2(0, -10);

        if (triggerCalled)
            enemy.SelfDestroy();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
