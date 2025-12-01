using UnityEngine;

/***************************************************************
 *  Refactored with: Facade Pattern
 *  Pattern Type: Structural
 *
 *  Document Reference:
 *  - See report section "2.2.5 Facade Pattern (Structural)"
 ***************************************************************/
/// <summary>
/// 游戏外观类 - 提供统一的游戏系统访问入口
/// 同时实现外观模式（简化子系统）和适配器模式（适配 ServiceLocator 接口）
/// </summary>
public class GameFacade
{
    private static GameFacade instance;

    // ========== 服务缓存 ==========
    private IPlayerManager playerManager;
    private ISkillManager skillManager;
    private IInventory inventory;
    private IAudioManager audioManager;
    private IGameManager gameManager;
    private ISaveManagerService saveManager;
    private IDroppedItemManager droppedItemManager;
    private IEquipmentUsageManager equipmentUsageManager;
    private IAmuletSkillManager amuletSkillManager;
    private GameEventBus eventBus;

    /// <summary>
    /// 单例实例
    /// </summary>
    public static GameFacade Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameFacade();
            }
            return instance;
        }
    }

    /// <summary>
    /// 初始化外观（从 ServiceLocator 获取所有服务）
    /// </summary>
    public void Initialize()
    {
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        skillManager = ServiceLocator.Instance.Get<ISkillManager>();
        inventory = ServiceLocator.Instance.Get<IInventory>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        gameManager = ServiceLocator.Instance.Get<IGameManager>();
        saveManager = ServiceLocator.Instance.Get<ISaveManagerService>();
        droppedItemManager = ServiceLocator.Instance.Get<IDroppedItemManager>();
        equipmentUsageManager = ServiceLocator.Instance.Get<IEquipmentUsageManager>();
        amuletSkillManager = ServiceLocator.Instance.Get<IAmuletSkillManager>();
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        Debug.Log("[GameFacade] Initialized with all services (Facade Pattern)");
    }

    /// <summary>
    /// 重置外观（场景切换时调用）
    /// </summary>
    public static void Reset()
    {
        instance = null;
    }

    // ========== 便捷访问属性 - 适配器模式（Adapter Pattern） ==========
    // 将 ServiceLocator.Get<T>() 的泛型方法接口适配为简单的属性访问
    // Before: ServiceLocator.Instance.Get<IPlayerManager>()
    // After:  GameFacade.Instance.Player

    /// <summary>
    /// 玩家管理器 - 适配器属性
    /// </summary>
    public IPlayerManager Player => playerManager;

    /// <summary>
    /// 技能管理器 - 适配器属性
    /// </summary>
    public ISkillManager Skills => skillManager;

    /// <summary>
    /// 物品栏系统 - 适配器属性
    /// </summary>
    public IInventory Inventory => inventory;

    /// <summary>
    /// 音频管理器 - 适配器属性
    /// </summary>
    public IAudioManager Audio => audioManager;

    /// <summary>
    /// 游戏管理器 - 适配器属性
    /// </summary>
    public IGameManager Game => gameManager;

    /// <summary>
    /// 存档管理器 - 适配器属性
    /// </summary>
    public ISaveManagerService Save => saveManager;

    /// <summary>
    /// 掉落物管理器 - 适配器属性
    /// </summary>
    public IDroppedItemManager DroppedItems => droppedItemManager;

    /// <summary>
    /// 装备使用管理器 - 适配器属性
    /// </summary>
    public IEquipmentUsageManager EquipmentUsage => equipmentUsageManager;

    /// <summary>
    /// 护身符技能管理器 - 适配器属性
    /// </summary>
    public IAmuletSkillManager AmuletSkills => amuletSkillManager;

    /// <summary>
    /// 事件总线 - 适配器属性
    /// </summary>
    public GameEventBus Events => eventBus;

    // ========== 便捷方法：封装常用操作 ==========

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(int index)
    {
        audioManager?.PlaySFX(index);
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM(int index)
    {
        audioManager?.PlayBGM(index);
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM(int index)
    {
        audioManager?.StopSFX(index);
    }

    /// <summary>
    /// 发布游戏事件
    /// </summary>
    public void PublishEvent<T>(T gameEvent) where T : IGameEvent
    {
        eventBus?.Publish(gameEvent);
    }

    /// <summary>
    /// 订阅游戏事件
    /// </summary>
    public void SubscribeEvent<T>(System.Action<T> handler) where T : IGameEvent
    {
        eventBus?.Subscribe(handler);
    }

    /// <summary>
    /// 取消订阅游戏事件
    /// </summary>
    public void UnsubscribeEvent<T>(System.Action<T> handler) where T : IGameEvent
    {
        eventBus?.Unsubscribe(handler);
    }

    /// <summary>
    /// 获取玩家对象
    /// </summary>
    public Player GetPlayer()
    {
        return playerManager?.Player;
    }

    /// <summary>
    /// 获取玩家货币
    /// </summary>
    public int GetCurrency()
    {
        return playerManager?.Currency ?? 0;
    }

    /// <summary>
    /// 添加货币
    /// </summary>
    public void AddCurrency(int amount)
    {
        if (playerManager != null)
            playerManager.Currency += amount;
    }

    /// <summary>
    /// 扣除货币
    /// </summary>
    public bool SpendCurrency(int amount)
    {
        if (playerManager != null && playerManager.HaveEnoughMoney(amount))
        {
            playerManager.Currency -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame(bool pause)
    {
        gameManager?.PauseGame(pause);
    }

    /// <summary>
    /// 重启场景
    /// </summary>
    public void RestartScene()
    {
        gameManager?.ReStartScene();
    }

    /// <summary>
    /// 保存游戏
    /// </summary>
    public void SaveGame()
    {
        saveManager?.SaveGame();
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    public void LoadGame()
    {
        saveManager?.LoadGame();
    }
}
/***************************************************************
 * End
 ***************************************************************/

