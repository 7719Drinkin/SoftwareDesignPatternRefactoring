using UnityEngine;

/// <summary>
/// Buff效果 - 具体实现者（Bridge Pattern - ConcreteImplementor）
/// 实现装备的属性增益效果
/// </summary>

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critPower,
    maxHealth,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    lightningDamage,
    level
}

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item effect/Buff Effect")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType bufftype;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override bool ExecuteEffect(Transform position)
    {
        stats = ServiceLocator.Instance.Get<IPlayerManager>().Player.GetComponent<PlayerStats>();
        stats.IncreaseStatBy(buffAmount, buffDuration, StatToModify());
        
        return true; // Buff效果总是执行
    }

    private Stat StatToModify()
    {
        switch (bufftype)
        {
            case StatType.strength:
                return stats.strength;
            case StatType.agility:
                return stats.agility;
            case StatType.intelligence:
                return stats.intelligence;
            case StatType.vitality:
                return stats.vitality;
            case StatType.critChance:
                return stats.critChance;
            case StatType.evasion:
                return stats.evasion;
            case StatType.damage:
                return stats.damage;
        }

        return null;
    }
}
