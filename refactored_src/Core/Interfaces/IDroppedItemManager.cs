using UnityEngine;

/// <summary>
/// 掉落物管理器接口
/// 定义物品和经验球的生成和管理功能
/// </summary>
public interface IDroppedItemManager
{
    /// <summary>
    /// 生成掉落物品
    /// </summary>
    /// <param name="itemData">物品数据</param>
    /// <param name="position">生成位置</param>
    /// <param name="velocity">初始速度</param>
    void SpawnItem(ItemData itemData, Vector3 position, Vector2 velocity);

    /// <summary>
    /// 生成经验球
    /// </summary>
    /// <param name="experienceAmount">经验值</param>
    /// <param name="position">生成位置</param>
    /// <param name="velocity">初始速度</param>
    void SpawnExperience(int experienceAmount, Vector3 position, Vector2 velocity);

    /// <summary>
    /// 清除所有掉落物品
    /// </summary>
    void ClearAllDroppedItems();
}

