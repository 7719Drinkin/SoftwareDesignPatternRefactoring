using UnityEngine;

public class NightBornMagicCircle : MonoBehaviour
{
    private Enemy_NightBorn enemy;
    private IAudioManager audioManager;

    private void Start()
    {
        enemy = FindObjectOfType<Enemy_NightBorn>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();

        audioManager.PlaySFX(56);
    }

    public void SelfDestroy() => Destroy(gameObject);

    public void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null)
            {
                var playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
                enemy.stats.DoMagicalDamage(playerManager.Player.stats, enemy.transform);
            }
        }
    }
}
