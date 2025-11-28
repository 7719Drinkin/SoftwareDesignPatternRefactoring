using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage, Transform _attacker, bool _canDoDamage, bool _canCrit)
    {
        base.TakeDamage(_damage, _attacker, _canDoDamage, _canCrit);

        player.DamageEffect(_attacker, true, true);

        TakeDamageFX(_canCrit);

        player.fx.CreatePopUpText(_damage.ToString(), _canCrit);

        audioManager.PlaySFX(7);
    }

    protected override void Die()
    {
        base.Die();

        // 玩家死亡时扣除一半的金钱和经验值
        playerManager.Currency = Mathf.Max(0, playerManager.Currency / 2);
        playerManager.CurrentExperience = Mathf.Max(0, playerManager.CurrentExperience / 2);

        player.Die();
    }

    protected override void DecreaseHealthBy(int damage)
    {
        base.DecreaseHealthBy(damage);

        ItemData_Equipment currentArmor = ServiceLocator.Instance.Get<IInventory>().GetEquipment(EquipmentType.Armor);

        if (!ServiceLocator.Instance.Get<IInventory>().CanUseArmor())
            return;

        if (currentArmor != null)
            currentArmor.ExecuteItemEffect(player.transform);
    }
}
