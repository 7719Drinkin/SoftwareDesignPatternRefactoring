public class PlayerBlackholeState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;

        AudioManager.instance.PlaySFX(36);
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = defaultGravity;
        player.MakeTransprent(false);

        AudioManager.instance.PlaySFX(37);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            player.SetVelocity(0, 15);
        else
        {
            player.SetVelocity(0, -0.1f);

            if (!skillUsed)
            {
                player.skill.blackhole.CreateBlackhole();
                skillUsed = true;
            }
        }

        if (player.skill.blackhole.SkillCompleted())
            player.stateMachine.ChangeState(player.airState);
    }
}
