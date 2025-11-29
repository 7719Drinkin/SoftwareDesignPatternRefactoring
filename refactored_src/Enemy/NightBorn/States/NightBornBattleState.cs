using UnityEngine;

public class NightBornBattleState : EnemyState
{
    private Enemy_NightBorn enemy;
    private Transform player;
    private int moveDir;

    private float assassinateTimer;
    private float emergencyTeleportCooldown;  // 紧急传送防重复触发计时器
    private const float EMERGENCY_COOLDOWN = 0.5f;  // 紧急传送最小间隔

    public NightBornBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        player = playerManager.Player.transform;

        assassinateTimer = enemy.assassinateCooldown;
        emergencyTeleportCooldown = 0;
    }

    public override void Update()
    {
        base.Update();

        if (player.GetComponent<CharacterStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

        if (assassinateTimer < 0)
        {
            if (enemy.TryAssassinateBehindPlayer(0.15f))
            {
                enemy.ZeroVelocity();  // 传送开始时立即停止移动
                assassinateTimer = enemy.assassinateCooldown;
            }
        }

        assassinateTimer -= Time.deltaTime;
        emergencyTeleportCooldown -= Time.deltaTime;

        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance)
            if (CanAttack() && !enemy.isKnocked)
                stateMachine.ChangeState(enemy.attackState);

        // 先朝向玩家
        if (player.position.x > enemy.transform.position.x && enemy.facingDir == -1)
            enemy.Flip();
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1)
            enemy.Flip();

        // 紧急情况：若前方是墙或悬崖，立即传送（绕过正常冷却，但防止频繁触发）
        if ((enemy.IsWallDetected() || !enemy.IsGroundDetected()) && emergencyTeleportCooldown < 0)
        {
            if (enemy.TryAssassinateBehindPlayer(0.5f))
            {
                enemy.ZeroVelocity();
                emergencyTeleportCooldown = EMERGENCY_COOLDOWN;  // 设置紧急传送防重复触发
                assassinateTimer = enemy.assassinateCooldown;   // 同时重置正常冷却
            }
        }

        // 路径安全再向玩家方向推进（传送期间不移动）
        if (!enemy.canAssassinate)
            enemy.SetVelocity(enemy.moveSpeed * enemy.facingDir * 1.2f, rb.velocity.y);
        else
            enemy.ZeroVelocity();  // 传送期间保持静止
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
