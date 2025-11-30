using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 暗杀技能解锁管理器 - 负责管理暗杀技能的解锁逻辑
/// 将解锁逻辑从 Assassinate_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Assassinate_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot assassinateUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (assassinateUnlockButton != null)
            assassinateUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAssassinate);

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
        if (assassinateUnlockButton != null && assassinateUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Assassinate" });
    }

    /// <summary>
    /// 解锁暗杀技能
    /// </summary>
    private void UnlockAssassinate()
    {
        if (assassinateUnlockButton != null && 
            assassinateUnlockButton.CanUnlockSkillSlot() && 
            assassinateUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Assassinate" });
        }
    }
}

