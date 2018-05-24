using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class GlobalIdManager : Singleton<GlobalIdManager>
{
    private Dictionary<Guid, Saveable> guidToComponent = new Dictionary<Guid, Saveable>();
    private Dictionary<Guid, object> guidToData = new Dictionary<Guid, object>();

    void Awake()
    {
        Assert.IsTrue(guidToComponent.Count == 0);
        DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    public void Register(Guid guid, Saveable component)
    {
        guidToComponent.Add(guid, component);
    }

    public void Unregister(Guid guid)
    {
        guidToComponent.Remove(guid);
    }

    public bool IsAlreadyRegistered(Guid guid)
    {
        return guidToComponent.ContainsKey(guid);
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
