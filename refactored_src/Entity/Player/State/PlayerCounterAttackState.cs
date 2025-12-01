using System;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{

    public PlayerCounterAttackState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.counterAttackDuration;

        player.anim.SetBool("SuccessfulCounterAttack", false);

        // ========== 发布技能使用事件到事件总线（Observer Pattern） ==========
        var eventBus = GameFacade.Instance.Events;
        eventBus?.Publish(new SkillUsedEvent
        {
            SkillName = "Parry",
            Cooldown = player.skill.Parry.cooldown
        });
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.ZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                if (hit.GetComponent<Enemy>().EnemyCanBeBlocked())
                {
                    stateTimer = 10; // any value bigger than 1
                    player.anim.SetBool("SuccessfulCounterAttack", true);
                    player.skill.Clone.CreateCloneOnCounterAttack(hit.transform);
                    audioManager.PlaySFX(5);

                    // 玩家成功格挡/招架时的相机抖动
                    if (CinemachineShaker.instance != null)
                        CinemachineShaker.instance.Shake(1.0f, 1.8f, 0.14f);

                    // 成功招架：更强的hit-stop、轻微拉近与色差闪光
                    if (CombatFeedback.instance != null)
                    {
                        CombatFeedback.instance.DoHitStop(0.1f);
                        CombatFeedback.instance.DoZoom(-5f, 0.06f, 0.18f);
                        CombatFeedback.instance.DoChromaticFlash(0.85f, 0.18f);
                    }
                }
        }

        if (stateTimer < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
