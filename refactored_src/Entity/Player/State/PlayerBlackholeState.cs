using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;
    private int damageBuff;

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

        player.stats.MakeInvincible(true);
        damageBuff = Mathf.RoundToInt(player.stats.damage.GetValue() * 1.2f);
        player.stats.damage.AddModifier(damageBuff);
        player.stats.critChance.AddModifier(40);
        player.stats.critPower.AddModifier(40);

        audioManager.PlaySFX(36);
    }

    public override void Exit()
    {
        base.Exit();

        player.stats.MakeInvincible(false);

        rb.gravityScale = defaultGravity;

        player.MakeTransprent(false);
        player.stats.damage.RemoveModifier(damageBuff);
        player.stats.critChance.RemoveModifier(40);
        player.stats.critPower.RemoveModifier(40);

        audioManager.PlaySFX(37);
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
                player.skill.Blackhole.CreateBlackhole();
                skillUsed = true;
            }
        }

        if (player.skill.Blackhole.SkillCompleted())
            player.stateMachine.ChangeState(player.airState);
    }
}
