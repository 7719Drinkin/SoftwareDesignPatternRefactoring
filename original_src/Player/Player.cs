using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家类 - 继承自Entity
/// 实现玩家的状态机、技能系统、装备系统和输入处理
/// 包含所有玩家状态和技能逻辑
/// </summary>
public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;                        // 攻击时的移动向量数组
    public float counterAttackDuration = 0.2f;              // 反击持续时间
    public float firstAttackCheckRadius;                     // 第一次攻击检测半径
    public float secondAttackCheckRadius;                    // 第二次攻击检测半径
    public float thirdAttackCheckRadius;                     // 第三次攻击检测半径

    [Header("Dash Attack info")]
    public float criticalAttackCheckRadius;                  // 暴击攻击检测半径

    public bool isBusy { get; private set; }                 // 是否忙碌状态

    [Header("Move Info")]
    public float moveSpeed = 12f;                            // 移动速度
    public float jumpForce;                                  // 跳跃力度
    public int continousJump;                               // 连续跳跃次数
    public float swordReturnImpact;                         // 剑返回冲击力
    private float defaultMoveSpeed;                          // 默认移动速度
    private float defaultJumpForce;                          // 默认跳跃力度
    private float defaultDashSpeed;                          // 默认冲刺速度

    public SkillManager skill { get; private set; }          // 技能管理器
    public GameObject sword { get; private set; }            // 剑对象

    [SerializeField] protected Transform groundCheckBack;   // 后方地面检测点

    private PlayerItemDrop playerItemDrop;

    #region States
    public PlayerStateMachine stateMachine { get; private set; }  // 玩家状态机

    public PlayerIdleState idleState { get; private set; }        // 空闲状态
    public PlayerMoveState moveState { get; private set; }        // 移动状态
    public PlayerJumpState jumpState { get; private set; }        // 跳跃状态
    public PlayerAirState airState { get; private set; }          // 空中状态
    public PlayerWallSlideState wallSlide { get; private set; }    // 墙壁滑行状态
    public PlayerWallJumpState wallJump { get; private set; }     // 墙壁跳跃状态
    public PlayerDashState dashState { get; private set; }         // 冲刺状态
    public PlayerPrimaryAttackState primaryAttack { get; private set; }  // 主要攻击状态
    public PlayerCounterAttackState counterAttack { get; private set; }  // 反击状态
    public PlayerAimSwordState aimSword { get; private set; }     // 瞄准剑状态
    public PlayerCatchSwordState catchSword { get; private set; } // 接剑状态
    public PlayerBlackholeState blackhole { get; private set; }   // 黑洞状态
    public PlayerDeadState deadState { get; private set; }        // 死亡状态
    public PlayerDashAttackState dashAttack { get; private set; } // 冲刺攻击状态
    public PlayerAssassinateState assassinate { get; private set; } // 暗杀状态

    #endregion

    /// <summary>
    /// 初始化玩家状态机
    /// 创建所有玩家状态实例
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        // 基础移动状态
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        // 攻击状态
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        dashAttack = new PlayerDashAttackState(this, stateMachine, "DashAttack");
        assassinate = new PlayerAssassinateState(this, stateMachine, "Assassinate");

        // 特殊状态
        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackhole = new PlayerBlackholeState(this, stateMachine, "Jump");
        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    /// <summary>
    /// 初始化玩家数据和状态
    /// </summary>
    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        // 保存默认值用于减速效果恢复
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = skill.dash.dashSpeed;

        playerItemDrop = GetComponent<PlayerItemDrop>();
    }

    /// <summary>
    /// 更新玩家状态和输入检测
    /// </summary>
    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        // 检测各种输入
        CheckForDashInput();
        CheckForCrystalInput();
        CheckForFlaskInput();
        CheckForAssassinateInput();

        CheckNormalColor();
    }

    /// <summary>
    /// 检测是否在地面上（包括前后两个检测点）
    /// </summary>
    /// <returns>是否在地面上</returns>
    public override bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround) ||
                                               Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckDistance, whatIsGround);

    /// <summary>
    /// 减速玩家（重写父类方法）
    /// </summary>
    /// <param name="slowPercentage">减速百分比</param>
    /// <param name="slowDuration">减速持续时间</param>
    public override void SlowEntityBy(float slowPercentage, float slowDuration)
    {
        ReturnDefaultSpeed();
        CancelInvoke("ReturnDefaultSpeed");

        moveSpeed *= 1 - slowPercentage;
        jumpForce *= 1 - slowPercentage;
        skill.dash.dashSpeed *= 1 - slowPercentage;
        anim.speed *= 1 - slowPercentage;

        Invoke("ReturnDefaultSpeed", slowDuration);
    }

    /// <summary>
    /// 恢复默认速度（重写父类方法）
    /// </summary>
    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        anim.speed = 1;
        skill.dash.dashSpeed = defaultDashSpeed;
    }

    /// <summary>
    /// 分配新剑对象
    /// </summary>
    /// <param name="_newSword">新的剑对象</param>
    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    /// <summary>
    /// 接住剑并切换到接剑状态
    /// </summary>
    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    /// <summary>
    /// 设置忙碌状态
    /// </summary>
    /// <param name="_seconds">忙碌持续时间</param>
    /// <returns></returns>
    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    /// <summary>
    /// 动画触发器
    /// </summary>
    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    /// <summary>
    /// 检测冲刺输入
    /// </summary>
    private void CheckForDashInput()
    {

        if (stats.isDead)
            return;

        if (IsWallDetected())
            return;

        if (!skill.dash.dash)
            return;

        if (Input.GetKeyDown(KeyCode.Q) && skill.dash.CanUseSkill())
        {
            if (stateMachine.currentState == blackhole)
                return;

            skill.dash.dashDir = Input.GetAxisRaw("Horizontal");

            if (skill.dash.dashDir == 0)
                skill.dash.dashDir = facingDir;

            // 如果装备了护身符且可以使用，则延迟使用
            if (Inventory.instance.dashUseAmulet)
                if (Inventory.instance.CanUseAmulet())
                    StartCoroutine(DelayUseAmulet());

            stateMachine.ChangeState(dashState);
        }
    }

    /// <summary>
    /// 延迟使用护身符
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayUseAmulet()
    {
        yield return new WaitForSeconds(0.125f);

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        if (equipedAmulet != null)
            equipedAmulet.ExecuteItemEffect(transform);
    }

    /// <summary>
    /// 检测水晶技能输入
    /// </summary>
    private void CheckForCrystalInput()
    {
        if (Input.GetKeyDown(KeyCode.C) && SkillManager.instance.crystal.crystal)
            skill.crystal.CanUseSkill();
    }

    /// <summary>
    /// 检测药水输入
    /// </summary>
    private void CheckForFlaskInput()
    {
        if (Input.GetKeyDown(KeyCode.H) && Inventory.instance.CanUseFlask())
        {
            ItemData_Equipment currentFlask = Inventory.instance.GetEquipment(EquipmentType.Flask);

            if (currentFlask != null)
                currentFlask.ExecuteItemEffect(transform);
        }
    }

    /// <summary>
    /// 检测暗杀技能输入
    /// </summary>
    private void CheckForAssassinateInput()
    {
        if (stateMachine.currentState == blackhole)
            return;

        if (Input.GetKeyDown(KeyCode.X) && SkillManager.instance.assassinate.assassinate)
        {
            if (!SkillManager.instance.assassinate.CanUseSkill())
                return;

            stateMachine.ChangeState(assassinate);
        }
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);

        playerItemDrop.GenerateDrop();
    }

    public void CheckNormalColor()
    {
        // 若已被强制设为透明（如黑洞技能期间），不要覆盖为白色
        if (sr != null && sr.color.a == 0f)
            return;

        bool isChillInvoking = fx != null && fx.IsInvoking("ChillColorFx");
        bool isShockInvoking = fx != null && fx.IsInvoking("ShockColorFx");
        bool isIgniteInvoking = fx != null && fx.IsInvoking("IgniteColorFx");

        // 如果正在执行任何颜色效果，直接返回，不改变颜色
        if (isChillInvoking || isShockInvoking || isIgniteInvoking)
        {
            return;
        }

        sr.color = Color.white;
    }
}

