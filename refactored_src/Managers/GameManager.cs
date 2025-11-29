using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏管理器 - 管理游戏的核心功能
/// 负责检查点系统、场景重启、游戏暂停和存档管理
/// 实现ISaveManager接口，支持检查点数据的保存和加载
/// </summary>
public class GameManager : MonoBehaviour, ISaveManager, IGameManager
{
    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private Chest[] chests;

    private IPlayerManager playerManager;
    private IAudioManager audioManager;
    private ISaveManagerService saveManager;

    private void Awake()
    {
        checkPoints = FindObjectsOfType<CheckPoint>();
        chests = FindObjectsOfType<Chest>();

        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        saveManager = ServiceLocator.Instance.Get<ISaveManagerService>();
    }

    /// <summary>
    /// 重启当前场景
    /// </summary>
    public void ReStartScene()
    {
        if (saveManager != null)
            saveManager.LoadGame();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// 加载检查点数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void LoadData(GameData data)
    {
        StartCoroutine(LoadDataRoutine(data));
    }

    private System.Collections.IEnumerator LoadDataRoutine(GameData data)
    {
        while (!TryEnsurePlayer())
            yield return null;

        // 激活已保存的检查点
        foreach (KeyValuePair<string, bool> pair in data.checkpoints)
        {
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (pair.Key == checkPoint.checkpointId && pair.Value)
                    checkPoint.ActivateCheckPoint();
            }
        }

        // 将玩家传送到最近的检查点
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (checkPoint.checkpointId == data.closestCheckpointId)
                playerManager.Player.transform.position = checkPoint.transform.position;
        }

        // 加载所有宝箱状态
        foreach (KeyValuePair<string, bool> pair in data.chests)
        {
            foreach (Chest chest in chests)
                if (pair.Key == chest.chestId && pair.Value)
                    chest.SetChestOpen();
        }
    }

    /// <summary>
    /// 保存检查点数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void SaveData(ref GameData data)
    {
        CheckPoint closestCheckpoint = FindClosestCheckpoint();
        if (closestCheckpoint != null)
            data.closestCheckpointId = closestCheckpoint.checkpointId;
        else
            data.closestCheckpointId = string.Empty;

        data.checkpoints.Clear();
        data.chests.Clear();

        // 保存所有检查点状态（跳过空ID，使用索引器避免重复Add异常）
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (string.IsNullOrEmpty(checkPoint.checkpointId))
                continue;
            data.checkpoints[checkPoint.checkpointId] = checkPoint.activated;
        }

        // 保存所有宝箱状态（跳过空ID，使用索引器避免重复Add异常）
        foreach (Chest chest in chests)
        {
            if (string.IsNullOrEmpty(chest.chestId))
                continue;
            data.chests[chest.chestId] = chest.opened;
        }
    }

    /// <summary>
    /// 查找最近的检查点
    /// </summary>
    /// <returns>最近的检查点</returns>
    private CheckPoint FindClosestCheckpoint()
    {
        if (!TryEnsurePlayer())
            return null;

        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        foreach (var checkpoint in checkPoints)
        {
            float distanceToCheckpoint = Vector2.Distance(playerManager.Player.transform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activated == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }

        }
        return closestCheckpoint;
    }

    private bool TryEnsurePlayer()
    {
        if (playerManager == null)
            playerManager = ServiceLocator.Instance.Get<IPlayerManager>();

        return playerManager != null && playerManager.Player != null;
    }

    /// <summary>
    /// 暂停/恢复游戏
    /// </summary>
    /// <param name="pause">是否暂停</param>
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            audioManager.StopAllLoopSFX();
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
