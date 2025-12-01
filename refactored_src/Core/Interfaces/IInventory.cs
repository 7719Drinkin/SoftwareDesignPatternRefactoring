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
    bool CanCraft(ItemData_Equipment itemToCraft, List<InventoryItem> requiredMaterials);
    void UpdateSlotUI();
}
