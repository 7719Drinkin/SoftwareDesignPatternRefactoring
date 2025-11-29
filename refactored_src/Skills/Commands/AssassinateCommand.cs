using UnityEngine;

/// <summary>
/// 暗杀技能命令
/// Receiver: Player + SkillManager
/// </summary>
public class AssassinateCommand : ISkillCommand
{
    private readonly Player player;
    private readonly ISkillManager skillManager;

    public AssassinateCommand(Player player, ISkillManager skillManager)
    {
        this.player = player;
        this.skillManager = skillManager;
    }

    public void Execute()
    {
        // 黑洞状态下不能使用暗杀
        if (player.stateMachine.currentState == player.blackhole)
            return;

        if (skillManager?.Assassinate != null && skillManager.Assassinate.assassinate)
        {
            if (skillManager.Assassinate.CanUseSkill())
            {
                player.stateMachine.ChangeState(player.assassinate);
            }
        }
    }
}

