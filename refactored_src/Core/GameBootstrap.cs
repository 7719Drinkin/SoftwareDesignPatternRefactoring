using UnityEngine;

// ========== Refactored with Service Locator Pattern ==========
// 目的：统一的服务注册和游戏初始化入口
// 作用：在游戏启动时注册所有核心服务到ServiceLocator
// 重构日期：2025-11-28
// ============================================================

/// <summary>
/// 游戏引导程序 - 游戏启动时的统一初始化入口
/// 负责：
/// 1. 注册所有核心服务到ServiceLocator
/// 2. 初始化游戏核心系统
/// 3. 确保服务按正确顺序初始化
/// </summary>
[DefaultExecutionOrder(-100)]  // 确保在其他脚本之前执行
public class GameBootstrap : MonoBehaviour
{
    [Header("Service References")]
    [Tooltip("如果为空，会自动查找场景中的实例")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private DroppedItemManager droppedItemManager;
    [SerializeField] private EquipmentUsageManager equipmentUsageManager;
    [SerializeField] private AmuletSkillManager amuletSkillManager;

    private void Awake()
    {
        // 确保场景切换时旧的服务不会残留
        ServiceLocator.Reset();
        GameFacade.Reset();

        // 1. 自动查找服务（如果未在Inspector中设置）
        FindServices();

        // 2. 注册核心服务到ServiceLocator
        RegisterCoreServices();

        // 3. 初始化外观模式（Facade Pattern）
        GameFacade.Instance.Initialize();
    }

    /// <summary>
    /// 自动查找场景中的服务实例
    /// </summary>
    private void FindServices()
    {
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (playerManager == null)
            playerManager = FindObjectOfType<PlayerManager>();

        if (inventory == null)
            inventory = FindObjectOfType<Inventory>();

        if (skillManager == null)
            skillManager = FindObjectOfType<SkillManager>();

        if (saveManager == null)
            saveManager = FindObjectOfType<SaveManager>();

        if (droppedItemManager == null)
            droppedItemManager = FindObjectOfType<DroppedItemManager>();

        if (equipmentUsageManager == null)
            equipmentUsageManager = FindObjectOfType<EquipmentUsageManager>();

        if (amuletSkillManager == null)
            amuletSkillManager = FindObjectOfType<AmuletSkillManager>();
    }

    /// <summary>
    /// 注册所有核心服务到ServiceLocator
    /// </summary>
    private void RegisterCoreServices()
    {
        // ========== 阶段1：注册现有的单例Manager ==========
        // 音频管理器（通过接口注册）
        if (audioManager != null)
            ServiceLocator.Instance.RegisterSingleton<IAudioManager>(audioManager);

        // 游戏管理器（通过接口注册）
        if (gameManager != null)
            ServiceLocator.Instance.RegisterSingleton<IGameManager>(gameManager);

        // 玩家管理器（通过接口注册）
        if (playerManager != null)
            ServiceLocator.Instance.RegisterSingleton<IPlayerManager>(playerManager);

        // 物品栏系统（通过接口注册）
        if (inventory != null)
            ServiceLocator.Instance.RegisterSingleton<IInventory>(inventory);

        // 技能管理器（通过接口注册）
        if (skillManager != null)
            ServiceLocator.Instance.RegisterSingleton<ISkillManager>(skillManager);

        // 存档管理器（通过接口注册）
        if (saveManager != null)
            ServiceLocator.Instance.RegisterSingleton<ISaveManagerService>(saveManager);

        // 掉落物管理器（通过接口注册）
        if (droppedItemManager != null)
            ServiceLocator.Instance.RegisterSingleton<IDroppedItemManager>(droppedItemManager);

        // 装备使用管理器（通过接口注册）
        if (equipmentUsageManager != null)
            ServiceLocator.Instance.RegisterSingleton<IEquipmentUsageManager>(equipmentUsageManager);

        // 护身符技能管理器（通过接口注册）
        if (amuletSkillManager != null)
            ServiceLocator.Instance.RegisterSingleton<IAmuletSkillManager>(amuletSkillManager);

        // ========== 阶段2：注册事件系统 ==========
        // 游戏事件总线（Observer Pattern）
        GameEventBus eventBus = new GameEventBus();
        ServiceLocator.Instance.RegisterSingleton<GameEventBus>(eventBus);
    }
}

