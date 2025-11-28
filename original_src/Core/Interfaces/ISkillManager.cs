/// <summary>
/// 技能管理器接口 - 定义技能管理操作
/// </summary>
public interface ISkillManager
{
    Dash_Skill Dash { get; }
    Clone_Skill Clone { get; }
    Sword_Skill Sword { get; }
    Blackhole_Skill Blackhole { get; }
    Crystal_Skill Crystal { get; }
    DashAttack_Skill DashAttack { get; }
    Parry_Skill Parry { get; }
    Assassinate_Skill Assassinate { get; }
}
