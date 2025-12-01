/// <summary>
/// 护身符技能管理器接口 - 负责管理护身符技能状态
/// </summary>
public interface IAmuletSkillManager
{
    bool DashUseAmulet { get; set; }
    bool JumpUseAmulet { get; set; }
    bool SwordUseAmulet { get; set; }
}

