using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GlobalIdManager : ManagementSingleton<GlobalIdManager>
{
    private Dictionary<string, int> guidToInstanceId = new Dictionary<string, int>();
    private Dictionary<string, object> guidToData = new Dictionary<string, object>();

    void Awake()
    {
        SceneHelper.Instance.OnActiveSceneChange += OnActiveSceneChanged;
    }

    public void Register(string stringGuid, int instanceId)
    {
        guidToInstanceId.Add(stringGuid, instanceId);
        SaveManagementSceneChanges();
    }

    public void Unregister(string stringGuid)
    {
        guidToInstanceId.Remove(stringGuid);
        SaveManagementSceneChanges();
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

    private void OnActiveSceneChanged()
    {
        guidToInstanceId.Clear();
        guidToData.Clear();
    }
}
