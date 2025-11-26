using UnityEngine;

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
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
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
