using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 创建了一个可序列化的字典类，解决了Unity中普通Dictionary无法在Inspector面板中显示和保存的问题
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    // Unity加载数据时调用
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            Debug.Log("Keys count is not equal to values count");

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }

    //  Unity保存数据时调用
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
