using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 分身技能解锁管理器 - 负责管理分身技能的所有解锁逻辑
/// 将解锁逻辑从 Clone_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Clone_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot mirageUnlockButton;
    [SerializeField] private UI_SkillTreeSlot cloneUnlockButton;
    [SerializeField] private UI_SkillTreeSlot duplicateUnlockButton;
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (mirageUnlockButton != null)
            mirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirage);
        if (cloneUnlockButton != null)
            cloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockClone);
        if (duplicateUnlockButton != null)
            duplicateUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDuplicate);
        if (crystalInsteadUnlockButton != null)
            crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);

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
        if (mirageUnlockButton != null && mirageUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Mirage" });
        
        if (cloneUnlockButton != null && cloneUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Clone" });
        
        if (duplicateUnlockButton != null && duplicateUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Duplicate" });
        
        if (crystalInsteadUnlockButton != null && crystalInsteadUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "CrystalInstead" });
    }

    /// <summary>
    /// 解锁幻影效果
    /// </summary>
    private void UnlockMirage()
    {
        if (mirageUnlockButton != null && 
            mirageUnlockButton.CanUnlockSkillSlot() && 
            mirageUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Mirage" });
        }
    }

    /// <summary>
    /// 解锁分身技能
    /// </summary>
    private void UnlockClone()
    {
        if (cloneUnlockButton != null && 
            cloneUnlockButton.CanUnlockSkillSlot() && 
            cloneUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Clone" });
        }
    }

    /// <summary>
    /// 解锁分身复制效果
    /// </summary>
    private void UnlockDuplicate()
    {
        if (duplicateUnlockButton != null && 
            duplicateUnlockButton.CanUnlockSkillSlot() && 
            duplicateUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Duplicate" });
        }
    }

    /// <summary>
    /// 解锁水晶替代分身效果
    /// </summary>
    private void UnlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton != null && 
            crystalInsteadUnlockButton.CanUnlockSkillSlot() && 
            crystalInsteadUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "CrystalInstead" });
        }
    }
}

