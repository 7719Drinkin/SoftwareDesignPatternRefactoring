using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    // 服务依赖
    private IAudioManager audioManager;
    private IInventory inventory;

    private void Awake()
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        inventory = ServiceLocator.Instance.Get<IInventory>();
    }

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<Enemy>().isDead)
            {
                EnemyStats target = hit.GetComponent<EnemyStats>();

                if (target != null)
                {
                    player.stats.DoDamage(target, player.transform, false);

                    if (target.isShocked && Random.Range(0f, 1f) < 0.5f)
                        target.ThunderStike();
                }

                if (inventory.GetEquipment(EquipmentType.Weapon) && inventory.CanUseWeapon())
                    inventory.GetEquipment(EquipmentType.Weapon).ExecuteItemEffect(hit.transform);
            }

            if (hit.GetComponent<Chest>() != null&& !hit.GetComponent<Chest>().opened)
            {
                hit.GetComponent<Chest>().OpenChest();
            }
        }
    }

    private void CriticalAttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.criticalAttackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<Enemy>().isDead)
            {
                EnemyStats target = hit.GetComponent<EnemyStats>();

                if (target != null)
                    player.stats.DoDamage(target, player.transform, true);
            }
        }
    }

    private void ThrowSword()
    {
        audioManager.PlaySFX(33);

        ServiceLocator.Instance.Get<ISkillManager>().Sword.CreateSword();
    }
}
