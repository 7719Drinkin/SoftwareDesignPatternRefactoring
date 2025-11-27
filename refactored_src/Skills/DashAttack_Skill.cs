using System;

public class DashAttack_Skill : Skill
{
    public event Action OnDashAttackUsed;

    public override void UseSkill()
    {
        base.UseSkill();

        OnDashAttackUsed?.Invoke();
    }
}
