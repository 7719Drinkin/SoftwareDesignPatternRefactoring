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

        player = ServiceLocator.Instance.Get<IPlayerManager>().Player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (((enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < 4) && enemy.IsGroundDetected()) && !player.GetComponent<CharacterStats>().isDead)
        {
            enemy.StartCoroutine("DiscoverPlayer");

            audioManager.PlaySFX(10);

            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
