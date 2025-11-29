using System;
using System.Collections.Generic;

/// <summary>
/// 物品栏接口 - 定义所有物品管理操作
/// </summary>
public interface IInventory
{
    void AddItem(ItemData item);
    void RemoveItem(ItemData item);
    bool CanAddItemToInventory();
    void EquipItem(ItemData item);
    void UnequipItem(ItemData_Equipment itemToRemove);
    ItemData_Equipment GetEquipment(EquipmentType type);
    List<InventoryItem> GetEquipmentList();
    bool CanUseWeapon();
    void ConsumeWeaponCooldown();
    bool CanUseArmor();
    bool CanUseAmulet();
    bool CanUseFlask();
    bool CanCraft(ItemData_Equipment itemToCraft, List<InventoryItem> requiredMaterials);
    void UpdateSlotUI();
    void UnlockDashUseAmulet();
    void UnlockJumpUseAmulet();
    void UnlockSwordUseAmulet();

    event Action OnWeaponEquiped;
    event Action OnArmorEquiped;
    event Action OnAmuletEquiped;
    event Action OnFlaskEquiped;
    event Action OnWeaponUsed;
    event Action OnArmorUsed;
    event Action OnAmuletUsed;
    event Action OnFlaskUsed;
    event Action OnWeaponUnequiped;
    event Action OnArmorUnequiped;
    event Action OnAmuletUnequiped;
    event Action OnFlaskUnequiped;

    bool DashUseAmulet { get; set; }
    bool JumpUseAmulet { get; set; }
    bool SwordUseAmulet { get; set; }
}
