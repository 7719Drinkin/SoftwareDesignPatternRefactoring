using UnityEngine;

/// <summary>
/// 药水使用命令
/// Receiver: Inventory
/// </summary>
public class FlaskCommand : ISkillCommand
{
    private readonly IInventory inventory;
    private readonly IEquipmentUsageManager equipmentUsageManager;
    private readonly Transform playerTransform;

    public FlaskCommand(IInventory inventory, IEquipmentUsageManager equipmentUsageManager, Transform playerTransform)
    {
        this.inventory = inventory;
        this.equipmentUsageManager = equipmentUsageManager;
        this.playerTransform = playerTransform;
    }

    public void Execute()
    {
        if (equipmentUsageManager.CanUseFlask())
        {
            ItemData_Equipment currentFlask = inventory.GetEquipment(EquipmentType.Flask);
            
            if (currentFlask != null)
            {
                // 执行装备效果
                currentFlask.ExecuteItemEffect(playerTransform);
            }
        }
    }
}

