using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人类 - 继承自Entity
/// 实现敌人的AI行为、状态机、攻击系统和奖励机制
/// 包含敌人特有的眩晕、格挡、发现玩家等功能
/// </summary>
public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;      // 玩家层级掩码

    // ========== 服务依赖 ==========
    protected IPlayerManager playerManager;
    protected IAudioManager audioManager;

    [Header("Stunned info")]
    public float stunnedDuration;                           // 眩晕持续时间
    public Vector2 stunnedDistance;                         // 眩晕时的击退距离
    protected bool canBeBlocked;                            // 是否可以被格挡
    public bool isStunned;                                 // 是否处于眩晕状态
    private Coroutine temporaryFreezeCoroutine;            // 临时冰冻协程

    [SerializeField] protected GameObject counterImage;    // 格挡提示图片

    [Header("Move info")]
    public float moveSpeed;                                 // 移动速度
    public float idleTime;                                  // 空闲时间
    public float battleTime;                               // 战斗时间
    private float defaultMoveSpeed;                        // 默认移动速度

    [Header("Flip info")]
    [SerializeField] private float flipCooldown = 0.5f;    // 翻转冷却（秒）
    private float lastFlipTime;                             // 上次翻转时间

    [Header("Attack info")]
    public float attackDistance;                            // 攻击距离
    public float attackCooldown;                            // 攻击冷却时间
    public float minAttackCoolDown;                        // 最小攻击冷却时间
    public float maxAttackCoolDown;                        // 最大攻击冷却时间
    public float minAttackSpeed;                           // 最小攻击速度
    public float maxAttackSpeed;                            // 最大攻击速度
    [HideInInspector] public float lastTimeAttacked;       // 上次攻击时间

    [Header("Detect info")]
    public float detectDistance;                            // 检测距离
    [SerializeField] protected GameObject discoverImage;    // 发现玩家提示图片

    [Header("Block info")]
    [SerializeField] protected Vector2 blockDistance;      // 格挡时的击退距离
    public GameObject sparkPrefab;                          // 火花特效预制体

    [Header("Reward info")]
    public ItemDrop dropSystem;                             // 掉落系统
    [SerializeField] private int dropCurrency;              // 掉落货币数量
    public int experencePoints;                             // 经验点数
    public ExperienceDrop experienceDrop;                   // 经验掉落组件

    public EnemyStateMachine stateMachine { get; private set; } // 敌人状态机

    [HideInInspector] public bool isDead;                   // 是否死亡

    public EnemyStats enemyStats;                           // 敌人属性

    /// <summary>
    /// 初始化敌人状态机和属性
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new EnemyStateMachine();

        defaultMoveSpeed = moveSpeed;

        enemyStats = GetComponent<EnemyStats>();

        // 通过ServiceLocator获取依赖
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
    }

    /// <summary>
    /// 更新敌人状态
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (stateMachine == null || stateMachine.currentState == null)
            return;

        stateMachine.currentState.Update();

        CheckNormalColor();
    }

    /// <summary>
    /// 减速敌人（重写父类方法）
    /// </summary>
    /// <param name="slowPercentage">减速百分比</param>
    /// <param name="slowDuration">减速持续时间</param>
    public override void SlowEntityBy(float slowPercentage, float slowDuration)
    {
        ReturnDefaultSpeed();
        CancelInvoke("ReturnDefaultSpeed");

        moveSpeed *= 1 - slowPercentage;
        anim.speed *= 1 - slowPercentage;

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    /// <summary>
    /// 恢复默认速度（重写父类方法）
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    /// <summary>
    /// 敌人翻转（带冷却）
    /// </summary>
    public override void Flip()
    {
        if (Time.time < lastFlipTime + flipCooldown)
            return;

        lastFlipTime = Time.time;
        base.Flip();
    }

    /// <summary>
    /// 冰冻时间（用于时间停止技能）
    /// </summary>
    /// <param name="_timeFrozen">是否冰冻</param>
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

    /// <summary>
    /// 临时冰冻指定时间
    /// </summary>
    /// <param name="duration">冰冻持续时间</param>
    public virtual void FreezeTimeFor(float duration)
    {
        if (temporaryFreezeCoroutine != null)
            StopCoroutine(temporaryFreezeCoroutine);

        temporaryFreezeCoroutine = StartCoroutine(FreezeTimerCoroutine(duration));
    }

    /// <summary>
    /// 冰冻计时器协程
    /// </summary>
    /// <param name="_seconds">冰冻秒数</param>
    /// <returns></returns>
    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds)
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window
    /// <summary>
    /// 检查敌人是否可以被格挡
    /// </summary>
    /// <returns>是否可以被格挡</returns>
    public virtual bool EnemyCanBeBlocked()
    {
        if (canBeBlocked)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 开启格挡反击窗口
    /// </summary>
    public virtual void OpenCounterAttackWindow()
    {
        canBeBlocked = true;
        // counterImage.SetActive(true); // 禁用格挡提示图片显示
    }

    /// <summary>
    /// 关闭格挡反击窗口
    /// </summary>
    public virtual void CloseCounterAttackWindow()
    {
        canBeBlocked = false;

        if (counterImage != null)
            counterImage.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 伤害效果处理（重写父类方法）
    /// </summary>
    /// <param name="attackerTransform">攻击者位置</param>
    /// <param name="canFlash">是否可以闪烁</param>
    /// <param name="isKnocked">是否击退</param>
    public override void DamageEffect(Transform attackerTransform, bool canFlash, bool isKnocked)
    {
        if (canFlash)
            fx.StartCoroutine("FlashFX");

        if (isKnocked)
            StartCoroutine(HitKnockback(attackerTransform));
        else
            StartCoroutine(BlockKnockback(attackerTransform));
    }

    /// <summary>
    /// 格挡击退协程
    /// </summary>
    /// <param name="attackerTransform">攻击者位置</param>
    /// <returns></returns>
    protected virtual IEnumerator BlockKnockback(Transform attackerTransform)
    {
        isKnocked = true;
        float direction = Mathf.Sign(attackerTransform.position.x - transform.position.x);
        Vector2 knockbackVelocity = new Vector2(blockDistance.x * -direction, blockDistance.y);
        rb.velocity = knockbackVelocity;

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
    }

    /// <summary>
    /// 发现玩家协程
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DiscoverPlayer()
    {
        if (discoverImage != null)
            discoverImage.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        if (discoverImage != null)
            discoverImage.SetActive(false);
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    /// <summary>
    /// 检测是否发现玩家
    /// </summary>
    /// <returns>射线检测结果</returns>
    public virtual RaycastHit2D IsPlayerDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, detectDistance, whatIsPlayer);

    /// <summary>
    /// 绘制调试信息（重写父类方法）
    /// </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y));
    }

    /// <summary>
    /// 自我销毁
    /// </summary>
    public void SelfDestroy() => Destroy(gameObject);

    /// <summary>
    /// 停止临时冰冻
    /// </summary>
    public void StopTemporaryFreeze()
    {
        if (temporaryFreezeCoroutine != null)
        {
            StopCoroutine(temporaryFreezeCoroutine);
            temporaryFreezeCoroutine = null;
        }
    }

    /// <summary>
    /// 玩家获得货币
    /// </summary>
    public void PlayerGetCurrency()
    {
        playerManager.Currency += dropCurrency;
    }

    public void CheckNormalColor()
    {
        bool isRedBlinkInvoking = fx != null && fx.IsInvoking("RedColorBlink");
        bool isChillInvoking = fx != null && fx.IsInvoking("ChillColorFx");
        bool isShockInvoking = fx != null && fx.IsInvoking("ShockColorFx");
        bool isIgniteInvoking = fx != null && fx.IsInvoking("IgniteColorFx");

        // 如果正在执行任何颜色效果，直接返回，不改变颜色
        if (isRedBlinkInvoking || isChillInvoking || isShockInvoking || isIgniteInvoking)
        {
            return;
        }

        sr.color = Color.white;
    }

    public override void Die()
    {
        base.Die();

        PlayerGetCurrency();

        // 生成经验掉落
        if (experienceDrop != null)
        {
            experienceDrop.SetExperienceAmount(experencePoints);
            experienceDrop.GenerateExperienceDrop();
        }

        dropSystem.GenerateDrop();
    }
}
