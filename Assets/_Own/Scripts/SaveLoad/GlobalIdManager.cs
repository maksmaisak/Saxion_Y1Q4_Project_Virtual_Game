using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class GlobalIdMananger : Singleton<GlobalIdMananger>
{
    private Dictionary<Guid, GlobalId> guidToComponent = new Dictionary<Guid, GlobalId>();
    private Dictionary<Guid, object> guidToData = new Dictionary<Guid, object>();

    void Awake()
    {
        Assert.IsTrue(guidToComponent.Count == 0);
        DontDestroyOnLoad(this);

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    public void Register(Guid guid, GlobalId component)
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

    public T RetrieveData<T>(Guid guid)
    {
        return (T)guidToData[guid];
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (oldScene.buildIndex != newScene.buildIndex)
        {
            guidToData.Clear();
        }
    }
}
