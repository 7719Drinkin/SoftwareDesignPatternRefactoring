using System.Collections;
using UnityEngine;

public class Burn_Controller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            Enemy enemy = collision.GetComponent<Enemy>();

            StartCoroutine(ExplodeDamage(playerStats, enemyTarget));
            AudioManager.instance.PlaySFX(16);
        }
    }

    private IEnumerator ExplodeDamage(PlayerStats playerStats, EnemyStats enemyTarget)
    {
        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);

        yield return new WaitForSeconds(0.5f);

        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);

        yield return new WaitForSeconds(0.25f);

        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);
    }

    private void DestroyMe() => Destroy(gameObject);
}
