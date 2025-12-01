using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 护身符使用解锁管理器 - 负责管理护身符使用的解锁逻辑
/// 将解锁逻辑从 Inventory 中分离，实现解耦
/// 使用事件总线与 Inventory 通信，符合 Observer Pattern
/// </summary>
public class Inventory_Amulet_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot dashUseAmuletUnlockButton;   // 冲刺护身符解锁按钮
    [SerializeField] private UI_SkillTreeSlot jumpUseAmuletUnlockButton;     // 跳跃护身符解锁按钮
    [SerializeField] private UI_SkillTreeSlot swordUseAmuletUnlockButton;  // 剑攻击护身符解锁按钮

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (dashUseAmuletUnlockButton != null)
            dashUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDashUseAmulet);
        if (jumpUseAmuletUnlockButton != null)
            jumpUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockJumpUseAmulet);
        if (swordUseAmuletUnlockButton != null)
            swordUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordUseAmulet);

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
        if (dashUseAmuletUnlockButton != null && dashUseAmuletUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "DashUseAmulet" });
        
        if (jumpUseAmuletUnlockButton != null && jumpUseAmuletUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "JumpUseAmulet" });
        
        if (swordUseAmuletUnlockButton != null && swordUseAmuletUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "SwordUseAmulet" });
    }

    /// <summary>
    /// 解锁冲刺护身符使用
    /// </summary>
    private void UnlockDashUseAmulet()
    {
        if (dashUseAmuletUnlockButton != null && 
            dashUseAmuletUnlockButton.CanUnlockSkillSlot() && 
            dashUseAmuletUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "DashUseAmulet" });
        }
    }

    /// <summary>
    /// 解锁跳跃护身符使用
    /// </summary>
    private void UnlockJumpUseAmulet()
    {
        if (jumpUseAmuletUnlockButton != null && 
            jumpUseAmuletUnlockButton.CanUnlockSkillSlot() && 
            jumpUseAmuletUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "JumpUseAmulet" });
        }
    }

    /// <summary>
    /// 解锁剑攻击护身符使用
    /// </summary>
    private void UnlockSwordUseAmulet()
    {
        if (swordUseAmuletUnlockButton != null && 
            swordUseAmuletUnlockButton.CanUnlockSkillSlot() && 
            swordUseAmuletUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "SwordUseAmulet" });
        }
    }
}

