using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 剑技能解锁管理器 - 负责管理剑技能的所有解锁逻辑
/// 将解锁逻辑从 Sword_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Sword_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private UI_SkillTreeSlot freezeEnemyUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (swordUnlockButton != null)
            swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        if (bounceUnlockButton != null)
            bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        if (pierceUnlockButton != null)
            pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        if (spinUnlockButton != null)
            spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        if (freezeEnemyUnlockButton != null)
            freezeEnemyUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFreezeEnemy);

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
        if (swordUnlockButton != null && swordUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Sword" });
        
        if (freezeEnemyUnlockButton != null && freezeEnemyUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "FreezeEnemy" });

        // 根据解锁状态设置剑的类型（优先级：旋转 > 穿透 > 弹跳 > 普通）
        if (spinUnlockButton != null && spinUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Spin" });
        else if (pierceUnlockButton != null && pierceUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Pierce" });
        else if (bounceUnlockButton != null && bounceUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Bounce" });
    }

    /// <summary>
    /// 解锁剑技能
    /// </summary>
    private void UnlockSword()
    {
        if (swordUnlockButton != null && 
            swordUnlockButton.CanUnlockSkillSlot() && 
            swordUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Sword" });
        }
    }

    /// <summary>
    /// 解锁弹跳剑
    /// </summary>
    private void UnlockBounce()
    {
        if (bounceUnlockButton != null && 
            bounceUnlockButton.CanUnlockSkillSlot() && 
            bounceUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Bounce" });
        }
    }

    /// <summary>
    /// 解锁穿透剑
    /// </summary>
    private void UnlockPierce()
    {
        if (pierceUnlockButton != null && 
            pierceUnlockButton.CanUnlockSkillSlot() && 
            pierceUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Pierce" });
        }
    }

    /// <summary>
    /// 解锁旋转剑
    /// </summary>
    private void UnlockSpin()
    {
        if (spinUnlockButton != null && 
            spinUnlockButton.CanUnlockSkillSlot() && 
            spinUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Spin" });
        }
    }

    /// <summary>
    /// 解锁冰冻敌人效果
    /// </summary>
    private void UnlockFreezeEnemy()
    {
        if (freezeEnemyUnlockButton != null && 
            freezeEnemyUnlockButton.CanUnlockSkillSlot() && 
            freezeEnemyUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "FreezeEnemy" });
        }
    }
}

