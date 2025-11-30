// ========== Composite Pattern (Leaf) ==========
// 目的：表示组合模式中的叶子节点（淡入淡出组件）
// 作用：封装淡入淡出逻辑，使其可以作为组合模式的一部分
// ==============================================
using UnityEngine;

/// <summary>
/// 淡入淡出组件 - 组合模式的叶子节点
/// 封装淡入淡出动画的显示和隐藏操作
/// </summary>
public class FadeComponent : IUIComponent
{
    private UI_FadeOut fadeOutComponent;
    private GameObject fadeGameObject;
    private bool hasFadedIn = false;

    public FadeComponent(GameObject fadeInGameObject)
    {
        fadeGameObject = fadeInGameObject;
        if (fadeGameObject != null)
            fadeOutComponent = fadeGameObject.GetComponent<UI_FadeOut>();
    }

    /// <summary>
    /// 显示（触发淡入动画，只在首次调用时生效）
    /// </summary>
    public void Show()
    {
        if (fadeOutComponent == null || hasFadedIn)
            return;

        // 确保 GameObject 是激活的
        if (fadeGameObject != null && !fadeGameObject.activeSelf)
            fadeGameObject.SetActive(true);

        fadeOutComponent?.FadeIn();
        hasFadedIn = true;
    }

    /// <summary>
    /// 隐藏（触发淡出动画并重置状态）
    /// </summary>
    public void Hide()
    {
        if (fadeOutComponent == null)
            return;

        fadeOutComponent.FadeOut();
        hasFadedIn = false;  // 重置状态，允许下次重新开始时再次淡入
    }

    /// <summary>
    /// 检查是否激活（是否已经淡入过）
    /// </summary>
    public bool IsActive()
    {
        return hasFadedIn;
    }
}

