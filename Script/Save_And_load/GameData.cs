using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public int currency;
    public int currentExperience;
    public int playerLevel;

    public SerializableDictionary<string, int> inventory;

    public List<string> equipmentId;

    public SerializableDictionary<string, bool> skillTree;

    public SerializableDictionary<string, bool> checkpoints;

    public string closestCheckpointId;

    public SerializableDictionary<Vector3, string> droppedItems;
    public SerializableDictionary<Vector3, int> droppedExperience;

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
    }
}
