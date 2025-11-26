using UnityEngine;

[CreateAssetMenu(fileName = "Heal effect", menuName = "Data/Item effect/Heal")]
public class Heal_Effect : ItemEffect
{
    [Range(0, 1)]
    [SerializeField] private float healPercent;

    public override bool ExecuteEffect(Transform position)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValue() * healPercent);
        playerStats.IncreaseHealthBy(healAmount);

        // 特定回复血量，播放吸血音效
        if (healPercent == 0.06f)
            AudioManager.instance.PlaySFX(40);

        return true; // 治疗效果总是执行
    }
}
