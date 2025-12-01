using UnityEngine;
using System;

/// <summary>
/// 玩家管理器 - 管理玩家的核心数据
/// 负责货币、经验值、等级系统和存档管理
/// 实现ISaveManager接口，支持玩家数据的保存和加载
/// </summary>
public class PlayerManager : MonoBehaviour, ISaveManager, IPlayerManager
{
    [Header("Player info")]
    [SerializeField] private Player player;
    [SerializeField] private int currency;
    [SerializeField] private int currentExperience;
    [SerializeField] private int playerLevel;

    private IAudioManager audioManager;

    public Player Player { get => player; set => player = value; }
    public int Currency { get => currency; set => currency = value; }
    public int CurrentExperience { get => currentExperience; set => currentExperience = value; }
    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }

    public event Action OnPlayerDataLoaded;

    private void Awake()
    {
        Invoke(nameof(InitializeServices), 0.1f);
    }

    private void InitializeServices()
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
    }

    /// <summary>
    /// 检查是否有足够的货币
    /// </summary>
    /// <param name="price">价格</param>
    /// <returns>是否有足够货币</returns>
    public bool HaveEnoughMoney(int price)
    {
        if (price > currency)
            return false;

        currency -= price;
        return true;
    }

    /// <summary>
    /// 加载玩家数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void LoadData(GameData data)
    {
        currency = data.currency;
        currentExperience = data.currentExperience;
        playerLevel = data.playerLevel;

        // 如果player还没有初始化，启动延迟初始化
        if (player == null || player.stats == null)
        {
            StartCoroutine(DelayedLoadData());
            return;
        }

        // 立即应用数据
        ApplyPlayerData();
    }

    /// <summary>
    /// 延迟加载数据协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator DelayedLoadData()
    {
        // 等待player初始化完成
        while (player == null || player.stats == null)
        {
            yield return null;
        }

        // 应用数据
        ApplyPlayerData();
    }

    /// <summary>
    /// 应用玩家数据
    /// </summary>
    private void ApplyPlayerData()
    {
        if (player != null && player.stats != null)
        {
            player.stats.level.SetValue(playerLevel);

            // 重新应用等级带来的属性加成
            int additionalLevels = playerLevel - 1; // 减去初始1级
            if (additionalLevels > 0)
            {
                player.stats.strength.AddModifier(2 * additionalLevels);
                player.stats.agility.AddModifier(2 * additionalLevels);
                player.stats.intelligence.AddModifier(2 * additionalLevels);
                player.stats.vitality.AddModifier(10 * additionalLevels);
            }

            // 触发数据加载完成事件
            OnPlayerDataLoaded?.Invoke();

            player.stats.currentHealth = player.stats.GetMaxHealthValue();
        }
    }

    /// <summary>
    /// 保存玩家数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void SaveData(ref GameData data)
    {
        data.currency = currency;
        data.currentExperience = currentExperience;
        data.playerLevel = playerLevel;
    }

    /// <summary>
    /// 增加经验值
    /// </summary>
    /// <param name="amount">经验值数量</param>
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        audioManager.PlaySFX(21);

        // 检查升级
        while (currentExperience >= 200 * (playerLevel / 5 + 1))
        {
            currentExperience -= 200 * (playerLevel / 5 + 1);
            playerLevel++;

            audioManager.PlaySFX(22);

            // 升级时增加属性
            if (player != null && player.stats != null)
            {
                player.stats.strength.AddModifier(3);
                player.stats.agility.AddModifier(1);
                player.stats.intelligence.AddModifier(2);
                player.stats.vitality.AddModifier(10);
                player.stats.level.SetValue(playerLevel);

                // 升级时恢复满血
                player.stats.currentHealth = player.stats.GetMaxHealthValue();
                ServiceLocator.Instance.Get<IInventory>().UpdateSlotUI();
            }
        }
    }
}
