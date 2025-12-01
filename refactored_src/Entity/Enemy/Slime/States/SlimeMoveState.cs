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

        if (playerManager != null && playerManager.Player != null)
            player = playerManager.Player.transform;
        else
            player = null;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir, rb.velocity.y);

        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.Flip();
        }

        if (player == null)
            return;

        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if (playerStats == null)
            return;

        if (((enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < 4) && enemy.IsGroundDetected()) && !playerStats.isDead)
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
