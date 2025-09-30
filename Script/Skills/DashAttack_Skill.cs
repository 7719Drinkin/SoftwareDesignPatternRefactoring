using System;

public class DashAttack_Skill : Skill
{
    public event Action OnDashAttackUsed;

    public override void UseSkill()
    {
        base.UseSkill();

        foreach (var item in player.dashAttackEffects)
            item.ExecuteEffect(player.transform);

        OnDashAttackUsed?.Invoke();
    }
}
