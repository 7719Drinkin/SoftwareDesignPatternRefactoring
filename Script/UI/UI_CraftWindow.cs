using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;

    [SerializeField] private Image[] materialImage;

    public void SetupCraftWindow(ItemData_Equipment data)
    {
        if (data == null)
            return;

        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }

        if (data.craftingMaterials == null)
            return;

        for (int i = 0; i < data.craftingMaterials.Count; i++)
        {
            if (i >= materialImage.Length)
                break;

            var mat = data.craftingMaterials[i];
            if (mat == null || mat.data == null)
                continue;

            materialImage[i].sprite = mat.data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = mat.stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        if (itemIcon != null)
            itemIcon.sprite = data.icon;
        if (itemName != null)
            itemName.text = data.name;
        if (itemDescription != null)
            itemDescription.text = data.GetDescription();

        if (craftButton != null)
            craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(data, data.craftingMaterials));
    }
}
