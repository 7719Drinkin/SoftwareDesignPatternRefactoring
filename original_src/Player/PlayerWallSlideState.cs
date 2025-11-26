using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    private float wallSlideTimer;
    private float wallSlideBuffer = 0.1f;

    public PlayerWallSlideState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJump);
            return;
        }

        if (yInput < 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y * 0.8f);

        if (xInput != 0 && player.facingDir != xInput)
            stateMachine.ChangeState(player.idleState);

        if (player.IsGroundDetected())
            stateMachine.ChangeState(player.idleState);

        if (!player.IsWallDetected())
        {
            wallSlideTimer += Time.deltaTime;
            if (wallSlideTimer >= wallSlideBuffer)
                stateMachine.ChangeState(player.airState);
        }
        else
        {
            wallSlideTimer = 0f;
        }
    }
}
