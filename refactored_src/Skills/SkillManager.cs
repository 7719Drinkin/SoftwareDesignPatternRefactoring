using UnityEngine;

/// <summary>
/// 技能管理器 - 管理所有玩家技能
/// 作为技能系统的中央管理器，统一管理各种技能组件
/// </summary>
public class SkillManager : MonoBehaviour, ISkillManager
{
    private Dash_Skill dash;
    private Clone_Skill clone;
    private Sword_Skill sword;
    private Blackhole_Skill blackhole;
    private Crystal_Skill crystal;
    private DashAttack_Skill dashAttack;
    private Parry_Skill parry;
    private Assassinate_Skill assassinate;

    // 接口属性实现
    public Dash_Skill Dash => dash;
    public Clone_Skill Clone => clone;
    public Sword_Skill Sword => sword;
    public Blackhole_Skill Blackhole => blackhole;
    public Crystal_Skill Crystal => crystal;
    public DashAttack_Skill DashAttack => dashAttack;
    public Parry_Skill Parry => parry;
    public Assassinate_Skill Assassinate => assassinate;

    /// <summary>
    /// 初始化所有技能组件
    /// </summary>
    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackhole = GetComponent<Blackhole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
        dashAttack = GetComponent<DashAttack_Skill>();
        parry = GetComponent<Parry_Skill>();
        assassinate = GetComponent<Assassinate_Skill>();
    }
}
