using System.Collections;
using UnityEngine;

public class Tornado_Controller : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            Enemy enemy = collision.GetComponent<Enemy>();

            playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Ice);

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            StartCoroutine(EnemyKnocked(enemy, rb, 0.5f));
        }
    }

    private IEnumerator EnemyKnocked(Enemy enemy, Rigidbody2D rb, float delay)
    {
        enemy.isKnocked = true;
        rb.velocity = new Vector2(5 * SkillManager.instance.dash.dashDir, 75);

        yield return new WaitForSeconds(delay);

        enemy.isKnocked = false;
    }

    private void DestroyMe() => Destroy(gameObject);
}
