using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 水晶技能解锁管理器 - 负责管理水晶技能的所有解锁逻辑
/// 将解锁逻辑从 Crystal_Skill 中分离，实现解耦
/// 使用事件总线与技能类通信，符合 Observer Pattern
/// </summary>
public class Crystal_Skill_Unlock : MonoBehaviour
{
    [Header("Unlock Buttons")]
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    [SerializeField] private UI_SkillTreeSlot mirageUnlockButton;
    [SerializeField] private UI_SkillTreeSlot explodeUnlockButton;
    [SerializeField] private UI_SkillTreeSlot moveUnlockButton;
    [SerializeField] private UI_SkillTreeSlot multiUnlockButton;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 绑定按钮事件
        if (crystalUnlockButton != null)
            crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        if (mirageUnlockButton != null)
            mirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirage);
        if (explodeUnlockButton != null)
            explodeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplode);
        if (moveUnlockButton != null)
            moveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMove);
        if (multiUnlockButton != null)
            multiUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMulti);

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
        // 这样技能类就能从事件中获取状态，而不需要直接读取 UI
        if (crystalUnlockButton != null && crystalUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Crystal" });
        
        if (mirageUnlockButton != null && mirageUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Mirage" });
        
        if (explodeUnlockButton != null && explodeUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Explode" });
        
        if (moveUnlockButton != null && moveUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Move" });
        
        if (multiUnlockButton != null && multiUnlockButton.unlocked)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "MultiCrystal" });
    }

    /// <summary>
    /// 解锁水晶技能
    /// </summary>
    private void UnlockCrystal()
    {
        if (crystalUnlockButton != null && 
            crystalUnlockButton.CanUnlockSkillSlot() && 
            crystalUnlockButton.unlocked)
        {
            // 发布技能解锁事件，技能类订阅处理
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Crystal" });
        }
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
    /// 解锁爆炸效果
    /// </summary>
    private void UnlockExplode()
    {
        if (explodeUnlockButton != null && 
            explodeUnlockButton.CanUnlockSkillSlot() && 
            explodeUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Explode" });
        }
    }

    /// <summary>
    /// 解锁移动效果
    /// </summary>
    private void UnlockMove()
    {
        if (moveUnlockButton != null && 
            moveUnlockButton.CanUnlockSkillSlot() && 
            moveUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "Move" });
        }
    }

    /// <summary>
    /// 解锁多重水晶
    /// </summary>
    private void UnlockMulti()
    {
        if (multiUnlockButton != null && 
            multiUnlockButton.CanUnlockSkillSlot() && 
            multiUnlockButton.unlocked)
        {
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "MultiCrystal" });
        }
    }
}

