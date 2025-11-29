using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;
    private IAudioManager audioManager;

    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5f;

    private Transform closestEnemy;
    [SerializeField] private LayerMask whatIsEnemy;
    private Player player;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestEnemy, Player _player)
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestEnemy = _closestEnemy;
        player = _player;
    }

    public void ChooseRandomEnemy()
    {
        float radius = ServiceLocator.Instance.Get<ISkillManager>().Blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestEnemy = colliders[Random.Range(0, colliders.Length)].transform;
    }

    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
            FinishCrystal();

        if (canMove && closestEnemy)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, closestEnemy.position) < 1)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        float explosionRadius = cd.radius * transform.localScale.x;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>(), transform);

                ItemData_Equipment equipedAmulet = ServiceLocator.Instance.Get<IInventory>().GetEquipment(EquipmentType.Amulet);

                if (equipedAmulet != null && ServiceLocator.Instance.Get<IInventory>().CanUseAmulet())
                    equipedAmulet.ExecuteItemEffect(hit.transform);
            }
        }

        audioManager.PlaySFX(13);
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;

            anim.SetTrigger("Explode");
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);
}
