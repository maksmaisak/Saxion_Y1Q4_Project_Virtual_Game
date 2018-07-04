using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class GameController : ManagementSingleton<GameController>
{
    /*public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (mode == LoadSceneMode.Additive)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.IsValid() && activeScene.name == sceneName) return;

        UnloadAllExcept(SceneNames.managementName);

        StartCoroutine(LoadAdditivelyAndMakeActive(sceneName));
    }

    public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (mode == LoadSceneMode.Additive)
        {
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        UnloadAllExcept(SceneNames.managementName);

        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        sceneLoading.completed += (op) => SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        return sceneLoading;
    }

    public void RestartActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        Assert.IsTrue(activeScene.IsValid());

        int buildIndex = activeScene.buildIndex;

        SceneManager.UnloadSceneAsync(buildIndex);

        //StartCoroutine(LoadAdditivelyAndMakeActive(activeScene.name));
        AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
        sceneLoading.completed += (op) => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex));
    }

    private void UnloadAllExcept(string sceneToPreserveName)
    {
        var toUnload = new List<Scene>(SceneManager.sceneCount);
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name != sceneToPreserveName)
            {
                toUnload.Add(loadedScene);
            }
        }

        foreach (Scene sceneToUnload in toUnload)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }

    private IEnumerator LoadAdditivelyAndMakeActive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }*/
}
