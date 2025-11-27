using System.Collections;
using UnityEngine;

/// <summary>
/// 实体基类 - 所有游戏实体的基础类
/// 提供通用的物理交互、碰撞检测、击退效果和翻转功能
/// 被Player和Enemy类继承，实现统一的实体行为
/// </summary>
public class Entity : MonoBehaviour
{
    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackDistance;  // 击退距离（X轴和Y轴）
    [SerializeField] protected float knockbackDuration;    // 击退持续时间
    public bool isKnocked;                                 // 是否正在被击退

    [Header("Collision info")]
    public Transform attackCheck;                           // 攻击检测点
    public float attackCheckRadius;                        // 攻击检测半径

    [SerializeField] protected Transform groundCheck;      // 地面检测点
    [SerializeField] protected float groundCheckDistance;  // 地面检测距离
    [SerializeField] protected Transform wallCheck;        // 墙壁检测点
    [SerializeField] protected float wallCheckDistance;    // 墙壁检测距离
    [SerializeField] protected LayerMask whatIsGround;     // 地面层级掩码

    public int facingDir { get; private set; } = 1;        // 面向方向（1右，-1左）
    private bool facingRight = true;                       // 是否面向右侧

    #region Components
    public Animator anim { get; private set; }              // 动画控制器
    public Rigidbody2D rb { get; private set; }            // 刚体组件
    public EntityFX fx { get; private set; }                 // 特效组件
    public SpriteRenderer sr { get; private set; }           // 精灵渲染器
    public CharacterStats stats { get; private set; }       // 角色属性
    public CapsuleCollider2D cd { get; private set; }       // 胶囊碰撞器

    #endregion

    public System.Action onFlipped;                         // 翻转事件回调

    protected virtual void Awake() { }

    /// <summary>
    /// 初始化实体组件
    /// 获取所有必要的组件引用
    /// </summary>
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

    /// <summary>
    /// 减速实体（可被子类重写）
    /// </summary>
    /// <param name="slowPercentage">减速百分比</param>
    /// <param name="slowDuration">减速持续时间</param>
    public virtual void SlowEntityBy(float slowPercentage, float slowDuration) { }

    /// <summary>
    /// 恢复默认速度
    /// </summary>
    protected virtual void ReturnDefaultSpeed() => anim.speed = 1;

    /// <summary>
    /// 伤害效果处理
    /// </summary>
    /// <param name="attackerTransform">攻击者位置</param>
    /// <param name="canFlash">是否可以闪烁</param>
    /// <param name="isKnocked">是否击退</param>
    public virtual void DamageEffect(Transform attackerTransform, bool canFlash, bool isKnocked)
    {
        if (canFlash)
            fx.StartCoroutine("FlashFX");

        StartCoroutine(HitKnockback(attackerTransform));
    }

    /// <summary>
    /// 击退协程
    /// </summary>
    /// <param name="attackerTransform">攻击者位置</param>
    /// <returns></returns>
    public virtual IEnumerator HitKnockback(Transform attackerTransform)
    {
        isKnocked = true;
        float direction = Mathf.Sign(attackerTransform.position.x - transform.position.x);
        Vector2 knockbackVelocity = new Vector2(knockbackDistance.x * -direction, knockbackDistance.y);
        rb.velocity = knockbackVelocity;

        yield return new WaitForSeconds(knockbackDuration);

        // 击退结束：如果在地面上则停止水平滑动（当摩擦力为0时有用）
        if (IsGroundDetected())
            rb.velocity = new Vector2(0, rb.velocity.y);

        isKnocked = false;
    }

    #region Velocity
    /// <summary>
    /// 将速度设为0（如果不在击退状态）
    /// </summary>
    public void ZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    /// <summary>
    /// 设置实体速度
    /// </summary>
    /// <param name="xVelocity">X轴速度</param>
    /// <param name="yVelocity">Y轴速度</param>
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(xVelocity, yVelocity);
        FlipController(xVelocity);
    }

    #endregion

    #region Collision
    /// <summary>
    /// 检测是否在地面上
    /// </summary>
    /// <returns>是否在地面上</returns>
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

    /// <summary>
    /// 检测是否碰到墙壁
    /// </summary>
    /// <returns>是否碰到墙壁</returns>
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    /// <summary>
    /// 在Scene视图中绘制调试信息
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }

    #endregion

    #region Flip
    /// <summary>
    /// 翻转实体朝向
    /// </summary>
    public virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFlipped != null)
            onFlipped();
    }

    /// <summary>
    /// 根据移动方向控制翻转
    /// </summary>
    /// <param name="_x">X轴移动方向</param>
    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    #endregion

    /// <summary>
    /// 设置实体透明度
    /// </summary>
    /// <param name="_transprent">是否透明</param>
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

    /// <summary>
    /// 实体死亡（可被子类重写）
    /// </summary>
    public virtual void Die() { }
}
