using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private GameData gameData;

    public static SaveManager instance;

    private List<ISaveManager> saveManagers = new List<ISaveManager>();

    [SerializeField] private string fileName;
    [SerializeField] private bool encrypt;

    private FileDataHandler dataHandler;

    [ContextMenu("Delete save file")]
    public void DeleteSaveFile()
    {
        dataHandler = new FileDataHandler(Application.streamingAssetsPath, fileName, encrypt);

        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.streamingAssetsPath, fileName, encrypt);

        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

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

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
            saveManager.SaveData(ref gameData);

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> _saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(_saveManagers);
    }

    public bool HaveSaveData()
    {
        if (dataHandler.Load() != null)
            return true;

        return false;
    }
}
