using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Stunned info")]
    public float stunnedDuration;
    public Vector2 stunnedDistance;
    protected bool canBeBlocked;
    public bool isStunned;
    private Coroutine temporaryFreezeCoroutine;

    [SerializeField] protected GameObject counterImage;

    [Header("Move info")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime;
    private float defaultMoveSpeed;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    public float minAttackCoolDown;
    public float maxAttackCoolDown;
    public float minAttackSpeed;
    public float maxAttackSpeed;
    [HideInInspector] public float lastTimeAttacked;

    [Header("Detect info")]
    public float detectDistance;
    [SerializeField] protected GameObject discoverImage;

    [Header("Block info")]
    [SerializeField] protected Vector2 blockDistance;
    public GameObject sparkPrefab;

    [Header("Reward info")]
    [SerializeField] private int dropCurrency;
    public int experencePoints;
    public ExperienceDrop experienceDrop;

    public EnemyStateMachine stateMachine { get; private set; }

    [HideInInspector] public bool isDead;

    public EnemyStats enemyStats;

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;

        enemyStats = GetComponent<EnemyStats>();
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public override void SlowEntityBy(float slowPercentage, float slowDuration)
    {
        ReturnDefaultSpeed();
        CancelInvoke("ReturnDefaultSpeed");

        moveSpeed *= 1 - slowPercentage;
        anim.speed *= 1 - slowPercentage;

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTime(bool _timeFrozen)
    {
        isStunned = _timeFrozen;
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            // 只有在没有冰冻减速效果时才恢复默认速度
            if (!enemyStats.isChilled)
            {
                moveSpeed = defaultMoveSpeed;
                anim.speed = 1;
            }
        }
    }

    public virtual void FreezeTimeFor(float duration)
    {
        if (temporaryFreezeCoroutine != null)
            StopCoroutine(temporaryFreezeCoroutine);

        temporaryFreezeCoroutine = StartCoroutine(FreezeTimerCoroutine(duration));
    }

    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual bool EnemyCanBeBlocked()
    {
        if (canBeBlocked)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public virtual void OpenCounterAttackWindow()
    {
        canBeBlocked = true;
        // counterImage.SetActive(true); // 禁用格挡提示图片显示
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeBlocked = false;
        counterImage.SetActive(false);
    }

    #endregion

    public override void DamageEffect(Transform attackerTransform, bool canFlash, bool isKnocked)
    {
        if (canFlash)
            fx.StartCoroutine("FlashFX");

        if (isKnocked)
            StartCoroutine(HitKnockback(attackerTransform));
        else
            StartCoroutine(BlockKnockback(attackerTransform));
    }

    protected virtual IEnumerator BlockKnockback(Transform attackerTransform)
    {
        isKnocked = true;
        float direction = Mathf.Sign(attackerTransform.position.x - transform.position.x);
        Vector2 knockbackVelocity = new Vector2(blockDistance.x * -direction, blockDistance.y);
        rb.velocity = knockbackVelocity;

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    public virtual IEnumerator DiscoverPlayer()
    {
        discoverImage.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        discoverImage.SetActive(false);
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, detectDistance, whatIsPlayer);

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

    public void SelfDestroy() => Destroy(gameObject);

    public void StopTemporaryFreeze()
    {
        if (temporaryFreezeCoroutine != null)
        {
            StopCoroutine(temporaryFreezeCoroutine);
            temporaryFreezeCoroutine = null;
        }
    }

    public void PlayerGetCurrency()
    {
        PlayerManager.instance.currency += dropCurrency;
    }
}
