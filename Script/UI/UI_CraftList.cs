using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<ItemData_Equipment> craftEquipment;

    [SerializeField] private bool isDefault;

    private void Start()
    {
        var parent = transform.parent;
        if (parent != null && parent.childCount > 0)
        {
            var first = parent.GetChild(0);
            var list = first.GetComponent<UI_CraftList>();
            if (list != null)
                list.SetupCraftList();
        }
        SetupDefaultCraftWindow();
    }

    public void SetupCraftList()
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
            Destroy(craftSlotParent.GetChild(i).gameObject);

        for (int i = 0; i < craftEquipment.Count; i++)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();

        AudioManager.instance.PlaySFX(24);
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment != null && craftEquipment.Count > 0 && craftEquipment[0] != null && isDefault)
        {
            var ui = GetComponentInParent<UI>();
            if (ui != null && ui.craftWindow != null)
                ui.craftWindow.SetupCraftWindow(craftEquipment[0]);
        }
    }
}
