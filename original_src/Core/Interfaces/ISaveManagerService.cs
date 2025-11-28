/// <summary>
/// 存档管理器服务接口
/// 定义游戏存档的保存和加载功能
/// </summary>
public interface ISaveManagerService
{
    /// <summary>
    /// 创建新游戏
    /// </summary>
    void NewGame();

    /// <summary>
    /// 加载游戏
    /// </summary>
    void LoadGame();

    /// <summary>
    /// 保存游戏
    /// </summary>
    void SaveGame();

    /// <summary>
    /// 删除存档文件
    /// </summary>
    void DeleteSaveFile();

    /// <summary>
    /// 检查是否有存档数据
    /// </summary>
    /// <returns>是否有存档</returns>
    bool HaveSaveData();
}

