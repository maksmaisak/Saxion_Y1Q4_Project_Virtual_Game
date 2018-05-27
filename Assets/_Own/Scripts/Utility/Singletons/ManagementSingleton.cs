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
            if (instance == null)
            {
                if (isApplicationQuitting) return null;

                MakeSureManagementSceneIsLoaded();
                instance = FindInstance() ?? CreateInstance();
            }

            return instance;
        }
    }

    private static bool isApplicationQuitting;

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
        if (IsManagementSceneLoaded()) return;

        if (Application.isPlaying)
        {
            SceneManager.LoadScene(managementSceneName, LoadSceneMode.Additive);
        }
        else 
        {
            EditorSceneManager.OpenScene(managementScenePath, OpenSceneMode.Additive);
        }
    }

    private static bool IsManagementSceneLoaded()
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            if (SceneManager.GetSceneAt(i).name == managementSceneName)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void OnDestroy()
    {
        isApplicationQuitting = true;
    }
}
