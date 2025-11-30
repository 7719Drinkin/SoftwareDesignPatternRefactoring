// ========== Composite Pattern (Leaf) ==========
// 目的：表示组合模式中的叶子节点（单个 UI 元素）
// 作用：封装单个 UI 元素的显示/隐藏逻辑
// ==============================================
using UnityEngine;

/// <summary>
/// UI 元素组件 - 组合模式的叶子节点
/// 封装单个 UI 元素的显示和隐藏操作
/// </summary>
public class UIElementComponent : IUIComponent
{
    private GameObject uiElement;

    public UIElementComponent(GameObject element)
    {
        uiElement = element;
    }

    public void Show()
    {
        if (uiElement != null)
            uiElement.SetActive(true);
    }

    public void Hide()
    {
        if (uiElement != null)
            uiElement.SetActive(false);
    }

    public bool IsActive()
    {
        return uiElement != null && uiElement.activeSelf;
    }
}

