using System.Collections.Generic;
using UnityEngine;

public class DroppedItemManager : MonoBehaviour, ISaveManager, IDroppedItemManager
{
    [SerializeField] private GameObject itemObjectPrefab;
    [SerializeField] private GameObject experienceObjectPrefab;

    private List<GameObject> activeItems = new List<GameObject>();
    private List<GameObject> activeExperience = new List<GameObject>();

    // 缓存物品数据库
    private Dictionary<string, ItemData> itemDatabaseCache;

    private void Awake()
    {
        // 初始化物品数据库缓存
        InitializeItemDatabaseCache();
    }

    /// <summary>
    /// 生成掉落物品
    /// </summary>
    public void SpawnItem(ItemData itemData, Vector3 position, Vector2 velocity)
    {
        if (itemObjectPrefab == null) return;

        GameObject newItem = Instantiate(itemObjectPrefab, position, Quaternion.identity);
        newItem.GetComponent<ItemObject>().SetupItem(itemData, velocity);
        activeItems.Add(newItem);
    }

    /// <summary>
    /// 生成经验球
    /// </summary>
    public void SpawnExperience(int experienceAmount, Vector3 position, Vector2 velocity)
    {
        if (experienceObjectPrefab == null) return;

        GameObject newExp = Instantiate(experienceObjectPrefab, position, Quaternion.identity);
        newExp.GetComponent<ExperienceObject>().SetupObject(experienceAmount, velocity);
        activeExperience.Add(newExp);
    }


    /// <summary>
    /// 清除所有掉落物品
    /// </summary>
    public void ClearAllDroppedItems()
    {
        foreach (var item in activeItems)
            if (item != null) Destroy(item);
        activeItems.Clear();

        foreach (var exp in activeExperience)
            if (exp != null) Destroy(exp);
        activeExperience.Clear();
    }

    #region ISaveManager Implementation

    public void SaveData(ref GameData data)
    {
        // 清除旧数据
        data.droppedItems.Clear();
        data.droppedExperience.Clear();

        // 保存物品
        foreach (var item in activeItems)
        {
            if (item != null)
            {
                ItemObject itemObject = item.GetComponent<ItemObject>();
                if (itemObject != null)
                {
                    Vector3 position = GetUniquePosition(item.transform.position, data.droppedItems);
                    data.droppedItems.Add(position, itemObject.GetItemData().itemId);
                }
            }
        }

        // 保存经验球
        foreach (var exp in activeExperience)
        {
            if (exp != null)
            {
                ExperienceObject expObject = exp.GetComponent<ExperienceObject>();
                if (expObject != null)
                {
                    Vector3 position = GetUniquePosition(exp.transform.position, data.droppedExperience);
                    data.droppedExperience.Add(position, expObject.GetExperienceAmount());
                }
            }
        }
    }

    public void LoadData(GameData data)
    {
        ClearAllDroppedItems();

        // 重新生成物品（静止状态）
        foreach (var kvp in data.droppedItems)
        {
            ItemData item = GetItemById(kvp.Value);
            if (item != null)
                SpawnItem(item, kvp.Key, Vector2.zero);
        }

        // 重新生成经验球（静止状态）
        foreach (var kvp in data.droppedExperience)
        {
            SpawnExperience(kvp.Value, kvp.Key, Vector2.zero);
        }
    }

    private Vector3 GetUniquePosition(Vector3 originalPosition, SerializableDictionary<Vector3, string> existingItems)
    {
        Vector3 position = originalPosition;
        float offset = 0.1f;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!existingItems.ContainsKey(position))
                return position;

            // 添加随机偏移
            position = originalPosition + new Vector3(
                Random.Range(-offset, offset),
                Random.Range(-offset, offset),
                0
            );
            offset += 0.1f;  // 每次增加偏移量
        }

        return originalPosition;  // 兜底方案
    }

    private Vector3 GetUniquePosition(Vector3 originalPosition, SerializableDictionary<Vector3, int> existingItems)
    {
        Vector3 position = originalPosition;
        float offset = 0.1f;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            if (!existingItems.ContainsKey(position))
                return position;

            // 添加随机偏移
            position = originalPosition + new Vector3(
                Random.Range(-offset, offset),
                Random.Range(-offset, offset),
                0
            );
            offset += 0.1f;  // 每次增加偏移量
        }

        return originalPosition;  // 兜底方案
    }

    /// <summary>
    /// 初始化物品数据库缓存
    /// </summary>
    private void InitializeItemDatabaseCache()
    {
        itemDatabaseCache = new Dictionary<string, ItemData>();

        List<ItemData> allItems = GetItemDatabase();
        foreach (var item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemId))
            {
                itemDatabaseCache[item.itemId] = item;
            }
        }
    }

    /// <summary>
    /// 根据itemId查找物品数据（使用缓存）
    /// </summary>
    private ItemData GetItemById(string itemId)
    {
        if (itemDatabaseCache != null && itemDatabaseCache.ContainsKey(itemId))
        {
            return itemDatabaseCache[itemId];
        }
        return null;
    }

    /// <summary>
    /// 获取物品数据库（参考Inventory的实现）
    /// </summary>
    private List<ItemData> GetItemDatabase()
    {
        List<ItemData> itemDatabase = new List<ItemData>();

        string[] assetNames = UnityEditor.AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            var SOpath = UnityEditor.AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDatabase.Add(itemData);
        }

        return itemDatabase;
    }

    #endregion
}
