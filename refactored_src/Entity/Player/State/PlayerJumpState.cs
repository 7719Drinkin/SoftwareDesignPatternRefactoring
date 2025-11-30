using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private float jumpCooldown = 0.1f;
    private float jumpTimer;
    private bool continuousJump;

    private float afterImageTimer;
    private float afterImageCooldown = 0.1f;

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(rb.velocity.x, player.jumpForce);
        player.continousJump = 1;
        continuousJump = false;

        ItemData_Equipment equipedAmulet = inventory.GetEquipment(EquipmentType.Amulet);

        if (equipedAmulet != null && amuletSkillManager.JumpUseAmulet)
            if (equipmentUsageManager.CanUseAmulet())
                equipedAmulet.ExecuteItemEffect(player.transform);

        afterImageTimer = 0f;

        audioManager.PlaySFX(14);
    }

    public override void Exit()
    {
        base.Exit();

        jumpTimer = jumpCooldown;
    }

    public override void Update()
    {
        base.Update();

        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);

        if (Input.GetKeyDown(KeyCode.Space) && player.continousJump > 0 && jumpTimer < 0)
        {
            jumpTimer = jumpCooldown;
            player.continousJump--;
            player.SetVelocity(rb.velocity.x * 1.2f, player.jumpForce * 1.2f);

            continuousJump = true;

            audioManager.PlaySFX(14);
        }

        jumpTimer -= Time.deltaTime;

        afterImageTimer -= Time.deltaTime;

        if (afterImageTimer <= 0f && continuousJump)
        {
            bool isFacingRight = player.facingDir > 0;
            AfterImageManager.instance.CreateAfterImage(player.sr.sprite, player.transform.position, isFacingRight, AfterImageType.Jump);
            afterImageTimer = afterImageCooldown;
        }
    }
}