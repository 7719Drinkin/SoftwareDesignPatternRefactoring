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
    [SerializeField] private UI_MaterialImage[] materialImageUI;

    private UI ui;

    private void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void SetupCraftWindow(ItemData_Equipment data)
    {
        if (data == null)
            return;

        craftButton.onClick.RemoveAllListeners();

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
            materialImageUI[i].SetMaterialName("");
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
            materialImageUI[i].SetMaterialName(mat.data.itemName);
        }

        if (itemIcon != null)
            itemIcon.sprite = data.icon;
        if (itemName != null)
            itemName.text = data.name;
        if (itemDescription != null)
            itemDescription.text = data.GetDescription();

        if (craftButton != null)
            craftButton.onClick.AddListener(() => {
                bool canCraft = Inventory.instance.CanCraft(data, data.craftingMaterials);
                if (canCraft)
                    ui.CreateUI_PopUpText("合成成功");
                else
                    ui.CreateUI_PopUpText("合成失败，材料不足");
            });
    }
}
