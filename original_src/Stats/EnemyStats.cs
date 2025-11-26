using UnityEngine;
using System.Collections;

/// <summary>
/// 敌人属性系统 - 继承自CharacterStats
/// 实现敌人的耐力系统、格挡机制和死亡处理
/// 包含耐力恢复、格挡消耗和特殊伤害判定逻辑
/// </summary>
public class EnemyStats : CharacterStats
{
    private Enemy enemy;                                  // 敌人组件引用
    private Player player;                                // 玩家组件引用

    [Header("Endurance stats")]
    public bool canBlock;                                 // 是否可以格挡
    public Stat maxEndurance;                             // 最大耐力值
    public int currentEndurance;                          // 当前耐力值
    public int enduranceRestore;                          // 耐力恢复量
    public int blockCost;                                // 格挡消耗
    public int blockedCost;                              // 被格挡时的消耗

    public float restoreCooldown;                         // 耐力恢复冷却时间
    private float cooldownTimer;                          // 冷却计时器

    /// <summary>
    /// 初始化敌人属性
    /// </summary>
    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        player = PlayerManager.instance.player.GetComponent<Player>();

        currentEndurance = maxEndurance.GetValue();
    }

    /// <summary>
    /// 更新敌人属性状态
    /// </summary>
    protected override void Update()
    {
        base.Update();

        cooldownTimer -= Time.deltaTime;

        // 耐力恢复逻辑：冷却时间结束且耐力未满且未被眩晕时恢复耐力
        if (cooldownTimer < 0 && currentEndurance < maxEndurance.GetValue() && !enemy.isStunned)
        {
            cooldownTimer = restoreCooldown;

            currentEndurance += enduranceRestore;

            if (currentEndurance > maxEndurance.GetValue())
                currentEndurance = maxEndurance.GetValue();
        }
    }

    /// <summary>
    /// 敌人受到伤害处理
    /// </summary>
    /// <param name="_damage">伤害值</param>
    /// <param name="_attacker">攻击者</param>
    /// <param name="_canDoDamage">是否可以造成伤害</param>
    public override void TakeDamage(int _damage, Transform _attacker, bool _canDoDamage, bool _canCrit)
    {
        // 玩家攻击判定
        if (_attacker.GetComponent<Player>() != null)
        {
            // 玩家面向敌人或敌人被眩晕或强制伤害时，可以造成伤害
            if (player.facingDir == enemy.facingDir || enemy.isStunned || _canDoDamage || !canBlock)
            {
                _canDoDamage = true;
                GetComponent<EntityFX>()?.CreatePopUpText(_damage.ToString(), _canCrit);
                AudioManager.instance.PlaySFX(4); // 成功攻击音效
            }
            else
            {
                GetComponent<EntityFX>()?.CreatePopUpText("格挡");
                AudioManager.instance.PlaySFX(6); // 格挡音效
            }
        }
        else
        {
            // 其他攻击者（如技能）的判定
            float directionToAttackerX = _attacker.position.x - transform.position.x;
            if (directionToAttackerX * enemy.facingDir <= 0 || enemy.isStunned || _canDoDamage || !canBlock)
            {
                _canDoDamage = true;

                GetComponent<EntityFX>()?.CreatePopUpText(_damage.ToString(), _canCrit);

                // 分身或剑技能攻击音效
                if (_attacker.GetComponent<Clone_Skill_Controller>() != null || _attacker.GetComponent<Sword_Skill_Controller>() != null)
                    AudioManager.instance.PlaySFX(4);
            }
            else
            {
                GetComponent<EntityFX>()?.CreatePopUpText("格挡");
                AudioManager.instance.PlaySFX(6); // 格挡音效
            }
        }

        // 根据是否造成伤害进行不同处理
        if (_canDoDamage)
        {
            base.TakeDamage(_damage, _attacker, _canDoDamage, _canCrit);
            TakeDamageFX(_canCrit);
        }
        else
        {
            currentEndurance -= blockCost; // 格挡时消耗耐力
        }

        enemy.DamageEffect(_attacker, _canDoDamage, _canDoDamage);

        // 玩家被敌人格挡：恢复相机抖动与短暂停顿
        if (_attacker.GetComponent<Player>() != null && !_canDoDamage)
        {
            if (CombatFeedback.instance != null)
                CombatFeedback.instance.DoHitStop(0.12f);
            if (CinemachineShaker.instance != null)
                CinemachineShaker.instance.Shake(0.8f, 1.4f, 0.12f);
        }
    }

    /// <summary>
    /// 敌人死亡处理
    /// </summary>
    protected override void Die()
    {
        base.Die();

        enemy.Die();
    }
}
