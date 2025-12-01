using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏事件总线 - Observer Pattern 的 Subject（发布者）
/// 负责管理事件订阅和发布
/// 通过 ServiceLocator 管理，不使用单例
/// </summary>
public class GameEventBus
{
    // 事件订阅字典：事件类型 → 订阅者列表
    private Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// 订阅事件 - 注册观察者（Observer）
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="callback">回调函数</param>
    public void Subscribe<T>(Action<T> callback) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!subscribers.ContainsKey(eventType))
        {
            subscribers[eventType] = new List<Delegate>();
        }

        subscribers[eventType].Add(callback);
    }

    /// <summary>
    /// 取消订阅事件 - 移除观察者
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="callback">回调函数</param>
    public void Unsubscribe<T>(Action<T> callback) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (subscribers.ContainsKey(eventType))
            subscribers[eventType].Remove(callback);
    }

    /// <summary>
    /// 发布事件 - 通知所有订阅者（观察者）
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="gameEvent">事件数据</param>
    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (subscribers.ContainsKey(eventType))
        {
            foreach (Delegate subscriber in subscribers[eventType])
            {
                (subscriber as Action<T>)?.Invoke(gameEvent);
            }
        }
    }
}

