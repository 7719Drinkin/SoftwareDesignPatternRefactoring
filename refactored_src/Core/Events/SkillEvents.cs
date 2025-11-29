/// <summary>
/// 技能使用事件
/// </summary>
public class SkillUsedEvent : IGameEvent
{
    public string EventName => "SkillUsed";
    public string SkillName { get; set; }
    public float Cooldown { get; set; }
}

/// <summary>
/// 技能解锁事件
/// </summary>
public class SkillUnlockedEvent : IGameEvent
{
    public string EventName => "SkillUnlocked";
    public string SkillName { get; set; }
}

