using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDistance;
    [SerializeField] protected float knockbackDuration;
    public bool isKnocked;

    [Header("Collision info")]
    public Transform attackCheck;
    public float attackCheckRadius;

    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int facingDir { get; private set; } = 1;
    private bool facingRight = true;

    #region Components
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }

    #endregion

    public System.Action onFlipped;

    protected virtual void Awake() { }

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        stats = GetComponentInChildren<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update() { }

    public virtual void SlowEntityBy(float slowPercentage, float slowDuration) { }

    protected virtual void ReturnDefaultSpeed() => anim.speed = 1;

    public virtual void DamageEffect(Transform attackerTransform, bool canFlash, bool isKnocked)
    {
        if (canFlash)
            fx.StartCoroutine("FlashFX");

        StartCoroutine(HitKnockback(attackerTransform));
    }

    public virtual IEnumerator HitKnockback(Transform attackerTransform)
    {
        isKnocked = true;
        float direction = Mathf.Sign(attackerTransform.position.x - transform.position.x);
        Vector2 knockbackVelocity = new Vector2(knockbackDistance.x * -direction, knockbackDistance.y);
        rb.velocity = knockbackVelocity;

        yield return new WaitForSeconds(knockbackDuration);

        // End of knockback: stop horizontal sliding if grounded (useful when friction is 0)
        if (IsGroundDetected())
            rb.velocity = new Vector2(0, rb.velocity.y);

        isKnocked = false;
    }

    #region Velocity
    public void ZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }

    #endregion

    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFlipped != null)
            onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    #endregion

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            Color transparentColor = sr.color;
            transparentColor.a = 0;
            sr.color = transparentColor;
        }
        else
        {
            Color opaqueColor = sr.color;
            opaqueColor.a = 1f;
            sr.color = opaqueColor;
        }
    }

    public virtual void Die() { }
}
