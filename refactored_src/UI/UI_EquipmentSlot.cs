using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equipment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (itemImage.sprite == null)
            return;

        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(24);

        ServiceLocator.Instance.Get<IInventory>().UnequipItem(item.data as ItemData_Equipment);
        ServiceLocator.Instance.Get<IInventory>().AddItem(item.data as ItemData_Equipment);
        ClearUpSlot();

        ui.itemToolTip.HideToolTip();
    }
}
