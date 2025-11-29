using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 存档管理器 - 管理游戏的保存和加载功能
/// 统一管理所有实现了ISaveManager接口的组件
/// 支持加密存档和自动保存功能
/// </summary>
public class SaveManager : MonoBehaviour
{
    private GameData gameData;                             // 游戏数据

    public static SaveManager instance;                    // 单例实例

    private List<ISaveManager> saveManagers = new List<ISaveManager>(); // 存档管理器列表

    [SerializeField] private string fileName;               // 存档文件名
    [SerializeField] private bool encrypt;                  // 是否加密

    private FileDataHandler dataHandler;                   // 文件数据处理器

    /// <summary>
    /// 删除存档文件（编辑器菜单）
    /// </summary>
    [ContextMenu("Delete save file")]
    public void DeleteSaveFile()
    {
        dataHandler = new FileDataHandler(Application.streamingAssetsPath, fileName, encrypt);

        dataHandler.Delete();
    }

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
    /// 初始化存档系统
    /// </summary>
    private void Start()
    {
        dataHandler = new FileDataHandler(Application.streamingAssetsPath, fileName, encrypt);

        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    /// <summary>
    /// 创建新游戏
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    public void LoadGame()
    {
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
            saveManager.LoadData(gameData);
    }

    /// <summary>
    /// 保存游戏
    /// </summary>
    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
            saveManager.SaveData(ref gameData);

        dataHandler.Save(gameData);
    }

    /// <summary>
    /// 应用退出时自动保存
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// 查找所有存档管理器
    /// </summary>
    /// <returns>存档管理器列表</returns>
    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> _saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(_saveManagers);
    }

    /// <summary>
    /// 检查是否有存档数据
    /// </summary>
    /// <returns>是否有存档</returns>
    public bool HaveSaveData()
    {
        if (dataHandler.Load() != null)
            return true;

        return false;
    }
}
