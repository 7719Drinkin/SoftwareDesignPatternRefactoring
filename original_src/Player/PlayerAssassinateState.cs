using System.Collections;
using UnityEngine;

public class PlayerAssassinateState : PlayerState
{
    private float afterImageTimer;
    private float afterImageCooldown = 0.03f;
    private float defaultGravity;

    public PlayerAssassinateState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = rb.gravityScale;
        rb.gravityScale = 0;

        player.stats.MakeInvincible(true);
        player.stats.critPower.AddModifier(50);
        player.stats.critChance.AddModifier(100);

        player.SetVelocity(player.facingDir * 15, 0);

        player.StartCoroutine(StopAssassinateSpeed());

        afterImageTimer = 0f;

        AudioManager.instance.PlaySFX(15);
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = defaultGravity;
        
        player.stats.MakeInvincible(false);
        player.stats.critChance.RemoveModifier(100);
        player.stats.critChance.RemoveModifier(50);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        afterImageTimer -= Time.deltaTime;
        if (afterImageTimer <= 0f)
        {
            bool isFacingRight = player.facingDir > 0;
            AfterImageManager.instance.CreateAfterImage(player.sr.sprite, player.transform.position, isFacingRight, AfterImageType.Assassinate);
            afterImageTimer = afterImageCooldown;
        }
    }

    private IEnumerator StopAssassinateSpeed()
    {
        yield return new WaitForSeconds(0.3f);

        player.ZeroVelocity();
    }
}
