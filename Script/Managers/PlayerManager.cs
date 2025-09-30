using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;

    [Header("Player info")]
    public Player player;
    public int currency;
    public int currentExperience;
    public int playerLevel;

    // 数据加载完成事件
    public System.Action OnPlayerDataLoaded;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HaveEnoughMoney(int price)
    {
        if (price > currency)
            return false;

        currency -= price;
        return true;
    }

    public void LoadData(GameData data)
    {
        this.currency = data.currency;
        this.currentExperience = data.currentExperience;
        this.playerLevel = data.playerLevel;

        // 如果player还没有初始化，启动延迟初始化
        if (player == null || player.stats == null)
        {
            StartCoroutine(DelayedLoadData());
            return;
        }

        // 立即应用数据
        ApplyPlayerData();
    }

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

    private void ApplyPlayerData()
    {
        if (player != null && player.stats != null)
        {
            player.stats.level.SetValue(playerLevel);

            // 重新应用等级带来的属性加成
            int additionalLevels = playerLevel - 1; // 减去初始1级
            if (additionalLevels > 0)
            {
                player.stats.strength.AddModifier(3 * additionalLevels);
                player.stats.agility.AddModifier(1 * additionalLevels);
                player.stats.intelligence.AddModifier(2 * additionalLevels);
                player.stats.vitality.AddModifier(10 * additionalLevels);
            }

            // 触发数据加载完成事件
            OnPlayerDataLoaded?.Invoke();

            player.stats.currentHealth = player.stats.GetMaxHealthValue();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.currency = this.currency;
        data.currentExperience = this.currentExperience;
        data.playerLevel = this.playerLevel;
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // 检查升级
        while (currentExperience >= 200 * (playerLevel / 5 + 1))
        {
            currentExperience -= 200 * (playerLevel / 5 + 1);
            playerLevel++;

            AudioManager.instance.PlaySFX(22);

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
                Inventory.instance.UpdateSlotUI();
            }
        }
    }
}
