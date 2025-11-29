using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    // ========== 服务依赖（所有子类共享） ==========
    protected IAudioManager audioManager;
    protected IInventory inventory;
    protected ISaveManagerService saveManager;

    protected float xInput;
    protected float yInput;
    private string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
        triggerCalled = false;

        // 初始化服务（延迟初始化，按需获取）
        if (audioManager == null)
            audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        if (inventory == null)
            inventory = ServiceLocator.Instance.Get<IInventory>();
        if (saveManager == null)
            saveManager = ServiceLocator.Instance.Get<ISaveManagerService>();
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
