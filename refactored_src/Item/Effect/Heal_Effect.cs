using UnityEngine;

/// <summary>
/// 治疗效果 - 具体实现者（Bridge Pattern - ConcreteImplementor）
/// 实现装备的治疗效果
/// </summary>
[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal")]
public class Heal_Effect : ItemEffect
{
    [Range(0, 1)]
    [SerializeField] private float healPercent;

    public override bool ExecuteEffect(Transform position)
    {
        PlayerStats playerStats = ServiceLocator.Instance.Get<IPlayerManager>().Player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
        playerStats.IncreaseHealthBy(healAmount);

        // 特定回复血量，播放吸血音效
        if (healPercent == 0.06f)
            ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(40);

        return true; // 治疗效果总是执行
    }
}
