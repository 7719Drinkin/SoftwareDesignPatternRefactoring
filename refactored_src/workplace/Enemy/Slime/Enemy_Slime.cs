using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlimeSize { Large, Medium, Small }

public class Enemy_Slime : Enemy
{
    #region States
    public SlimeMoveState moveState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    public SlimeBattleState battleState { get; private set; }

    #endregion

    [Header("Slime Split")]
    [SerializeField] private SlimeSize size = SlimeSize.Large;
    [SerializeField] private Enemy_Slime mediumPrefab;
    [SerializeField] private Enemy_Slime smallPrefab;
    [SerializeField] private Vector2 spawnOffset;

    protected override void Awake()
    {
        base.Awake();

        moveState = new SlimeMoveState(this, stateMachine, "Move", this);
        attackState = new SlimeAttackState(this, stateMachine, "Attack", this);
        deadState = new SlimeDeadState(this, stateMachine, "Die", this);
        battleState = new SlimeBattleState(this, stateMachine, "Move", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(moveState);

        CloseCounterAttackWindow();
        if (discoverImage != null)
            discoverImage.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (isDead && stateMachine.currentState != deadState)
            SelfDestroy();
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        AudioManager.instance.PlaySFX(46);

        switch (size)
        {
            case SlimeSize.Large:
                SpawnChildren(mediumPrefab);
                Destroy(gameObject); 
                return;

            case SlimeSize.Medium:
                SpawnChildren(smallPrefab);
                Destroy(gameObject);
                return;

            case SlimeSize.Small:
                base.Die(); 
                stateMachine.ChangeState(deadState);
                return;
        }
    }

    private void SpawnChildren(Enemy_Slime childPrefab)
    {
        if (childPrefab == null) return;

		var left = Instantiate(childPrefab, (Vector2)transform.position + new Vector2(-spawnOffset.x, spawnOffset.y), Quaternion.identity);

		var right = Instantiate(childPrefab, (Vector2)transform.position + new Vector2(+spawnOffset.x, spawnOffset.y), Quaternion.identity);
    }

    public override bool EnemyCanBeBlocked()
    {
        if (base.EnemyCanBeBlocked())
        {
            Die();
            return true;
        }
        return false;
    }
}
