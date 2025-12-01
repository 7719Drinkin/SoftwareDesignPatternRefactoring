using System;

public class DashAttack_Skill : Skill
{
    public override void UseSkill()
    {
        base.UseSkill();

        // ========== 发布到事件总线（Observer Pattern） ==========
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Publish(new SkillUsedEvent
        {
            SkillName = "DashAttack",
            Cooldown = cooldown
        });
    }
}
