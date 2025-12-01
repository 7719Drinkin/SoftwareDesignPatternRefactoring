// ========== Composite Pattern (Leaf) ==========
// 目的：表示组合模式中的叶子节点（Tooltip 元素）
// 作用：封装 Tooltip 的显示/隐藏逻辑，适配不同的 Tooltip 类型
// ==============================================
using UnityEngine;

/// <summary>
/// Tooltip 组件 - 组合模式的叶子节点
/// 封装各种 Tooltip 的显示和隐藏操作
/// </summary>
public class TooltipComponent : IUIComponent
{
    private MonoBehaviour tooltip;

    public TooltipComponent(MonoBehaviour tooltipComponent)
    {
        tooltip = tooltipComponent;
    }

    public void Show()
    {
        // Tooltip 的显示由各自的 Show 方法处理，这里只确保对象存在
        if (tooltip != null && !tooltip.gameObject.activeSelf)
            tooltip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (tooltip == null)
            return;

        // 根据不同的 Tooltip 类型调用对应的 Hide 方法
        if (tooltip is UI_ItemToolTip itemToolTip)
            itemToolTip.HideToolTip();
        else if (tooltip is UI_StatToolTip statToolTip)
            statToolTip.HideStatToolTip();
        else if (tooltip is UI_SkillToolTip skillToolTip)
            skillToolTip.HideSkillToolTip();
        else if (tooltip is UI_CraftToolTip craftToolTip)
            craftToolTip.HideCraftToolTip();
        else
            tooltip.gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return tooltip != null && tooltip.gameObject.activeSelf;
    }
}

