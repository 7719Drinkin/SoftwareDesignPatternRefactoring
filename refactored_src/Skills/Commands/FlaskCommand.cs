using UnityEngine;

/// <summary>
/// 药水使用命令
/// Receiver: Inventory
/// </summary>
public class FlaskCommand : ISkillCommand
{
    private readonly IInventory inventory;
    private readonly Transform playerTransform;

    public FlaskCommand(IInventory inventory, Transform playerTransform)
    {
        this.inventory = inventory;
        this.playerTransform = playerTransform;
    }

    public void Execute()
    {
        if (inventory.CanUseFlask())
        {
            ItemData_Equipment currentFlask = inventory.GetEquipment(EquipmentType.Flask);
            
            if (currentFlask != null)
            {
                currentFlask.ExecuteItemEffect(playerTransform);
            }
        }
    }
}

