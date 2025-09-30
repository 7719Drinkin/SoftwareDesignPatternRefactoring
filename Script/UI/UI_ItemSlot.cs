using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;

    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
                itemText.text = item.stackSize.ToString();
            else
                itemText.text = "";
        }
    }

    public void ClearUpSlot()
    {
        item = null;

        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (itemImage.sprite == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        AudioManager.instance.PlaySFX(24);

        if (item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);

        ui.itemToolTip.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 若槽位为空或不是装备类型，直接返回
        if (itemImage.sprite == null || item.data.itemType != ItemType.Equipment)
            return;

        // 鼠标的屏幕坐标
        Vector2 mousePosition = Input.mousePosition;

        // 先显示提示框以便我们可以读取其尺寸
        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);

        // 计算提示框在屏幕内的安全位置
        RectTransform tooltipRectTransform = ui.itemToolTip.GetComponent<RectTransform>();
        Canvas parentCanvas = ui.GetComponentInParent<Canvas>();

        if (tooltipRectTransform != null && parentCanvas != null)
        {
            // 获取提示框像素尺寸（考虑Canvas缩放）
            Rect pixelAdjustedRect = RectTransformUtility.PixelAdjustRect(tooltipRectTransform, parentCanvas);
            float tooltipWidth = pixelAdjustedRect.width;
            float tooltipHeight = pixelAdjustedRect.height;

            // 根据右侧空间决定显示在鼠标左/右侧
            const float margin = 16f; // 与鼠标的间距
            bool placeOnLeft = mousePosition.x + margin + tooltipWidth > Screen.width;

            // 调整pivot，便于后续位置计算与限制
            tooltipRectTransform.pivot = placeOnLeft ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f);

            // 计算期望的X坐标（加上/减去边距）
            float desiredX = placeOnLeft ? mousePosition.x - margin : mousePosition.x + margin;

            // 限制Y坐标，确保提示框完全在屏幕内
            float minY = margin + tooltipHeight * tooltipRectTransform.pivot.y;
            float maxY = Screen.height - margin - tooltipHeight * (1f - tooltipRectTransform.pivot.y);
            float desiredY = Mathf.Clamp(mousePosition.y, minY, maxY);

            // 限制X坐标，确保提示框完全在屏幕内
            float minX = margin + tooltipWidth * tooltipRectTransform.pivot.x;
            float maxX = Screen.width - margin - tooltipWidth * (1f - tooltipRectTransform.pivot.x);
            desiredX = Mathf.Clamp(desiredX, minX, maxX);

            ui.itemToolTip.transform.position = new Vector2(desiredX, desiredY);
        }
        else
        {
            // 兜底：若缺少必要组件，则直接放在鼠标位置
            ui.itemToolTip.transform.position = mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemImage.sprite == null || item.data.itemType != ItemType.Equipment)
            return;

        ui.itemToolTip.HideToolTip();
    }
}
