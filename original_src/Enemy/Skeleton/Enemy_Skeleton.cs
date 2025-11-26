using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    #region States
    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }
    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }
    public SkeletonBlockedState blockedState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Move", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, stateMachine, "Die", this);
        blockedState = new SkeletonBlockedState(this, stateMachine, "Blocked", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        CloseCounterAttackWindow();
        discoverImage.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (enemyStats != null && enemyStats.currentEndurance <= 0 && stateMachine.currentState != stunnedState)
            stateMachine.ChangeState(stunnedState);

        if (isDead && stateMachine.currentState != deadState)
            SelfDestroy();
    }

    public override bool EnemyCanBeBlocked()
    {
        if (base.EnemyCanBeBlocked())
        {
            StartCoroutine(BlockKnockback(PlayerManager.instance.player.transform));

            Vector2 spawnPosition = new Vector2(transform.position.x + (0.5f * facingDir), transform.position.y + 1.2f);
            Instantiate(sparkPrefab, spawnPosition, transform.rotation);

            if (enemyStats != null)
                enemyStats.currentEndurance -= enemyStats.blockedCost;

            if (enemyStats.currentEndurance > 0)
                stateMachine.ChangeState(blockedState);

            return true;
        }

        return false;
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}
