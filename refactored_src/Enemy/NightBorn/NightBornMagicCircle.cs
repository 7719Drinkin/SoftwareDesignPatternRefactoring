using UnityEngine;

public class NightBornMagicCircle : MonoBehaviour
{
    private Enemy_NightBorn enemy;

    private void Start()
    {
        enemy = FindObjectOfType<Enemy_NightBorn>();

        AudioManager.instance.PlaySFX(56);
    }

    public void SelfDestroy() => Destroy(gameObject);

    public void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                enemy.stats.DoMagicalDamage(PlayerManager.instance.player.stats, enemy.transform);
            }
        }
    }
}
