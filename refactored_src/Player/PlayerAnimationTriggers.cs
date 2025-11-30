using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    // ========== 使用 Facade Pattern 简化服务访问 ==========
    // Before: audioManager = ServiceLocator.Instance.Get<IAudioManager>();
    // After:  GameFacade.Instance.Audio
    
    private GameFacade game => GameFacade.Instance;

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

                if (game.Inventory.GetEquipment(EquipmentType.Weapon) && game.Inventory.CanUseWeapon())
                    game.Inventory.GetEquipment(EquipmentType.Weapon).ExecuteItemEffect(hit.transform);
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
        game.PlaySFX(33);
        game.Skills.Sword.CreateSword();
    }
}
