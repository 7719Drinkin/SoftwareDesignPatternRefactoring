using UnityEngine;

/***************************************************************
 *  Refactored with: Command Pattern
 *  Pattern Type: Behavioral
 *
 *  Document Reference:
 *  - See report section "2.2.4 Command Pattern (Behavioral)"
 ***************************************************************/
/// <summary>
/// 玩家输入处理器 - Invoker（命令调用者）
/// 负责检测输入并执行对应的命令
/// 不依赖于具体的命令或接收者类，通过命令接口间接传递请求
/// </summary>
public class PlayerInputHandler : MonoBehaviour
{
    // 命令槽位
    private ISkillCommand dashCommand;
    private ISkillCommand crystalCommand;
    private ISkillCommand flaskCommand;
    private ISkillCommand assassinateCommand;

    /// <summary>
    /// 设置冲刺命令
    /// </summary>
    public void SetDashCommand(ISkillCommand command)
    {
        this.dashCommand = command;
    }

    /// <summary>
    /// 设置水晶命令
    /// </summary>
    public void SetCrystalCommand(ISkillCommand command)
    {
        this.crystalCommand = command;
    }

    /// <summary>
    /// 设置药水命令
    /// </summary>
    public void SetFlaskCommand(ISkillCommand command)
    {
        this.flaskCommand = command;
    }

    /// <summary>
    /// 设置暗杀命令
    /// </summary>
    public void SetAssassinateCommand(ISkillCommand command)
    {
        this.assassinateCommand = command;
    }

    /// <summary>
    /// 检测输入并执行命令
    /// </summary>
    public void HandleInput()
    {
        // 检测冲刺输入（Q键）
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExecuteCommand(dashCommand);
        }

        // 检测水晶技能输入（C键）
        if (Input.GetKeyDown(KeyCode.C))
        {
            ExecuteCommand(crystalCommand);
        }

        // 检测药水输入（H键）
        if (Input.GetKeyDown(KeyCode.H))
        {
            ExecuteCommand(flaskCommand);
        }

        // 检测暗杀技能输入（X键）
        if (Input.GetKeyDown(KeyCode.X))
        {
            ExecuteCommand(assassinateCommand);
        }
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    private void ExecuteCommand(ISkillCommand command)
    {
        if (command != null)
        {
            command.Execute();
        }
    }
}
/***************************************************************
 * End
 ***************************************************************/

