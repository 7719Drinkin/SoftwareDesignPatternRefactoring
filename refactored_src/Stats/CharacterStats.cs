using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 元素类型枚举
/// </summary>
public enum ElementType
{
    Auto,       // 自动选择（根据最高伤害）
    Fire,       // 火焰效果
    Ice,        // 寒冰效果
    Lightning   // 闪电效果
}

/// <summary>
/// 角色属性系统 - 管理角色的所有属性和状态效果
/// 包含基础属性、伤害计算、元素效果和状态异常
/// 支持属性修改、伤害计算和状态效果应用
/// </summary>
public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;                                   // 特效组件

    // ========== 服务依赖 ==========
    protected IPlayerManager playerManager;
    protected IAudioManager audioManager;

    [Header("Level details")]
    public Stat level;                                     // 等级

    [Header("Major stats")]
    public Stat strength;                                  // 力量：5点增加1点伤害和1%暴击威力
    public Stat agility;                                   // 敏捷：1点增加1%闪避和1%暴击率
    public Stat intelligence;                              // 智力：1点增加5点魔法伤害和3点魔法抗性
    public Stat vitality;                                  // 活力：1点增加5点生命值

    [Header("Offensive stats")]
    public Stat damage;                                    // 基础伤害
    public Stat critChance;                                // 暴击率
    public Stat critPower;                                // 暴击威力

    [Header("Defensive stats")]
    public Stat maxHealth;                                 // 最大生命值
    public Stat armor;                                     // 护甲
    public Stat evasion;                                   // 闪避
    public Stat magicResistance;                          // 魔法抗性

    [Header("Magic stats")]
    public Stat fireDamage;                                // 火焰伤害
    public Stat iceDamage;                                 // 寒冰伤害
    public Stat lightningDamage;                          // 闪电伤害

    // 状态效果
    public bool isIgnited;                                 // 是否被点燃
    public bool isChilled;                                 // 是否被冰冻
    public bool isShocked;                                 // 是否被电击

    // 状态计时器
    private float ignitedTimer;                            // 点燃计时器
    private float chilledTimer;                            // 冰冻计时器
    private float shockedTimer;                            // 电击计时器

    private float ignitedCooldown = 0.3f;                  // 点燃伤害间隔
    private float igniteDamageTimer;                       // 点燃伤害计时器
    private int igniteDamage;                              // 点燃伤害值
    private int shockDamage;                               // 电击伤害值
    [SerializeField] private GameObject shockStrikePrefab; // 闪电打击预制体

    public int currentHealth;                               // 当前生命值
    public bool isInvincible { get; private set; }         // 是否无敌
    public bool isDead { get; private set; }               // 是否死亡

    // ========== Strategy Pattern ==========
    private IDamageCalculationStrategy physicalDamageStrategy;     // 物理伤害策略
    private IDamageCalculationStrategy magicalDamageStrategy;      // 魔法伤害策略（默认自动选择元素）
    // private IDamageCalculationStrategy trueDamageStrategy;      // 可扩展的真实伤害
    // private IDamageCalculationStrategy percentDamageStrategy;   // 可扩展的百分比伤害
    private IDamageCalculationStrategy currentDamageStrategy;      // 当前使用的伤害策略（可运行时切换）
    // =====================================

    /// <summary>
    /// 初始化角色属性
    /// </summary>
    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();

        // 等级缩放：只对敌人应用，玩家从PlayerManager获取加成
        if (GetComponent<Player>() == null)
        {
            // 敌人：根据敌人预设等级应用等级加成
            Modify(strength, 2);
            Modify(armor, 5);
            Modify(agility, 1);
            Modify(vitality, 80);
        }

        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();

        // 通过ServiceLocator获取依赖
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();

        // ========== 初始化策略模式 ==========
        physicalDamageStrategy = new PhysicalDamageStrategy();
        magicalDamageStrategy = new MagicalDamageStrategy(); 
        currentDamageStrategy = physicalDamageStrategy;  // 默认使用物理伤害策略
        // ===================================
    }

    /// <summary>
    /// 更新状态效果计时器
    /// </summary>
    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        // 检查状态效果是否结束
        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        // 处理点燃伤害
        if (igniteDamageTimer < 0 && isIgnited)
        {
            igniteDamageTimer = ignitedCooldown;
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();
        }
    }

    /// <summary>
    /// 临时增加属性值
    /// </summary>
    /// <param name="_modifier">修改值</param>
    /// <param name="_duration">持续时间</param>
    /// <param name="_statToModify">要修改的属性</param>
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify) => StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));

    /// <summary>
    /// 属性修改协程
    /// </summary>
    /// <param name="_modifier">修改值</param>
    /// <param name="_duration">持续时间</param>
    /// <param name="_statToModify">要修改的属性</param>
    /// <returns></returns>
    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    /// <summary>
    /// 根据等级修改属性
    /// </summary>
    /// <param name="_stat">要修改的属性</param>
    /// <param name="perLevel">每级加成</param>
    private void Modify(Stat _stat, int _perLevel)
    {
        for (int i = 1; i < level.GetValue(); i++)
        {
            _stat.AddModifier(_perLevel);
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    /// <param name="attacker">攻击者</param>
    /// <param name="canDoDamage">是否造成伤害</param>
    public virtual void TakeDamage(int damage, Transform attacker, bool canDoDamage, bool canCrit)
    {
        DecreaseHealthBy(damage);

        if (currentHealth <= 0 && !isDead)
            Die();
    }

    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    /// <summary>
    /// 增加生命值
    /// </summary>
    /// <param name="amount">增加量</param>
    public void IncreaseHealthBy(int amount)
    {
        currentHealth += amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
    }

    /// <summary>
    /// 减少生命值
    /// </summary>
    /// <param name="damage">伤害值</param>
    protected virtual void DecreaseHealthBy(int damage) => currentHealth -= damage;

    /// <summary>
    /// 设置伤害计算策略（策略模式的核心优势：运行时切换策略）
    /// </summary>
    /// <param name="strategy">伤害计算策略</param>
    public void SetDamageStrategy(IDamageCalculationStrategy strategy)
    {
        if (strategy != null)
            currentDamageStrategy = strategy;
    }

    /// <summary>
    /// 重置为默认物理伤害策略
    /// </summary>
    public void ResetToPhysicalDamageStrategy()
    {
        currentDamageStrategy = physicalDamageStrategy;
    }

    /// <summary>
    /// 造成物理伤害 - 使用策略模式
    /// </summary>
    /// <param name="targetStats">目标属性</param>
    /// <param name="attacker">攻击者</param>
    /// <param name="canDoDamage">是否造成伤害</param>
    public virtual void DoDamage(CharacterStats targetStats, Transform attacker, bool canDoDamage)
    {
        // 目标无敌则不造成伤害
        if (targetStats != null && targetStats.isInvincible)
        {
            targetStats.GetComponent<EntityFX>().CreatePopUpText("闪避");
            return;
        }

        if (TargetCanAvoidAttack(targetStats))
        {
            targetStats.GetComponent<EntityFX>().CreatePopUpText("闪避");
            return;
        }

        // ========== 使用策略模式计算物理伤害 ==========
        // 使用当前策略（默认是物理伤害策略，但可以在运行时切换）
        DamageResult result = currentDamageStrategy.CalculateDamage(this, targetStats, attacker, null);
        targetStats.TakeDamage(result.FinalDamage, attacker, canDoDamage, result.IsCritical);
        // =============================================
    }

    protected virtual void TakeDamageFX(bool canCrit)
    {
        if (fx != null)
            fx.CreateHitFX(transform, canCrit);
    }

    #region Magical damage and ailments
    /// <summary>
    /// 造成魔法伤害（自动选择元素）
    /// </summary>
    /// <param name="targetStats">目标属性</param>
    /// <param name="attacker">攻击者</param>
    public virtual void DoMagicalDamage(CharacterStats targetStats, Transform attacker)
    {
        DoMagicalDamage(targetStats, attacker, ElementType.Auto);
    }

    /// <summary>
    /// 造成魔法伤害（指定元素）- 使用策略模式
    /// </summary>
    /// <param name="targetStats">目标属性</param>
    /// <param name="attacker">攻击者</param>
    /// <param name="specifiedElement">指定元素类型</param>
    public virtual void DoMagicalDamage(CharacterStats targetStats, Transform attacker, ElementType specifiedElement)
    {
        if (targetStats == null || targetStats.isInvincible)
            return;

        // ========== 使用策略模式计算魔法伤害 ==========
        // 切换为魔法伤害策略，并传入元素类型参数
        IDamageCalculationStrategy previousStrategy = currentDamageStrategy;  // 保存之前的策略
        currentDamageStrategy = magicalDamageStrategy;  // 切换到魔法伤害策略
        
        // 使用策略计算伤害，传入元素类型参数
        DamageResult result = currentDamageStrategy.CalculateDamage(this, targetStats, attacker, specifiedElement);

        // 应用伤害
        targetStats.TakeDamage(result.FinalDamage, attacker, true, false);

        // 应用元素效果（如果策略返回了元素类型）
        if (result.CanApplyElementEffect && result.ElementType.HasValue)
        {
            ApplyElementEffect(targetStats, result.ElementType.Value);
        }

        // 恢复之前的策略（魔法伤害是临时切换）
        currentDamageStrategy = previousStrategy;
        // =============================================
    }

    /// <summary>
    /// 应用元素效果
    /// </summary>
    private void ApplyElementEffect(CharacterStats targetStats, ElementType elementType)
    {
        if (targetStats == null)
            return;

        bool canApplyIgnite = false;
        bool canApplyChill = false;
        bool canApplyShock = false;

        // 根据元素类型确定要应用的效果
        switch (elementType)
        {
            case ElementType.Fire:
                canApplyIgnite = (fireDamage.GetValue() > 0);
                if (canApplyIgnite)
                    targetStats.SetupIgniteDamage(Mathf.RoundToInt(targetStats.currentHealth * Mathf.Clamp(fireDamage.GetValue() * 0.0002f, 0, 0.01f)));
                break;
            case ElementType.Ice:
                canApplyChill = (iceDamage.GetValue() > 0);
                break;
            case ElementType.Lightning:
                canApplyShock = (lightningDamage.GetValue() > 0);
                if (canApplyShock)
                    targetStats.SetupShockDamage(Mathf.RoundToInt(lightningDamage.GetValue() * 1.2f));
                break;
        }

        // 应用状态异常
        targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    /// <summary>
    /// 应用状态异常
    /// </summary>
    /// <param name="_ignite">是否点燃</param>
    /// <param name="_chill">是否冰冻</param>
    /// <param name="_shock">是否电击</param>
    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        // 状态异常互斥：只能同时拥有一种状态
        bool canApplyIgnite = !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        // 应用点燃效果
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = 4;

            if (fx != null)
                fx.IgniteFxFor(4);
        }

        // 应用冰冻效果
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = 4;

            GetComponent<Entity>().SlowEntityBy(0.5f, 4);
            if (fx != null)
                fx.ChillFxFor(4);
        }

        // 应用电击效果
        if (_shock && canApplyShock)
        {
            if (!isShocked)
                ApplyShock(_shock);
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                ThunderStike(); // 如果已经电击，则触发闪电打击
            }
        }
    }

    /// <summary>
    /// 应用电击状态
    /// </summary>
    /// <param name="_shock">是否电击</param>
    public void ApplyShock(bool _shock)
    {
        isShocked = _shock;
        shockedTimer = 4;

        if (fx != null)
            fx.ShockFxFor(4);
    }

    /// <summary>
    /// 闪电打击 - 电击状态的连锁效果
    /// </summary>
    public void ThunderStike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // 查找最近的敌人
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.transform != transform)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        // 创建闪电打击效果
        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage + 3 * playerManager.Player.stats.intelligence.GetValue(), closestEnemy.GetComponent<CharacterStats>());
        }
    }

    /// <summary>
    /// 设置点燃伤害
    /// </summary>
    /// <param name="_damage">伤害值</param>
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    /// <summary>
    /// 设置电击伤害
    /// </summary>
    /// <param name="_damage">伤害值</param>
    public void SetupShockDamage(int _damage) => shockDamage = _damage;

    #endregion

    #region calculations
    /// <summary>
    /// 检查目标护甲并计算最终伤害
    /// </summary>
    /// <param name="targetStats">目标属性</param>
    /// <param name="totalDamage">总伤害</param>
    /// <returns>最终伤害</returns>
    private int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
    {
        if (targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(targetStats.armor.GetValue() * 0.7f); // 冰冻状态护甲效果降低30%
        else
            totalDamage -= targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        return totalDamage;
    }

    /// <summary>
    /// 检查目标是否可以闪避攻击
    /// </summary>
    /// <param name="targetStats">目标属性</param>
    /// <returns>是否可以闪避</returns>
    private bool TargetCanAvoidAttack(CharacterStats targetStats)
    {

        if (targetStats.GetComponent<Enemy>() != null && targetStats.GetComponent<Enemy>().isStunned)
            return false;

        int totalEvation = targetStats.evasion.GetValue() + targetStats.agility.GetValue();

        if (isShocked)
            totalEvation += 30; // 电击状态增加30%闪避率

        if (Random.Range(0, 100) < totalEvation)
            return true;

        return false;
    }

    /// <summary>
    /// 检查是否可以暴击
    /// </summary>
    /// <returns>是否可以暴击</returns>
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
            return true;

        return false;
    }

    /// <summary>
    /// 计算暴击伤害
    /// </summary>
    /// <param name="_damage">基础伤害</param>
    /// <returns>暴击伤害</returns>
    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue() * 5) * 0.01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    #endregion

    /// <summary>
    /// 角色死亡处理
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity() => Die();

    /// <summary>
    /// 获取最大生命值
    /// </summary>
    /// <returns>最大生命值</returns>
    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    /// <summary>
    /// 根据属性类型获取对应的属性对象
    /// </summary>
    /// <param name="statType">属性类型</param>
    /// <returns>属性对象</returns>
    public Stat StatOfType(StatType statType)
    {
        switch (statType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.maxHealth:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicResistance:
                return magicResistance;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightningDamage:
                return lightningDamage;
            case StatType.level:
                return level;
        }

        return null;
    }
}
