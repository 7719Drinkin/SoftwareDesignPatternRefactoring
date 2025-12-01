using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightBornWave : MonoBehaviour
{
    private Transform playerTf;
    private Enemy_NightBorn enemy;
    private Rigidbody2D rb;
    private IPlayerManager playerManager;
    private IAudioManager audioManager;

    [SerializeField] private ElementType elementType;
    [SerializeField] private float speed = 12f;

    void Start()
    {
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        playerTf = playerManager.Player.transform;
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
            // 如果玩家在格挡状态，wave 直接消失
            if (playerManager.Player.stateMachine.currentState == playerManager.Player.counterAttack)
            {
                SelfDestroy();
                audioManager.PlaySFX(5);
                return;
            }

            // 玩家未格挡，造成伤害
            enemy.stats.DoMagicalDamage(playerManager.Player.stats, enemy.transform, elementType);
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}
