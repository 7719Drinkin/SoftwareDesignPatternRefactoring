/// <summary>
/// 装备使用管理器接口 - 负责管理装备的使用逻辑（冷却时间、使用检查等）
/// </summary>
public interface IEquipmentUsageManager
{
    /// <summary>
    /// 检查是否可以使用武器
    /// </summary>
    bool CanUseWeapon();

    /// <summary>
    /// 消耗武器冷却时间（武器使用后调用）
    /// </summary>
    void ConsumeWeaponCooldown();

    /// <summary>
    /// 检查是否可以使用护甲
    /// </summary>
    bool CanUseArmor();

    /// <summary>
    /// 检查是否可以使用护身符
    /// </summary>
    bool CanUseAmulet();

    /// <summary>
    /// 检查是否可以使用药水
    /// </summary>
    bool CanUseFlask();
}

