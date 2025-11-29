using System;

/// <summary>
/// 玩家管理器接口 - 定义所有玩家数据管理操作
/// </summary>
public interface IPlayerManager
{
    Player Player { get; set; }
    int Currency { get; set; }
    int CurrentExperience { get; set; }
    int PlayerLevel { get; set; }
    bool HaveEnoughMoney(int price);
    void AddExperience(int amount);
    event Action OnPlayerDataLoaded;
}
