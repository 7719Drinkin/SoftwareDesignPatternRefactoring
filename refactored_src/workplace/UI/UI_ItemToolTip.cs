using TMPro;
using UnityEngine;

public class UI_ItemToolTip : MonoBehaviour
{
    public static UI_ItemToolTip instance { get; private set; }

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        gameObject.SetActive(false);
    }

    public void ShowToolTip(ItemData_Equipment item)
    {
        itemNameText.text = item.itemName;
        switch (item.equipmentType)
        {
            case EquipmentType.Weapon:
                itemTypeText.text = "武器";
                break;
            case EquipmentType.Armor:
                itemTypeText.text = "防具";
                break;
            case EquipmentType.Amulet:
                itemTypeText.text = "护身符";
                break;
            case EquipmentType.Flask:
                itemTypeText.text = "药品";
                break;
        }
        itemDescription.text = item.GetDescription();

        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
