using UnityEngine;

public class Enemy_NightBornAnimationTrigger : MonoBehaviour
{
    private Enemy_NightBorn enemy => GetComponentInParent<Enemy_NightBorn>();

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target, enemy.transform, true);
            }
        }

        AudioManager.instance.PlaySFX(48);
    }

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();

    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
