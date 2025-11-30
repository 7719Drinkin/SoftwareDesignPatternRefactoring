using UnityEngine;

/// <summary>
/// 装备使用管理器 - 负责管理装备的使用逻辑（冷却时间、使用检查等）
/// 将装备使用逻辑从 Inventory 中解耦出来
/// </summary>
public class EquipmentUsageManager : MonoBehaviour, IEquipmentUsageManager
{
    private IInventory inventory;
    private IAudioManager audioManager;
    private GameEventBus eventBus;

    // 冷却时间记录
    private float lastTimeUseWeapon = -100;
    private float lastTimeUseArmor = -100;
    private float lastTimeUseAmulet = -100;
    private float lastTimeUseFlask = -100;

    private void Start()
    {
        // 获取服务依赖
        inventory = ServiceLocator.Instance.Get<IInventory>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();
    }

    /// <summary>
    /// 检查是否可以使用武器
    /// </summary>
    public bool CanUseWeapon()
    {
        ItemData_Equipment currentWeapon = inventory.GetEquipment(EquipmentType.Weapon);

        if (currentWeapon == null)
            return false;

        return Time.time > lastTimeUseWeapon + currentWeapon.itemCooldown;
    }

    /// <summary>
    /// 消耗武器冷却时间
    /// </summary>
    public void ConsumeWeaponCooldown()
    {
        ItemData_Equipment currentWeapon = inventory.GetEquipment(EquipmentType.Weapon);
        if (currentWeapon != null)
        {
            lastTimeUseWeapon = Time.time;

            // ========== 发布装备使用事件（Observer Pattern） ==========
            eventBus?.Publish(new EquipmentUsedEvent
            {
                EquipmentType = EquipmentType.Weapon
            });
        }
    }

    /// <summary>
    /// 检查是否可以使用护甲
    /// </summary>
    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = inventory.GetEquipment(EquipmentType.Armor);

        if (currentArmor == null)
            return false;

        bool canUseArmor = Time.time > lastTimeUseArmor + currentArmor.itemCooldown;
        if (canUseArmor)
        {
            lastTimeUseArmor = Time.time;

            // ========== 发布装备使用事件（Observer Pattern） ==========
            eventBus?.Publish(new EquipmentUsedEvent
            {
                EquipmentType = EquipmentType.Armor
            });
        }

        return canUseArmor;
    }

    /// <summary>
    /// 检查是否可以使用护身符
    /// </summary>
    public bool CanUseAmulet()
    {
        ItemData_Equipment currentAmulet = inventory.GetEquipment(EquipmentType.Amulet);

        if (currentAmulet == null)
            return false;

        bool canUseAmulet = Time.time > lastTimeUseAmulet + currentAmulet.itemCooldown;
        if (canUseAmulet)
        {
            lastTimeUseAmulet = Time.time;

            // ========== 发布装备使用事件（Observer Pattern） ==========
            eventBus?.Publish(new EquipmentUsedEvent
            {
                EquipmentType = EquipmentType.Amulet
            });
        }

        return canUseAmulet;
    }

    /// <summary>
    /// 检查是否可以使用药水
    /// </summary>
    public bool CanUseFlask()
    {
        ItemData_Equipment currentFlask = inventory.GetEquipment(EquipmentType.Flask);

        if (currentFlask == null)
            return false;

        bool canUseFlask = Time.time > lastTimeUseFlask + currentFlask.itemCooldown;
        if (canUseFlask)
        {
            lastTimeUseFlask = Time.time;
            audioManager?.PlaySFX(38); // 药水使用音效

            // ========== 发布装备使用事件（Observer Pattern） ==========
            eventBus?.Publish(new EquipmentUsedEvent
            {
                EquipmentType = EquipmentType.Flask
            });
        }

        return canUseFlask;
    }
}
