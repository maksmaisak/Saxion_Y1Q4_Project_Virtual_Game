using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public abstract class ManagementSingleton<T> : MonoBehaviour where T : ManagementSingleton<T>
{
    public const string managementSceneName = "management";
    public const string managementScenePath = "Assets/_Own/Scenes/Final/management.unity";

    private static T instance;
    public static T Instance
    {
        get
        {
            MakeSureManagementSceneIsLoaded();

            if (instance == null)
            {
                if (isApplicationQuitting) return null;

                instance = FindInstance() ?? CreateInstance();
            }

            return instance;
        }
    }

    private static Scene managementScene;
    public static Scene ManagementScene 
    {
        get 
        {
            MakeSureManagementSceneIsLoaded();
            return managementScene;
        }
    }

    private static bool isApplicationQuitting;

    protected static void SaveManagementSceneChanges()
    {
        if (Application.isPlaying) return;
        if (isApplicationQuitting) return;
        EditorSceneManager.SaveScene(ManagementScene);
    }

    private static T FindInstance()
    {
        Scene currentlyActiveScene = SceneManager.GetActiveScene();
        bool shouldSwitch = currentlyActiveScene.IsValid() && currentlyActiveScene.name != managementSceneName;
        if (shouldSwitch)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(managementSceneName));
        }

        T result = FindObjectOfType<T>();

        if (shouldSwitch)
        {
            SceneManager.SetActiveScene(currentlyActiveScene);
        }

        return result; 
    }

    private static T CreateInstance()
    {
        var gameObject = new GameObject("(Management Singleton) " + typeof(T));
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(managementSceneName));
        return gameObject.AddComponent<T>();
    }

    private static void MakeSureManagementSceneIsLoaded()
    {
        if (managementScene.IsValid() && managementScene.isLoaded) return;

        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name == managementSceneName)
            {
                managementScene = loadedScene;
                return;
            }
        }

        if (Application.isPlaying)
        {
            SceneManager.LoadScene(managementSceneName, LoadSceneMode.Additive);
            managementScene = SceneManager.GetSceneByName(managementSceneName);
        }
        else 
        {
            managementScene = EditorSceneManager.OpenScene(managementScenePath, OpenSceneMode.Additive);
        }
    }

    protected virtual void OnDestroy()
    {
        isApplicationQuitting = true;
    }
}
