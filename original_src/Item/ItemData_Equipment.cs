using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Header("Unique effect")]
    public float itemCooldown;
    public ItemEffect[] itemEffects;
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

    public void ExecuteItemEffect(Transform position)
    {
        bool effectExecuted = false;

        foreach (var item in itemEffects)
        {
            if (item.ExecuteEffect(position))
                effectExecuted = true;
        }

        // 只有在效果真正执行时才消耗对应装备的冷却时间
        if (effectExecuted)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    ServiceLocator.Instance.Get<IInventory>().ConsumeWeaponCooldown();
                    break;
                case EquipmentType.Armor:
                    // 护甲冷却在CanUseArmor中已经处理
                    break;
                case EquipmentType.Amulet:
                    // 护身符冷却在CanUseAmulet中已经处理
                    break;
                case EquipmentType.Flask:
                    // 药瓶冷却在CanUseFlask中已经处理
                    break;
            }
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
