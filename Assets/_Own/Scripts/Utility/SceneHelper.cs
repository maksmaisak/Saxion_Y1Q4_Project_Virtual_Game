using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SceneHelper : SimpleSingleton<SceneHelper>
{
    public event Action OnActiveSceneChange;

    public SceneHelper() 
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (oldScene.IsValid() && oldScene.buildIndex != newScene.buildIndex)
        {
            if (OnActiveSceneChange != null) OnActiveSceneChange();
        }
    }
}
