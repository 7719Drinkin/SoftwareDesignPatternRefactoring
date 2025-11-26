using UnityEngine;

public class PlayerAirState : PlayerState
{
    private float jumpCooldown = 0.25f;
    private float jumpTimer;

    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if (xInput != 0)
            player.SetVelocity(player.moveSpeed * 0.8f * xInput, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && player.continousJump > 0 && jumpTimer < 0)
        {
            jumpTimer = jumpCooldown;
            player.continousJump--;
            player.SetVelocity(rb.velocity.x, player.jumpForce * 0.8f);

            AudioManager.instance.PlaySFX(14);
        }

        jumpTimer -= Time.deltaTime;
    }
}
