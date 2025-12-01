/// <summary>
/// 装备变更事件
/// </summary>
public class EquipmentChangedEvent : IGameEvent
{
    public string EventName => "EquipmentChanged";
    public EquipmentType EquipmentType { get; set; }
    public ItemData_Equipment Equipment { get; set; }
    public bool IsEquipped { get; set; }  // true=装备, false=卸下
}

/// <summary>
/// 装备使用事件
/// </summary>
public class EquipmentUsedEvent : IGameEvent
{
    public string EventName => "EquipmentUsed";
    public EquipmentType EquipmentType { get; set; }
}

