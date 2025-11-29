public class PlayerDashAttackState : PlayerState
{
    public PlayerDashAttackState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.stats.MakeInvincible(true);
        player.stats.critChance.AddModifier(100);

        player.ZeroVelocity();

        AudioManager.instance.PlaySFX(0);
    }

    public override void Exit()
    {
        base.Exit();

        player.stats.MakeInvincible(false);
        player.stats.critChance.RemoveModifier(100);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
