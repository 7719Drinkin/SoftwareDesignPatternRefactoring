using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;

    [SerializeField] private CheckPoint[] checkPoints;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        checkPoints = FindObjectsOfType<CheckPoint>();
    }

    public void ReStartScene()
    {
        SaveManager.instance.LoadGame();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData data)
    {
        foreach (KeyValuePair<string, bool> pair in data.checkpoints)
        {
            foreach (CheckPoint checkPoint in checkPoints)
            {
                if (pair.Key == checkPoint.checkpointId && pair.Value)
                    checkPoint.ActivateCheckPoint();
            }
        }

        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (checkPoint.checkpointId == data.closestCheckpointId)
                PlayerManager.instance.player.transform.position = checkPoint.transform.position;
        }
    }

    public void SaveData(ref GameData data)
    {
        CheckPoint closestCheckpoint = FindClosestCheckpoint();
        if (closestCheckpoint != null)
            data.closestCheckpointId = closestCheckpoint.checkpointId;
        else
            data.closestCheckpointId = string.Empty;

        data.checkpoints.Clear();

        foreach (CheckPoint checkPoint in checkPoints)
            data.checkpoints.Add(checkPoint.checkpointId, checkPoint.activated);
    }

    private CheckPoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        foreach (var checkpoint in checkPoints)
        {
            float distanceToCheckpoint = Vector2.Distance(PlayerManager.instance.player.transform.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activated == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }

        }
        return closestCheckpoint;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
            AudioManager.instance.StopAllLoopSFX();
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
