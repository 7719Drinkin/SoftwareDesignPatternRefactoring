using UnityEngine;
using System.Text;
using UnityEditor;

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemId;

    protected StringBuilder builder = new StringBuilder();
    protected int descriptionLength;

    [Range(0, 100)]
    public float dropChance;

    private void OnValidate()
    {
        itemName = name;

#if UNITY_EDITOR
        // 获取当前ScriptableObject在项目中的相对路径
        string path = AssetDatabase.GetAssetPath(this);
        // 路径转换为GUID
        itemId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDescription()
    {
        return "";
    }
}