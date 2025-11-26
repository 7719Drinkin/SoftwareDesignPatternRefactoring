using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    [SerializeField] private string skillName;
    [SerializeField] private int skillPrice;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedColor;

    public bool unlocked;
    private UI ui;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;
    [SerializeField] private Image skillImage;

    private void OnValidate()
    {
        gameObject.name = skillName;
    }

    private void Start()
    {
        skillImage = GetComponent<Image>();

        skillImage.color = lockedColor;

        ui = GetComponentInParent<UI>();

        UpdateUIState();
    }

    private void UpdateUIState()
    {
        if (skillImage == null)
            skillImage = GetComponent<Image>();
        if (skillImage == null)
            return;

        if (unlocked)
            skillImage.color = Color.white;
        else
            skillImage.color = lockedColor;
    }

    public bool CanUnlockSkillSlot()
    {
        if (unlocked)
        {
            ui.CreateUI_PopUpText("技能已解锁");
            AudioManager.instance.PlaySFX(29);
            return false;
        }

        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                ui.CreateUI_PopUpText("需要前置技能");
                AudioManager.instance.PlaySFX(29);
                return false;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                ui.CreateUI_PopUpText("技能冲突");
                AudioManager.instance.PlaySFX(29);
                return false;
            }
        }

        if (!PlayerManager.instance.HaveEnoughMoney(skillPrice))
        {
            ui.CreateUI_PopUpText("金币不足");
            AudioManager.instance.PlaySFX(29);
            return false;
        }

        unlocked = true;

        UpdateUIState();

        ui.skillToolTip.HideSkillToolTip();

        ui.CreateUI_PopUpText("技能解锁成功");
        AudioManager.instance.PlaySFX(26);

        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 先显示提示框，便于获取其RectTransform尺寸
        ui.skillToolTip.ShowSkillToolTip(skillDescription, skillName, "消耗 " + skillPrice.ToString() + " 金币");

        // 鼠标屏幕坐标
        Vector2 mousePosition = Input.mousePosition;

        // 计算屏幕安全位置（不越界）
        RectTransform tooltipRectTransform = ui.skillToolTip.GetComponent<RectTransform>();
        Canvas parentCanvas = ui.GetComponentInParent<Canvas>();

        if (tooltipRectTransform != null && parentCanvas != null)
        {
            // 提示框像素尺寸（考虑Canvas缩放）
            Rect pixelAdjustedRect = RectTransformUtility.PixelAdjustRect(tooltipRectTransform, parentCanvas);
            float tooltipWidth = pixelAdjustedRect.width;
            float tooltipHeight = pixelAdjustedRect.height;

            // 根据右侧剩余空间决定左/右侧显示
            const float margin = 16f;
            bool placeOnLeft = mousePosition.x + margin + tooltipWidth > Screen.width;

            // 调整pivot，便于位置计算与限制
            tooltipRectTransform.pivot = placeOnLeft ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);

            // 期望坐标
            float desiredX = placeOnLeft ? mousePosition.x - margin : mousePosition.x + margin;
            float minX = margin + tooltipWidth * tooltipRectTransform.pivot.x;
            float maxX = Screen.width - margin - tooltipWidth * (1f - tooltipRectTransform.pivot.x);
            desiredX = Mathf.Clamp(desiredX, minX, maxX);

            float minY = margin + tooltipHeight * tooltipRectTransform.pivot.y;
            float maxY = Screen.height - margin - tooltipHeight * (1f - tooltipRectTransform.pivot.y);
            float desiredY = Mathf.Clamp(mousePosition.y, minY, maxY);

            ui.skillToolTip.transform.position = new Vector2(desiredX, desiredY);
        }
        else
        {
            // 兜底：直接放在鼠标位置
            ui.skillToolTip.transform.position = mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideSkillToolTip();
    }

    public void LoadData(GameData data)
    {
        if (data.skillTree.TryGetValue(skillName, out bool value))
            unlocked = value;

        UpdateUIState(); // 确保UI状态与数据同步
    }

    public void SaveData(ref GameData data)
    {
        if (data.skillTree.TryGetValue(skillName, out bool value))
        {
            data.skillTree.Remove(skillName);
            data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            data.skillTree.Add(skillName, unlocked);
        }
    }
}
