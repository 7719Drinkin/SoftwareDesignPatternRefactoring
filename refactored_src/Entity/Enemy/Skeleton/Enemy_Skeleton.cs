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

    private SkeletonStates skeletonStates;

    protected override void Awake()
    {
        base.Awake();

        skeletonStates = new SkeletonStateFactory().CreateStates(this, stateMachine);

        idleState = skeletonStates.Idle;
        moveState = skeletonStates.Move;
        battleState = skeletonStates.Battle;
        attackState = skeletonStates.Attack;
        stunnedState = skeletonStates.Stunned;
        deadState = skeletonStates.Dead;
        blockedState = skeletonStates.Blocked;
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(skeletonStates.InitialState);

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
            StartCoroutine(BlockKnockback(playerManager.Player.transform));

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
