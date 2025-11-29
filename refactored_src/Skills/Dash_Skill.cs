using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 冲刺技能 - 继承自Skill基类
/// 实现玩家的冲刺移动、冲刺分身和冲刺攻击功能
/// 支持技能解锁和事件系统
/// </summary>
public class Dash_Skill : Skill
{
    [Header("Dash Info")]
    public float dashSpeed;                                // 冲刺速度
    public float dashDuration;                            // 冲刺持续时间

    [HideInInspector]
    public float dashDir;                                  // 冲刺方向

    [Header("Dash")]
    public bool dash;                                      // 是否解锁冲刺
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton; // 冲刺解锁按钮

    [Header("Clone on dash")]
    public bool cloneOnDash;                              // 是否冲刺时创建分身
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton; // 冲刺分身解锁按钮

    [Header("Dash attack")]
    public bool dashAttack;                               // 是否解锁冲刺攻击
    [SerializeField] private UI_SkillTreeSlot dashAttackUnlockButton; // 冲刺攻击解锁按钮

    /// <summary>
    /// 初始化冲刺技能
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // 绑定技能解锁按钮事件
        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        dashAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    /// <summary>
    /// 延迟初始化协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        dash = dashUnlockButton.unlocked;
        cloneOnDash = cloneOnDashUnlockButton.unlocked;
        dashAttack = dashAttackUnlockButton.unlocked;

        // 触发已解锁技能的事件（从存档加载时）
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        if (dash)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Dash" });
        if (cloneOnDash)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "CloneOnDash" });
        if (dashAttack)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "DashAttack" });
    }

    /// <summary>
    /// 使用冲刺技能
    /// </summary>
    public override void UseSkill()
    {
        base.UseSkill();

        // ========== 发布到事件总线（Observer Pattern）==========
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Publish(new SkillUsedEvent
        {
            SkillName = "Dash",
            Cooldown = cooldown
        });
    }

    /// <summary>
    /// 解锁冲刺技能
    /// </summary>
    private void UnlockDash()
    {
        if (dashUnlockButton.CanUnlockSkillSlot() && dashUnlockButton.unlocked)
        {
            dash = true;
            
            // ========== 发布技能解锁事件到事件总线（Observer Pattern） ==========
            var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
            eventBus?.Publish(new SkillUnlockedEvent
            {
                SkillName = "Dash"
            });
        }
    }

    /// <summary>
    /// 解锁冲刺分身技能
    /// </summary>
    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.CanUnlockSkillSlot() && cloneOnDashUnlockButton.unlocked)
        {
            cloneOnDash = true;
            
            // ========== 发布技能解锁事件到事件总线（Observer Pattern） ==========
            var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
            eventBus?.Publish(new SkillUnlockedEvent
            {
                SkillName = "CloneOnDash"
            });
        }
    }

    /// <summary>
    /// 解锁冲刺攻击技能
    /// </summary>
    private void UnlockCloneAttack()
    {
        if (dashAttackUnlockButton.CanUnlockSkillSlot() && dashAttackUnlockButton.unlocked)
        {
            dashAttack = true;
            
            // ========== 发布技能解锁事件到事件总线（Observer Pattern） ==========
            var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
            eventBus?.Publish(new SkillUnlockedEvent
            {
                SkillName = "DashAttack"
            });
        }
    }
}
