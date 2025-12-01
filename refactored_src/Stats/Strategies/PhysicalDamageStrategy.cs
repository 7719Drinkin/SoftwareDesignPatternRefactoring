// ========== Strategy Pattern (ConcreteStrategy) ==========
// 目的：实现物理伤害计算策略
// 作用：封装物理伤害的计算逻辑（基础伤害+力量加成，暴击，护甲减免）
// =========================================================
using UnityEngine;

/// <summary>
/// 物理伤害计算策略 - Strategy Pattern 的具体策略
/// 计算物理伤害：基础伤害 + 力量加成，支持暴击，受护甲减免
/// </summary>
public class PhysicalDamageStrategy : IDamageCalculationStrategy
{
    public DamageResult CalculateDamage(CharacterStats attacker, CharacterStats target, Transform attackerTransform, ElementType? elementType = null)
    {
        DamageResult result = new DamageResult
        {
            CanCrit = true,  // 物理伤害可以暴击
            CanApplyElementEffect = false  // 物理伤害不应用元素效果
        };

        // 1. 计算基础伤害：基础伤害值 + 力量*5
        int baseDamage = attacker.damage.GetValue() + attacker.strength.GetValue() * 5;
        int totalDamage = baseDamage;

        // 2. 检查本次攻击是否触发暴击
        bool triggeredCrit = CheckCriticalHit(attacker);
        result.IsCritical = triggeredCrit;

        // 3. 如果暴击，计算暴击伤害
        if (triggeredCrit)
        {
            totalDamage = CalculateCriticalDamage(attacker, totalDamage);
        }

        // 4. 应用护甲减免
        totalDamage = ApplyArmorReduction(target, totalDamage);

        result.FinalDamage = totalDamage;
        return result;
    }

    /// <summary>
    /// 检查本次攻击是否触发暴击（根据暴击率随机判断）
    /// </summary>
    private bool CheckCriticalHit(CharacterStats attacker)
    {
        int totalCriticalChance = attacker.critChance.GetValue() + attacker.agility.GetValue();
        return Random.Range(0, 100) < totalCriticalChance;
    }

    /// <summary>
    /// 计算暴击伤害
    /// </summary>
    private int CalculateCriticalDamage(CharacterStats attacker, int baseDamage)
    {
        float totalCritPower = (attacker.critPower.GetValue() + attacker.strength.GetValue() * 5) * 0.01f;
        float critDamage = baseDamage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    /// <summary>
    /// 应用护甲减免
    /// </summary>
    private int ApplyArmorReduction(CharacterStats target, int damage)
    {
        int armorValue = target.armor.GetValue();

        // 冰冻状态护甲效果降低30%
        if (target.isChilled)
        {
            armorValue = Mathf.RoundToInt(armorValue * 0.7f);
        }

        damage -= armorValue;
        return Mathf.Clamp(damage, 0, int.MaxValue);
    }
}

