using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

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

                if (Inventory.instance.GetEquipment(EquipmentType.Weapon) && Inventory.instance.CanUseWeapon())
                    Inventory.instance.GetEquipment(EquipmentType.Weapon).ExecuteItemEffect(hit.transform);
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
        AudioManager.instance.PlaySFX(33);

        SkillManager.instance.sword.CreateSword();
    }
}
