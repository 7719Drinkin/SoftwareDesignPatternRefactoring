using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MaterialImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string materialName;

    private UI ui;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ui.craftToolTip != null && !string.IsNullOrEmpty(materialName))
        {
            // 鼠标的屏幕坐标
            Vector2 mousePosition = Input.mousePosition;

            // 显示提示框
            ui.craftToolTip.ShowCraftToolTip(materialName);

            // 计算提示框在屏幕内的安全位置
            RectTransform tooltipRectTransform = ui.craftToolTip.GetComponent<RectTransform>();
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

                ui.craftToolTip.transform.position = new Vector2(desiredX, desiredY);
            }
            else
            {
                // 兜底：若缺少必要组件，则直接放在鼠标位置
                ui.craftToolTip.transform.position = mousePosition;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui.craftToolTip != null)
        {
            ui.craftToolTip.HideCraftToolTip();
        }
    }

    private void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void SetMaterialName(string _materialName)
    {
        materialName = _materialName;
    }
}
