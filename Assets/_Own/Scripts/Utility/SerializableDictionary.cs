using System.Collections.Generic;
using UnityEngine;

public abstract class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField] K[] keys;
    [SerializeField] V[] values;

    public void OnBeforeSerialize()
    {
        keys = new K[Count];
        values = new V[Count];

        int i = 0;
        foreach (var kvp in this)
        {
            keys[i] = kvp.Key;
            values[i] = kvp.Value;
            i++;
        }
    }

    public void OnAfterDeserialize()
    {
        int capacity = keys.Length;
        for (int i = 0; i < capacity; i++)
        {
            this[keys[i]] = values[i];
        }

        keys   = null;
        values = null;
    }
}