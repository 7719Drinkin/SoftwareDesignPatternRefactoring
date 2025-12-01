using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 冲刺技能解锁管理器 - 负责管理冲刺技能的所有解锁逻辑
/// 将解锁逻辑从 Dash_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Dash_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    [SerializeField] private UI_SkillTreeSlot dashAttackUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (dashUnlockButton != null)
            dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        if (cloneOnDashUnlockButton != null)
            cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        if (dashAttackUnlockButton != null)
            dashAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDashAttack);

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
        if (dashUnlockButton != null && dashUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Dash" });
        
        if (cloneOnDashUnlockButton != null && cloneOnDashUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "CloneOnDash" });
        
        if (dashAttackUnlockButton != null && dashAttackUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "DashAttack" });
    }

    /// <summary>
    /// 解锁冲刺技能
    /// </summary>
    private void UnlockDash()
    {
        if (dashUnlockButton != null && 
            dashUnlockButton.CanUnlockSkillSlot() && 
            dashUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Dash" });
        }
    }

    /// <summary>
    /// 解锁冲刺分身技能
    /// </summary>
    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton != null && 
            cloneOnDashUnlockButton.CanUnlockSkillSlot() && 
            cloneOnDashUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "CloneOnDash" });
        }
    }

    /// <summary>
    /// 解锁冲刺攻击技能
    /// </summary>
    private void UnlockDashAttack()
    {
        if (dashAttackUnlockButton != null && 
            dashAttackUnlockButton.CanUnlockSkillSlot() && 
            dashAttackUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "DashAttack" });
        }
    }
}

