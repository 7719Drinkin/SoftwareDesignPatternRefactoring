/// <summary>
/// 游戏事件接口 - 所有事件的基础接口
/// 用于类型安全的事件传递
/// </summary>
public interface IGameEvent
{
    /// <summary>
    /// 事件名称
    /// </summary>
    string EventName { get; }
}

