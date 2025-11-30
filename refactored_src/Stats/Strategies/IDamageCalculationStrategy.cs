// ========== Strategy Pattern (Strategy Interface) ==========
// 目的：定义一系列算法，并将每种算法分别放入独立的类中
// 作用：将不同的伤害计算算法封装为独立策略，使它们可以相互替换
// 优势：
// 1. 算法独立：每种伤害计算逻辑独立封装
// 2. 易于扩展：新增伤害类型只需实现新策略
// 3. 可替换：运行时可以切换不同的伤害计算策略
// 4. 符合开闭原则：对扩展开放，对修改关闭
// 
// 策略模式结构：
// - Strategy（策略接口）: IDamageCalculationStrategy
// - ConcreteStrategy（具体策略）: PhysicalDamageStrategy, MagicalDamageStrategy
// - Context（上下文）: CharacterStats
// ============================================================
using UnityEngine;

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

