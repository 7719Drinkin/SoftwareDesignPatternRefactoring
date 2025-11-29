using System;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private bool canCreateClone;

    private float afterImageTimer;
    private float afterImageCooldown = 0.03f;

    public event Action OnCloneOnDashUsed;

    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        player.stats.MakeInvincible(true);

        if (player.skill.clone.CanUseSkill() && SkillManager.instance.dash.cloneOnDash)
        {
            canCreateClone = true;
            player.skill.clone.CreateCloneOnDashStart();
        }

        stateTimer = player.skill.dash.dashDuration;

        afterImageTimer = 0f;

        AudioManager.instance.PlaySFX(15);
    }

    public override void Exit()
    {
        base.Exit();

        player.stats.MakeInvincible(false);

        if (canCreateClone)
        {
            player.skill.clone.CreateCloneOnDashOver();
            OnCloneOnDashUsed?.Invoke();
            canCreateClone = false;
        }

        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.skill.dash.dashSpeed * player.skill.dash.dashDir, 0);

        afterImageTimer -= Time.deltaTime;
        if (afterImageTimer <= 0f)
        {
            bool isFacingRight = player.facingDir > 0;
            AfterImageManager.instance.CreateAfterImage(player.sr.sprite, player.transform.position, isFacingRight, AfterImageType.Dash);
            afterImageTimer = afterImageCooldown;
        }

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);

        if (Input.GetKeyDown(KeyCode.Mouse0) && SkillManager.instance.dashAttack.CanUseSkill() && SkillManager.instance.dash.dashAttack)
            stateMachine.ChangeState(player.dashAttack);
    }
}
