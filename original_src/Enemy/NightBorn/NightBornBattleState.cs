using UnityEngine;

public class NightBornBattleState : EnemyState
{
    private Enemy_NightBorn enemy;
    private Transform player;
    private int moveDir;

    private float assassinateTimer;

    public NightBornBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        assassinateTimer = enemy.assassinateCooldown;
    }

    public override void Update()
    {
        base.Update();

        if (player.GetComponent<CharacterStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

        if (assassinateTimer < 0)
        {
            if (enemy.TryAssassinateBehindPlayer(0.5f))
                assassinateTimer = enemy.assassinateCooldown;
        }

        assassinateTimer -= Time.deltaTime;
       
        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance)
            if (CanAttack() && !enemy.isKnocked)
                stateMachine.ChangeState(enemy.attackState);

        // 先朝向玩家
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
            enemy.Flip();

        // 若前方是墙或悬崖，则停止向前推进，防止掉下去
        if (enemy.IsWallDetected() || !enemy.IsGroundDetected())
        {
            enemy.SetVelocity(0, rb.velocity.y);
            return;
        }

        // 路径安全再向玩家方向推进
        enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir * 1.2f, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time > enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCoolDown, enemy.maxAttackCoolDown);

            return true;
        }

        return false;
    }
}
