using UnityEngine;

/***************************************************************
 *  Refactored with: Strategy Pattern
 *  Pattern Type: Behavioral
 *
 *  Document Reference:
 *  - See report section "2.2.8 Strategy Pattern (Behavioral)"
 ***************************************************************/
/// <summary>
/// 伤害计算策略接口 - Strategy Pattern
/// 定义所有伤害计算策略必须实现的方法
/// </summary>
public interface IDamageCalculationStrategy
{
    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="attacker">攻击者属性</param>
    /// <param name="target">目标属性</param>
    /// <param name="attackerTransform">攻击者Transform</param>
    /// <param name="elementType">元素类型（可选，仅魔法伤害使用）</param>
    /// <returns>伤害计算结果</returns>
    DamageResult CalculateDamage(CharacterStats attacker, CharacterStats target, Transform attackerTransform, ElementType? elementType = null);
}

/// <summary>
/// 伤害计算结果
/// </summary>
public class DamageResult
{
    public int FinalDamage { get; set; }        // 最终伤害值
    public bool CanCrit { get; set; }            // 伤害类型是否支持暴击（特性：物理伤害可以，魔法伤害不可以）
    public bool IsCritical { get; set; }        // 本次伤害是否实际触发了暴击（结果：true=暴击了，false=没暴击）
    public bool CanApplyElementEffect { get; set; } // 是否可以应用元素效果
    public ElementType? ElementType { get; set; }  // 元素类型（如果适用）
}
/***************************************************************
 * End
 ***************************************************************/

