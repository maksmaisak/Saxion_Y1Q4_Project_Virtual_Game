using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GlobalIdManager : ManagementSingleton<GlobalIdManager>
{
    // We have to make a non-generic class for each kind of dictionary, since unity doesn't serialize anything generic except List<T>
    //[Serializable] private class SerializableDictionaryStringToInt : SerializableDictionary<string, int> {}
    [SerializeField] Dictionary<string, int> guidToInstanceId = new Dictionary<string, int>();

    //[Serializable] private class SerializableDictionaryStringToObject : SerializableDictionary<string, object> {}
    [SerializeField] Dictionary<string, object> guidToData = new Dictionary<string, object>();

    void Awake()
    {
        Debug.Log(name + " Awake with " + guidToInstanceId.Count + " guidToInstanceId items, " + guidToData.Count + " guidToData items.");
        /*SceneHelper.Instance.OnActiveSceneChange += () =>
        {
            guidToData.Clear();
        };*/
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log(name + " OnDestroy with " + guidToInstanceId.Count + " guidToInstanceId items, " + guidToData.Count + " guidToData items.");
    }

    public void Register(string stringGuid, int instanceId)
    {
        guidToInstanceId.Add(stringGuid, instanceId);
    }

    public void Unregister(string stringGuid)
    {
        guidToInstanceId.Remove(stringGuid);
    }

    public bool GetRegistered(string stringGuid, out int instanceId)
    {
        return guidToInstanceId.TryGetValue(stringGuid, out instanceId);
    }

    public void SaveData<T>(string stringGuid, T data)
    {
        guidToData[stringGuid] = data;
    }

    public bool GetSavedData<T>(string stringGuid, out T data)
    {
        object obj;
        if (guidToData.TryGetValue(stringGuid, out obj))
        {
            data = (T)obj;
            return true;
        }
        data = default(T);
        return false;
    }
}
