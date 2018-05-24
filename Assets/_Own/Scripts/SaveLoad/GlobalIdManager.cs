using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class GlobalIdManager : SimpleSingleton<GlobalIdManager>
{
    private Dictionary<Guid, int> guidToInstanceId = new Dictionary<Guid, int>();
    private Dictionary<Guid, object> guidToData = new Dictionary<Guid, object>();

    public GlobalIdManager()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    public void Register(Guid guid, int instanceId)
    {
        guidToInstanceId.Add(guid, instanceId);
    }

    public void Unregister(Guid guid)
    {
        guidToInstanceId.Remove(guid);
    }

    public bool GetRegistered(Guid guid, out int instanceId)
    {
        return guidToInstanceId.TryGetValue(guid, out instanceId);
    }

    public void SaveData<T>(Guid guid, T data)
    {
        guidToData[guid] = data;
    }

    public bool GetSavedData<T>(Guid guid, out T data)
    {
        object obj;
        if (guidToData.TryGetValue(guid, out obj))
        {
            data = (T)obj;
            return true;
        }
        data = default(T);
        return false;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (oldScene.IsValid() && oldScene.buildIndex != newScene.buildIndex)
        {
            guidToData.Clear();
        }
    }
}
