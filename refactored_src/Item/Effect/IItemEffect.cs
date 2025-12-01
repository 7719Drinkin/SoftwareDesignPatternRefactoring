using UnityEngine;

/***************************************************************
 *  Refactored with: Bridge Pattern
 *  Pattern Type: Structural
 *
 *  Document Reference:
 *  - See report section "2.2.6 Bridge Pattern (Structural)"
 ***************************************************************/
/// <summary>
/// 装备效果接口 - 实现者接口（Bridge Pattern - Implementor）
/// 定义装备效果的执行接口，符合官方桥接模式定义
/// 说明：根据官方桥接模式，Implementor 应该是接口（IImplementation）。
/// 由于 Unity ScriptableObject 需要序列化支持，ItemEffect 抽象基类实现此接口。
/// </summary>
public interface IItemEffect
{
    /// <summary>
    /// 执行效果 - 实现者方法
    /// </summary>
    /// <param name="position">效果触发位置</param>
    /// <returns>是否成功执行效果</returns>
    bool ExecuteEffect(Transform position);
}
/***************************************************************
 * End
 ***************************************************************/

