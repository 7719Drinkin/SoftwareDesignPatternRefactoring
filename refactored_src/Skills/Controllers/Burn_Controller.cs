using System.Collections;
using UnityEngine;

public class Burn_Controller : MonoBehaviour
{
    private IAudioManager audioManager;

    private void Start()
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            IPlayerManager playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
            PlayerStats playerStats = playerManager.Player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            Enemy enemy = collision.GetComponent<Enemy>();

            StartCoroutine(ExplodeDamage(playerStats, enemyTarget));
            audioManager.PlaySFX(16);
        }
    }

    private IEnumerator ExplodeDamage(PlayerStats playerStats, EnemyStats enemyTarget)
    {
        if (playerStats == null || enemyTarget == null)
            yield break;

        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);

        yield return new WaitForSeconds(0.5f);

        if (enemyTarget == null)
            yield break;
        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);

        yield return new WaitForSeconds(0.25f);

        if (enemyTarget == null)
            yield break;
        playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);
    }

    private void DestroyMe() => Destroy(gameObject);
}
