using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBornWave : MonoBehaviour
{
    private Transform playerTf;
    private Enemy_NightBorn enemy;
    private Rigidbody2D rb;

    [SerializeField] private ElementType elementType;
    [SerializeField] private float speed = 12f;

    void Start()
    {
        playerTf = PlayerManager.instance.player.transform;
        enemy = FindObjectOfType<Enemy_NightBorn>();
        rb = GetComponent<Rigidbody2D>();
        
        // 计算朝向玩家的方向并设置速度
        Vector2 direction = (playerTf.position - transform.position).normalized;
        transform.right = direction;
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (PlayerManager.instance.player.stateMachine.currentState != PlayerManager.instance.player.counterAttack)
                enemy.stats.DoMagicalDamage(PlayerManager.instance.player.stats, enemy.transform, elementType);

            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}
