using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    [SerializeField] private float returnSpeed = 12f;
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;

    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;
    private int bounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;
    private float spinDirection;

    private bool canFreezeEnemy;
    private bool canUseAmulet;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, bool _canFreezeEnemy, float _freezeTimeDuration, bool _canUseAmulet)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeTimeDuration = _freezeTimeDuration;
        canFreezeEnemy = _canFreezeEnemy;
        canUseAmulet = _canUseAmulet;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 6);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

		if (isReturning)
        {
			if (player != null)
			{
				float speedMultiplier = canRotate ? 2.5f : 1f;
				transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime * speedMultiplier);

				if (Vector2.Distance(transform.position, player.transform.position) < 1)
					player.CatchTheSword();
			}
        }

        BounceLogic();

        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
                StopWhenSpinning();

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.25f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }
            }

            hitTimer -= Time.deltaTime;

            if (hitTimer < 0)
                hitTimer = hitCooldown;
        }
    }

    private void StopWhenSpinning()
    {
        if (!wasStopped)
        {
            wasStopped = true;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            spinTimer = spinDuration;
        }
    }

	private void BounceLogic()
    {
		if (isBouncing && enemyTarget != null && enemyTarget.Count > 0 && !isReturning)
        {
			// 清理已被销毁的目标
			enemyTarget.RemoveAll(t => t == null);
			if (enemyTarget.Count == 0)
			{
				isBouncing = false;
				isReturning = true;
				return;
			}

			if (targetIndex < 0 || targetIndex >= enemyTarget.Count)
				targetIndex = 0;

			var target = enemyTarget[targetIndex];
			if (target == null)
			{
				// 目标刚好被销毁，尝试下一个
				targetIndex = (targetIndex + 1) % enemyTarget.Count;
				return;
			}

			transform.position = Vector2.MoveTowards(transform.position, target.position, bounceSpeed * Time.deltaTime);

			if (Vector2.Distance(transform.position, target.position) < 0.1f)
			{
				targetIndex = (targetIndex + 1) % enemyTarget.Count;
				bounceAmount--;

				if (bounceAmount <= 0 || enemyTarget.Count == 0)
				{
					isBouncing = false;
					isReturning = true;
				}
			}
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;

        bool hitEnemy = false;

        if (collision.GetComponent<Enemy>() != null)
        {
            SetupTargetsForBounce();

            if (pierceAmount > 0)
                player.stats.DoDamage(collision.GetComponent<CharacterStats>(), transform, true);
            else
                player.stats.DoDamage(collision.GetComponent<CharacterStats>(), transform, false);

            if (canFreezeEnemy)
            {
                collision.GetComponent<Enemy>().FreezeTimeFor(freezeTimeDuration);

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                if (equipedAmulet != null && canUseAmulet)
                    if (Inventory.instance.CanUseAmulet())
                        equipedAmulet.ExecuteItemEffect(collision.GetComponent<Enemy>().transform);
            }

            hitEnemy = true;
        }

        StuckInto(collision, hitEnemy);
    }

    private void SetupTargetsForBounce()
    {
		if (isBouncing && enemyTarget.Count <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

			foreach (var hit in colliders)
				if (hit.GetComponent<Enemy>() != null && hit.transform != null && hit.transform != transform)
					enemyTarget.Add(hit.transform);
        }
    }

    private void StuckInto(Collider2D collision, bool hitEnemy)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        if (!hitEnemy)
            AudioManager.instance.PlaySFX(42);

        canRotate = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
            return;

        cd.enabled = false;
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;
    }
}
