using System;
using UnityEngine;

/// <summary>
/// 冲刺技能 - 继承自Skill基类
/// 实现玩家的冲刺移动、冲刺分身和冲刺攻击功能
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
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

    [Header("Clone on dash")]
    public bool cloneOnDash;                              // 是否冲刺时创建分身

    [Header("Dash attack")]
    public bool dashAttack;                               // 是否解锁冲刺攻击

    private GameEventBus eventBus;

    /// <summary>
    /// 初始化冲刺技能
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // ========== 订阅技能解锁事件（Observer Pattern） ==========
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Subscribe<SkillUnlockedEvent>(OnSkillUnlocked);
        // ===========================================================
    }

    private void OnDestroy()
    {
        // 取消订阅
        eventBus?.Unsubscribe<SkillUnlockedEvent>(OnSkillUnlocked);
    }

    /// <summary>
    /// 处理技能解锁事件 - 从解锁类接收解锁通知
    /// </summary>
    private void OnSkillUnlocked(SkillUnlockedEvent evt)
    {
        switch (evt.SkillName)
        {
            case "Dash":
                dash = true;
                break;
            case "CloneOnDash":
                cloneOnDash = true;
                break;
            case "DashAttack":
                dashAttack = true;
                break;
        }
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

}
