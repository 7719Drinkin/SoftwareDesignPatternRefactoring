using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLoseItems;

    public override void GenerateDrop()
    {
        IInventory inventory = ServiceLocator.Instance.Get<IInventory>();

        List<InventoryItem> currentEquipment = new List<InventoryItem>(inventory.GetEquipmentList());

        foreach (InventoryItem item in currentEquipment)
            if (Random.Range(0, 100) < chanceToLoseItems)
            {
                DropItem(item.data);
                inventory.UnequipItem(item.data as ItemData_Equipment);
            }
    }
}
