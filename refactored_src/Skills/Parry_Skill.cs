using System;
using UnityEngine;

/// <summary>
/// 格挡技能 - 继承自Skill基类
/// 实现玩家的格挡和反击功能
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Parry_Skill : Skill
{
    [Header("Parry")]
    public bool parry;

    [Header("Fight back")]
    public bool fightBack;

    private GameEventBus eventBus;

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
            case "Parry":
                parry = true;
                break;
            case "FightBack":
                fightBack = true;
                break;
        }
    }
}
