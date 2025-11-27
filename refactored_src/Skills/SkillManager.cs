using UnityEngine;

/// <summary>
/// 技能管理器 - 管理所有玩家技能
/// 作为技能系统的中央管理器，统一管理各种技能组件
/// 提供单例访问模式，方便其他系统调用技能
/// </summary>
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;                  // 单例实例

    public Dash_Skill dash { get; private set; }           // 冲刺技能
    public Clone_Skill clone { get; private set; }         // 分身技能
    public Sword_Skill sword { get; private set; }         // 剑技能
    public Blackhole_Skill blackhole { get; private set; } // 黑洞技能
    public Crystal_Skill crystal { get; private set; }     // 水晶技能
    public DashAttack_Skill dashAttack { get; private set; } // 冲刺攻击技能
    public Parry_Skill parry { get; private set; }        // 格挡技能
    public Assassinate_Skill assassinate { get; private set; } // 暗杀技能

    /// <summary>
    /// 初始化单例
    /// </summary>
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

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
