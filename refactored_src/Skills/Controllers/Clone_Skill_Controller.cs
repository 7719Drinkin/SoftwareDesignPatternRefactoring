using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private Player player;
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLosingSpeed;
    private float cloneTimer;
    private IAudioManager audioManager;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius;
    private Transform closestEnemy;

    private int chanceToDuplicate;
    private bool canDuplicate;
    private int DuplicateOffset = 1;

    private CharacterStats cloneStats;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cloneStats = GetComponent<CharacterStats>();
    }


    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - Time.deltaTime * colorLosingSpeed);

            if (sr.color.a < 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform, float _cloneDuration, Vector3 _offset, Transform _closestEnemy, bool _canDuplicate, int _chanceToDuplicate, Player _player, bool _increaseDamage)
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        
        int comboCounter = Random.Range(1, 3);
        anim.SetInteger("AttackNumber", comboCounter);

        player = _player;
        transform.position = _newTransform.position + _offset;
        closestEnemy = _closestEnemy;
        FaceClosestTartget();

        cloneTimer = _cloneDuration;
        canDuplicate = _canDuplicate;
        chanceToDuplicate = _chanceToDuplicate;

        float increaseDamage = _increaseDamage ? 1.5f : 0.6f;
        int damage = Mathf.RoundToInt((player.GetComponent<CharacterStats>().damage.GetValue() + 5 * player.GetComponent<CharacterStats>().strength.GetValue()) * increaseDamage);
        cloneStats.damage.AddModifier(damage);

        // 继承玩家的属性
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        cloneStats.critChance.AddModifier(playerStats.critChance.GetValue());
        cloneStats.critPower.SetValue(playerStats.critPower.GetValue());
        cloneStats.intelligence.AddModifier(playerStats.intelligence.GetValue());

        switch (comboCounter)
        {
            case 0:
                audioManager.PlaySFX(1);
                break;
            case 1:
                audioManager.PlaySFX(2);
                break;
            case 2:
                audioManager.PlaySFX(3);
                break;
        }
    }

    private void AnimationTrigger()
    {
        cloneTimer = -0.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                cloneStats.DoDamage(hit.GetComponent<CharacterStats>(), transform, false);

                if (hit != null && canDuplicate)
                    if (Random.Range(0, 100) < chanceToDuplicate)
                        GameFacade.Instance.Skills.Clone.CreateClone(hit.transform, new Vector3(0.5f * DuplicateOffset, 0));

                if (GameFacade.Instance.Skills.Clone.clone)
                    if (GameFacade.Instance.Inventory.GetEquipment(EquipmentType.Weapon) && GameFacade.Instance.EquipmentUsage.CanUseWeapon())
                        GameFacade.Instance.Inventory.GetEquipment(EquipmentType.Weapon).ExecuteItemEffect(hit.transform);
            }
        }
    }

    private void FaceClosestTartget()
    {
        if (closestEnemy != null)
            if (transform.position.x > closestEnemy.position.x)
            {
                DuplicateOffset = -1;
                transform.Rotate(0, 180, 0);
            }
    }
}
