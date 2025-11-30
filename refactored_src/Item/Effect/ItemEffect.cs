using UnityEngine;

// ========== Bridge Pattern ==========
// 目的：将抽象部分与实现部分分离，使它们可以独立变化
// 作用：装备（Abstraction）和效果（Implementor）分离，可独立扩展
// 优势：
// 1. 独立变化：装备类型和效果实现可以独立扩展
// 2. 灵活组合：任意装备可以组合任意效果
// 3. 符合开闭原则：添加新效果不需要修改装备类
// 4. 减少类爆炸：不需要为每种装备+效果组合创建新类
// 
// 桥接模式结构：
// - Abstraction（抽象）: ItemData_Equipment（装备）
// - Implementor（实现者）: ItemEffect（效果基类）
// - ConcreteImplementor: Fire_Effect, Heal_Effect, Buff_Effect 等
// - Bridge（桥接）: ItemData_Equipment.itemEffects[]
// ======================================

/// <summary>
/// 装备效果基类 - 实现者接口（Bridge Pattern - Implementor）
/// 定义装备效果的执行接口，所有具体效果继承此类
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect")]
public class ItemEffect : ScriptableObject
{
    /// <summary>
    /// 执行效果 - 实现者方法
    /// </summary>
    /// <param name="position">效果触发位置</param>
    /// <returns>是否成功执行效果</returns>
    public virtual bool ExecuteEffect(Transform position) { return false; }
}
