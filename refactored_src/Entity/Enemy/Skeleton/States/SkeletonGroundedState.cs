using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Enemy_Skeleton enemy;

    protected Transform player;

    public SkeletonGroundedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        if (playerManager != null && playerManager.Player != null)
            player = playerManager.Player.transform;
        else
            player = null;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player == null)
            return;

        CharacterStats playerStats = player.GetComponent<CharacterStats>();
        if (playerStats == null)
            return;

        if (((enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < 4) && enemy.IsGroundDetected()) && !playerStats.isDead)
        {
            enemy.StartCoroutine("DiscoverPlayer");

            audioManager.PlaySFX(10);

            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
