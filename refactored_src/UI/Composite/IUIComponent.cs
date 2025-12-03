using UnityEngine;

/***************************************************************
 *  Refactored with: Composite Pattern
 *  Pattern Type: Structural
 *
 *  Document Reference:
 *  - See report section "2.2.7 Composite Pattern (Structural)"
 ***************************************************************/
/// <summary>
/// UI 组件接口 - 组合模式的组件接口
/// 定义所有 UI 元素（单个或组合）的统一操作
/// </summary>
public interface IUIComponent
{
    /// <summary>
    /// 显示 UI 组件
    /// </summary>
    void Show();

    /// <summary>
    /// 隐藏 UI 组件
    /// </summary>
    void Hide();

    /// <summary>
    /// 检查 UI 组件是否激活
    /// </summary>
    bool IsActive();
}
/***************************************************************
 * End
 ***************************************************************/

