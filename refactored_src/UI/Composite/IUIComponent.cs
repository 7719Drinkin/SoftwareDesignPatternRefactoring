// ========== Composite Pattern (Component) ==========
// 目的：将对象组合成树形结构以表示"部分-整体"的层次结构
// 作用：统一管理 UI 元素（单个元素或元素组），提供一致的显示/隐藏接口
// 优势：
// 1. 统一接口：单个 UI 元素和 UI 元素组使用相同的接口
// 2. 递归组合：可以组合多个 UI 元素组，形成树形结构
// 3. 简化操作：可以一次性操作整个 UI 元素组
// 4. 易于扩展：新增 UI 元素类型只需实现接口
// 
// 组合模式结构：
// - Component（组件接口）: IUIComponent
// - Leaf（叶子节点）: UIElementComponent（单个 UI 元素）
// - Composite（组合节点）: UIElementGroup（UI 元素组）
// ====================================================
using UnityEngine;

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

