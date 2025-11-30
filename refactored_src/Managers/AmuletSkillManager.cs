using UnityEngine;

/// <summary>
/// 护身符技能管理器 - 负责管理护身符技能状态
/// 将护身符技能状态从 Inventory 中解耦出来
/// </summary>
public class AmuletSkillManager : MonoBehaviour, IAmuletSkillManager
{
    [Header("Use amulet")]
    [SerializeField] private bool dashUseAmulet;
    [SerializeField] private bool jumpUseAmulet;
    [SerializeField] private bool swordUseAmulet;

    public bool DashUseAmulet { get => dashUseAmulet; set => dashUseAmulet = value; }
    public bool JumpUseAmulet { get => jumpUseAmulet; set => jumpUseAmulet = value; }
    public bool SwordUseAmulet { get => swordUseAmulet; set => swordUseAmulet = value; }

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取事件总线
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // ========== 订阅技能解锁事件（Observer Pattern） ==========
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
            case "DashUseAmulet":
                dashUseAmulet = true;
                break;
            case "JumpUseAmulet":
                jumpUseAmulet = true;
                break;
            case "SwordUseAmulet":
                swordUseAmulet = true;
                break;
        }
    }
}
