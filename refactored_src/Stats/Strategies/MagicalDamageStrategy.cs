// ========== Strategy Pattern (ConcreteStrategy) ==========
// 目的：实现魔法伤害计算策略
// 作用：封装魔法伤害的计算逻辑（元素伤害+智力加成，魔法抗性减免，元素效果应用）
// =========================================================
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 魔法伤害计算策略 - Strategy Pattern 的具体策略
/// 计算魔法伤害：元素伤害 + 智力加成，受魔法抗性减免，可以应用元素效果
/// </summary>
public class MagicalDamageStrategy : IDamageCalculationStrategy
{
    public DamageResult CalculateDamage(CharacterStats attacker, CharacterStats target, Transform attackerTransform, ElementType? elementType = null)
    {
        // 使用传入的元素类型，如果没有指定则使用 Auto
        ElementType specifiedElement = elementType ?? ElementType.Auto;

        DamageResult result = new DamageResult
        {
            CanCrit = false,  // 魔法伤害不能暴击
            CanApplyElementEffect = true  // 魔法伤害可以应用元素效果
        };

        // 1. 获取各元素伤害值
        int fireDamage = attacker.fireDamage.GetValue();
        int iceDamage = attacker.iceDamage.GetValue();
        int lightningDamage = attacker.lightningDamage.GetValue();

        // 2. 计算总魔法伤害：所有元素伤害之和 + 智力*5
        int totalMagicalDamage = fireDamage + iceDamage + lightningDamage + 5 * attacker.intelligence.GetValue();

        // 3. 应用魔法抗性减免
        totalMagicalDamage -= target.magicResistance.GetValue() + target.intelligence.GetValue() * 3;
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);

        result.FinalDamage = totalMagicalDamage;

        // 4. 确定要应用的元素效果
        DetermineElementEffect(result, fireDamage, iceDamage, lightningDamage, specifiedElement);

        return result;
    }

    /// <summary>
    /// 确定要应用的元素效果
    /// </summary>
    private void DetermineElementEffect(DamageResult result, int fireDamage, int iceDamage, int lightningDamage, ElementType specifiedElement)
    {
        if (specifiedElement == ElementType.Auto)
        {
            // 自动选择：根据最高伤害值选择元素效果
            int maxDamage = Mathf.Max(fireDamage, iceDamage, lightningDamage);
            if (maxDamage <= 0)
            {
                result.CanApplyElementEffect = false;
                return;
            }

            List<int> maxElements = new List<int>();
            if (fireDamage == maxDamage) maxElements.Add(0);
            if (iceDamage == maxDamage) maxElements.Add(1);
            if (lightningDamage == maxDamage) maxElements.Add(2);

            int selectedElement = maxElements[Random.Range(0, maxElements.Count)];

            if (selectedElement == 0)
                result.ElementType = ElementType.Fire;
            else if (selectedElement == 1)
                result.ElementType = ElementType.Ice;
            else
                result.ElementType = ElementType.Lightning;
        }
        else
        {
            // 指定元素：使用指定的元素类型
            result.ElementType = specifiedElement;
        }
    }
}

