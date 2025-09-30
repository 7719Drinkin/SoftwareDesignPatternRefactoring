using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage, Transform _attacker, bool _canDoDamage)
    {
        base.TakeDamage(_damage, _attacker, _canDoDamage);

        player.DamageEffect(_attacker, true, true);

        AudioManager.instance.PlaySFX(7);
    }

    protected override void Die()
    {
        base.Die();

        // 玩家死亡时扣除一半的金钱和经验值
        PlayerManager.instance.currency = Mathf.Max(0, PlayerManager.instance.currency / 2);
        PlayerManager.instance.currentExperience = Mathf.Max(0, PlayerManager.instance.currentExperience / 2);

        player.Die();
    }

    protected override void DecreaseHealthBy(int damage)
    {
        base.DecreaseHealthBy(damage);

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (!Inventory.instance.CanUseArmor())
            return;

        if (currentArmor != null)
            currentArmor.ExecuteItemEffect(player.transform);
    }
}
