using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMoveState : EnemyState    
{
    private Enemy_Slime enemy;

    private Transform player;

    public SlimeMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.isKnocked = false;

        player = ServiceLocator.Instance.Get<IPlayerManager>().Player.transform;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
        }

        if (((enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < 4) && enemy.IsGroundDetected()) && !player.GetComponent<CharacterStats>().isDead)
        {
            enemy.StartCoroutine("DiscoverPlayer");

            stateMachine.ChangeState(enemy.battleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
