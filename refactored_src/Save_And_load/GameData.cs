using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据类 - 存储所有需要保存的游戏数据
/// 包含玩家信息、物品、技能、检查点和掉落物等数据
/// 使用SerializableDictionary来序列化字典数据
/// </summary>
[System.Serializable]
public class GameData
{
    // 玩家基础信息
    public int currency;                                    // 货币数量
    public int currentExperience;                          // 当前经验值
    public int playerLevel;                                // 玩家等级

    // 物品系统数据
    public SerializableDictionary<string, int> inventory;  // 背包物品（物品ID -> 数量）

    public List<string> equipmentId;                       // 已装备物品ID列表

    // 技能树数据
    public SerializableDictionary<string, bool> skillTree; // 技能树解锁状态（技能ID -> 是否解锁）

    // 检查点数据
    public SerializableDictionary<string, bool> checkpoints; // 检查点激活状态（检查点ID -> 是否激活）

    public string closestCheckpointId;                     // 最近的检查点ID

    // 掉落物数据
    public SerializableDictionary<Vector3, string> droppedItems;     // 掉落物品（位置 -> 物品ID）
    public SerializableDictionary<Vector3, int> droppedExperience;   // 掉落经验（位置 -> 经验值）

    public float SFXVolume; // 游戏音效音量
    public float BGMVolume; // 背景音乐音量

    public SerializableDictionary<string, bool> chests; // 宝箱解锁状态（宝箱ID -> 是否解锁）

    /// <summary>
    /// 构造函数 - 初始化默认值
    /// </summary>
    public GameData()
    {
        this.currency = 0;
        this.currentExperience = 0;
        this.playerLevel = 1;

        inventory = new SerializableDictionary<string, int>();

        equipmentId = new List<string>();

        skillTree = new SerializableDictionary<string, bool>();

        checkpoints = new SerializableDictionary<string, bool>();

        closestCheckpointId = string.Empty;

        droppedItems = new SerializableDictionary<Vector3, string>();
        droppedExperience = new SerializableDictionary<Vector3, int>();

        SFXVolume = 1;
        BGMVolume = 1;

        chests = new SerializableDictionary<string, bool>();
    }
}
