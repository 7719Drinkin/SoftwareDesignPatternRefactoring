using UnityEngine;

/// <summary>
/// 冲刺命令 - 封装冲刺技能的执行逻辑
/// Receiver: Player + SkillManager + Inventory
/// </summary>
public class DashCommand : ISkillCommand
{
    private readonly Player player;
    private readonly ISkillManager skillManager;
    private readonly IInventory inventory;

    public DashCommand(Player player, ISkillManager skillManager, IInventory inventory)
    {
        this.player = player;
        this.skillManager = skillManager;
        this.inventory = inventory;
    }

    public void Execute()
    {
        if (player.stats.isDead)
            return;

        if (player.IsWallDetected())
            return;

        if (!skillManager.Dash.dash)
            return;

        if (skillManager.Dash.CanUseSkill())
        {
            // 黑洞状态下不能冲刺
            if (player.stateMachine.currentState == player.blackhole)
                return;

            // 设置冲刺方向
            skillManager.Dash.dashDir = Input.GetAxisRaw("Horizontal");

            if (skillManager.Dash.dashDir == 0)
                skillManager.Dash.dashDir = player.facingDir;

            // 如果装备了护身符且可以使用，则延迟使用
            if (inventory.DashUseAmulet && inventory.CanUseAmulet())
            {
                player.StartCoroutine(DelayUseAmulet());
            }

            player.stateMachine.ChangeState(player.dashState);
        }
    }

    /// <summary>
    /// 延迟使用护身符
    /// </summary>
    private System.Collections.IEnumerator DelayUseAmulet()
    {
        yield return new WaitForSeconds(0.125f);

        ItemData_Equipment equipedAmulet = inventory.GetEquipment(EquipmentType.Amulet);

        if (equipedAmulet != null)
            equipedAmulet.ExecuteItemEffect(player.transform);
    }
}

