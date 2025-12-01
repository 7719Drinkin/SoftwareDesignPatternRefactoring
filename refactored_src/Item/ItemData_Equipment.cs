using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

/***************************************************************
 *  Refactored with: Bridge Pattern
 *  Pattern Type: Structural
 *
 *  Document Reference:
 *  - See report section "2.2.6 Bridge Pattern (Structural)"
 ***************************************************************/
/// <summary>
/// 装备数据 - 桥接模式的抽象部分（Bridge Pattern - Abstraction）
/// 通过组合 IItemEffect[] 实现效果，而不是继承
/// 这样装备和效果可以独立变化和扩展
/// 
/// 说明：根据官方桥接模式定义，Abstraction 可以是具体类。
/// 当前实现符合桥接模式的核心结构：
/// - Abstraction: ItemData_Equipment（具体类）
/// - Implementor: IItemEffect（接口）
/// - 桥接关系：通过 itemEffects[] 组合效果
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    
    // ========== Bridge：桥接到效果实现 ==========
    // 注意：由于 Unity ScriptableObject 序列化限制，使用 ScriptableObject[] 数组
    // 在运行时通过 IItemEffect 接口使用，符合桥接模式定义
    public ScriptableObject[] itemEffects;  // 桥接：装备持有效果引用（运行时转换为 IItemEffect 接口）
    [TextArea]
    public string itemEffectDescription;

    [Header("Major stats")]
    public int strength; // 1 point increases damage by 1 and crit.power by 1%
    public int agility; // 1 point increases evasion by 1% and crit.chance by 1%
    public int intelligence; // 1 point increases magic damage by 1 and magic resistance by 3
    public int vitality; // 1 point increases health by 5 points

    [Header("Offensive stats")]
    public int damage;
    public int critChance;
    public int critPower;

    [Header("Defensive stats")]
    public int maxHealth;
    public int armor;
    public int evasion;
    public int magicResistence;

    [Header("Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int lightningDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftingMaterials;

    /// <summary>
    /// 执行装备效果 - 桥接方法（Bridge Pattern）
    /// 通过桥接调用效果实现，而不是在装备类中硬编码效果逻辑
    /// </summary>
    public void ExecuteItemEffect(Transform position)
    {
        bool effectExecuted = false;

        // ========== Bridge Pattern：通过桥接调用效果实现 ==========
        // 将 ScriptableObject 转换为 IItemEffect 接口使用，符合桥接模式定义
        foreach (var item in itemEffects)
        {
            if (item is IItemEffect effect && effect.ExecuteEffect(position))
                effectExecuted = true;
        }

        // 只有在效果真正执行时才消耗武器的冷却时间
        if (effectExecuted && equipmentType == EquipmentType.Weapon )
        {
            GameFacade.Instance.EquipmentUsage.ConsumeWeaponCooldown();
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = ServiceLocator.Instance.Get<IPlayerManager>().Player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistence);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightningDamage.AddModifier(lightningDamage);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = ServiceLocator.Instance.Get<IPlayerManager>().Player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critPower.RemoveModifier(critPower);

        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistence);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.lightningDamage.RemoveModifier(lightningDamage);
    }

    public override string GetDescription()
    {
        builder.Length = 0;
        descriptionLength = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智慧");
        AddItemDescription(vitality, "活力");
        AddItemDescription(damage, "伤害");
        AddItemDescription(critChance, "暴击率");
        AddItemDescription(critPower, "暴击伤害");
        AddItemDescription(fireDamage, "火焰伤害");
        AddItemDescription(iceDamage, "冰霜伤害");
        AddItemDescription(lightningDamage, "雷电伤害");
        AddItemDescription(maxHealth, "最大生命值");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicResistence, "魔法抗性");
        AddItemDescription(evasion, "闪避率");
       
        builder.AppendLine();
        builder.Append("");

        if (!string.IsNullOrEmpty(itemEffectDescription))
        {
            builder.AppendLine();
            builder.Append(itemEffectDescription);
        }

        return builder.ToString();
    }

    private void AddItemDescription(int value, string name)
    {
        if (value != 0)
        {
            if (builder.Length > 0)
                builder.AppendLine();

            builder.Append("+ " + value.ToString().PadRight(3) + "  " + name);

            descriptionLength++;
        }
    }
}
/***************************************************************
 * End
 ***************************************************************/
