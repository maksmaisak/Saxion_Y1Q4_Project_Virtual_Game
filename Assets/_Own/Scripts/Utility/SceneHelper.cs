using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SceneHelper : SimpleSingleton<SceneHelper>
{
    public event Action OnActiveSceneChange;

    private int currentSceneBuildIndex = -1;

    public SceneHelper()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene currentScene, Scene nextScene)
    {
        if (currentSceneBuildIndex == -1)
        {
            currentSceneBuildIndex = nextScene.buildIndex;
            return;
        }

        if (currentSceneBuildIndex != nextScene.buildIndex)
        {
            currentSceneBuildIndex = nextScene.buildIndex;
            if (OnActiveSceneChange != null) OnActiveSceneChange();
        }
    }
}