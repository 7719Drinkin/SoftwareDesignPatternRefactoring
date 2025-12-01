using UnityEngine;

/// <summary>
/// 水晶技能命令
/// Receiver: SkillManager
/// </summary>
public class CrystalCommand : ISkillCommand
{
    private readonly ISkillManager skillManager;

    public CrystalCommand(ISkillManager skillManager)
    {
        this.skillManager = skillManager;
    }

    public void Execute()
    {
        if (skillManager?.Crystal != null && skillManager.Crystal.crystal)
        {
            skillManager.Crystal.CanUseSkill();
        }
    }
}

