using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 格挡技能解锁管理器 - 负责管理格挡技能的所有解锁逻辑
/// 将解锁逻辑从 Parry_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Parry_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    [SerializeField] private UI_SkillTreeSlot fightBackUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (parryUnlockButton != null)
            parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        if (fightBackUnlockButton != null)
            fightBackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFightBack);

        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }

    /// <summary>
    /// 延迟初始化协程 - 处理存档加载时的状态同步
    /// </summary>
    private IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;

        // 发布已解锁技能的事件（从存档加载时）
        if (parryUnlockButton != null && parryUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Parry" });
        
        if (fightBackUnlockButton != null && fightBackUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "FightBack" });
    }

    /// <summary>
    /// 解锁格挡技能
    /// </summary>
    private void UnlockParry()
    {
        if (parryUnlockButton != null && 
            parryUnlockButton.CanUnlockSkillSlot() && 
            parryUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Parry" });
        }
    }

    /// <summary>
    /// 解锁反击技能
    /// </summary>
    private void UnlockFightBack()
    {
        if (fightBackUnlockButton != null && 
            fightBackUnlockButton.CanUnlockSkillSlot() && 
            fightBackUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "FightBack" });
        }
    }
}

