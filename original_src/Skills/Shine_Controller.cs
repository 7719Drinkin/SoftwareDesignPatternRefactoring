using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shine_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
            EnemyStats enemyTarget = collision.GetComponent<EnemyStats>();
            Enemy enemy = collision.GetComponent<Enemy>();

            playerStats.DoMagicalDamage(enemyTarget, transform, ElementType.Fire);

            anim.SetTrigger("Hit");
            rb.velocity = Vector3.zero;

            AudioManager.instance.PlaySFX(17);
        }
    }

    public void Setup(Vector2 velocity)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        rb.velocity = velocity;

        Invoke("DestroyMe", 6);
    }

    private void DestroyMe() => Destroy(gameObject);
}
