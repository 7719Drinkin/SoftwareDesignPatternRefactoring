using UnityEngine;

public class IceAndFire_Controller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            IPlayerManager playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
            PlayerStats playerStats = playerManager.Player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            playerStats.DoMagicalDamage(enemyTarget, transform);
        }
    }
}
