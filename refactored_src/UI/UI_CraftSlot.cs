using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment data)
    {
        if (data == null)
            return;

        item.data = data;

        itemImage.sprite = data.icon;
        itemText.text = data.itemName;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (itemImage.sprite == null)
            return;

        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(24);

        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
