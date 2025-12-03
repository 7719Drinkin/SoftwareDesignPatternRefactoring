using System.Collections.Generic;
using UnityEngine;

/***************************************************************
 *  Refactored with: Composite Pattern
 *  Pattern Type: Structural
 *
 *  Document Reference:
 *  - See report section "2.2.7 Composite Pattern (Structural)"
 ***************************************************************/
/// <summary>
/// UI 元素组 - 组合模式的组合节点
/// 管理多个 UI 组件，可以包含单个元素或元素组
/// </summary>
public class UIElementGroup : IUIComponent
{
    private List<IUIComponent> components = new List<IUIComponent>();

    /// <summary>
    /// 添加 UI 组件到组中
    /// </summary>
    public void Add(IUIComponent component)
    {
        if (component != null && !components.Contains(component))
            components.Add(component);
    }

    /// <summary>
    /// 从组中移除 UI 组件
    /// </summary>
    public void Remove(IUIComponent component)
    {
        components.Remove(component);
    }

    /// <summary>
    /// 显示组内所有 UI 组件
    /// </summary>
    public void Show()
    {
        foreach (var component in components)
        {
            component?.Show();
        }
    }

    /// <summary>
    /// 隐藏组内所有 UI 组件
    /// </summary>
    public void Hide()
    {
        foreach (var component in components)
        {
            component?.Hide();
        }
    }

    /// <summary>
    /// 检查组内是否有激活的 UI 组件
    /// </summary>
    public bool IsActive()
    {
        foreach (var component in components)
        {
            if (component != null && component.IsActive())
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取指定索引的组件（用于按顺序操作）
    /// </summary>
    public IUIComponent GetComponent(int index)
    {
        if (index >= 0 && index < components.Count)
            return components[index];
        return null;
    }

    /// <summary>
    /// 获取组件数量
    /// </summary>
    public int Count => components.Count;
}
/***************************************************************
 * End
 ***************************************************************/

