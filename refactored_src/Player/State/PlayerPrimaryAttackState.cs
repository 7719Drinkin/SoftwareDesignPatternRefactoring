using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter { get; private set; }

    private float lastTimeAttacked;
    private float defaultRadius;
    private float comboWindow = 2f;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        defaultRadius = player.attackCheckRadius;
        switch (comboCounter)
        {
            case 0:
                player.attackCheckRadius = player.firstAttackCheckRadius;
                audioManager.PlaySFX(1);
                break;
            case 1:
                player.attackCheckRadius = player.secondAttackCheckRadius;
                audioManager.PlaySFX(2);
                break;
            case 2:
                player.attackCheckRadius = player.thirdAttackCheckRadius;
                audioManager.PlaySFX(3);
                break;
        }

        player.anim.SetInteger("ComboCounter", comboCounter);

        player.SetVelocity(player.attackMovement[comboCounter].x * player.facingDir, player.attackMovement[comboCounter].y);

        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", 0.15f);

        comboCounter++;
        lastTimeAttacked = Time.time;

        player.attackCheckRadius = defaultRadius;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.ZeroVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
