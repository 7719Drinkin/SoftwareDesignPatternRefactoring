using System.Collections;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.2f;
    public float firstAttackCheckRadius;
    public float secondAttackCheckRadius;
    public float thirdAttackCheckRadius;

    [Header("Dash Attack info")]
    public ItemEffect[] dashAttackEffects;
    public float criticalAttackCheckRadius;

    [Header("Assassinate info")]
    public ItemEffect[] assassinateEffects;

    public bool isBusy { get; private set; }

    [Header("Move Info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public int continousJump;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeed;

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    [SerializeField] protected Transform groundCheckBack;

    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }

    public PlayerMoveState moveState { get; private set; }

    public PlayerJumpState jumpState { get; private set; }

    public PlayerAirState airState { get; private set; }

    public PlayerWallSlideState wallSlide { get; private set; }

    public PlayerWallJumpState wallJump { get; private set; }

    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }

    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSword { get; private set; }

    public PlayerCatchSwordState catchSword { get; private set; }

    public PlayerBlackholeState blackhole { get; private set; }

    public PlayerDeadState deadState { get; private set; }

    public PlayerDashAttackState dashAttack { get; private set; }

    public PlayerAssassinateState assassinate { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        dashAttack = new PlayerDashAttackState(this, stateMachine, "DashAttack");
        assassinate = new PlayerAssassinateState(this, stateMachine, "Assassinate");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");

        blackhole = new PlayerBlackholeState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = skill.dash.dashSpeed;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();
        CheckForCrystalInput();
        CheckForFlaskInput();
        CheckForAssassinateInput();
    }

    public override bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround) ||
                                               Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckDistance, whatIsGround);

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

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        skill.dash.dashSpeed = defaultDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    private void CheckForDashInput()
    {
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

            if (Inventory.instance.dashUseAmulet)
                if (Inventory.instance.CanUseAmulet())
                    StartCoroutine(DelayUseAmulet());

            stateMachine.ChangeState(dashState);
        }
    }

    private IEnumerator DelayUseAmulet()
    {
        yield return new WaitForSeconds(0.125f);

        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

        if (equipedAmulet != null)
            equipedAmulet.ExecuteItemEffect(transform);
    }

    private void CheckForCrystalInput()
    {
        if (Input.GetKeyDown(KeyCode.C) && SkillManager.instance.crystal.crystal)
            skill.crystal.CanUseSkill();
    }

    private void CheckForFlaskInput()
    {
        if (Input.GetKeyDown(KeyCode.H) && Inventory.instance.CanUseFlask())
        {
            ItemData_Equipment currentFlask = Inventory.instance.GetEquipment(EquipmentType.Flask);

            if (currentFlask != null)
                currentFlask.ExecuteItemEffect(transform);
        }
    }

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

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}

