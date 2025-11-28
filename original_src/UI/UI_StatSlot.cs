using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StatSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string statName;
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string statDescription;

    private UI ui;
    private IPlayerManager playerManager;

    private void OnValidate()
    {
        gameObject.name = "stat - " + statName;

        if (statNameText != null)
            statNameText.text = statName;
    }

    void Start()
    {
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        UpdateStatValueUI();

        ui = GetComponentInParent<UI>();
        
        // 订阅PlayerManager数据加载完成事件
        if (playerManager != null)
            playerManager.OnPlayerDataLoaded += UpdateStatValueUI;
    }
    
    private void OnDestroy()
    {
        // 取消订阅事件
        if (playerManager != null)
            playerManager.OnPlayerDataLoaded -= UpdateStatValueUI;
    }

    public void UpdateStatValueUI()
    {
        if (playerManager == null)
            playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        PlayerStats playerStats = playerManager.Player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            statValueText.text = playerStats.StatOfType(statType).GetValue().ToString();

            if (statType == StatType.maxHealth)
                statValueText.text = playerStats.GetMaxHealthValue().ToString();

            if (statType == StatType.damage)
                statValueText.text = (playerStats.damage.GetValue() + playerStats.strength.GetValue() * 5).ToString();

            if (statType == StatType.critPower)
                statValueText.text = (playerStats.critPower.GetValue() + playerStats.strength.GetValue()).ToString();

            if (statType == StatType.critChance)
                statValueText.text = (playerStats.critChance.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.evasion)
                statValueText.text = (playerStats.evasion.GetValue() + playerStats.agility.GetValue()).ToString();

            if (statType == StatType.magicResistance)
                statValueText.text = (playerStats.magicResistance.GetValue() + playerStats.intelligence.GetValue() * 3).ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 先显示提示框，便于获取其RectTransform尺寸
        ui.statToolTip.ShowStatToolTip(statDescription);

        // 鼠标屏幕坐标
        Vector2 mousePosition = Input.mousePosition;

        // 计算屏幕安全位置（不越界）
        RectTransform tooltipRectTransform = ui.statToolTip.GetComponent<RectTransform>();
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

            ui.statToolTip.transform.position = new Vector2(desiredX, desiredY);
        }
        else
        {
            // 兜底：直接放在鼠标位置
            ui.statToolTip.transform.position = mousePosition;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statToolTip.HideStatToolTip();
    }
}
