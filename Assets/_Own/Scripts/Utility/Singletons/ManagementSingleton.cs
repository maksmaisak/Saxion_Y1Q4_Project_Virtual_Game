using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public class ManagementSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public const string managementSceneName = "management";
    public const string managementScenePath = "Assets/_Own/Scenes/Final/management.unity";

    private static T instance;

    public static T Instance
    {
        get
        {
            if (isApplicationQuitting) return null;

            if (instance == null)
            {
                instance = FindInstance() ?? CreateInstance();
            }

            return instance;
        }
    }

    private static bool isApplicationQuitting;

    private static T FindInstance()
    {
        return FindObjectOfType<T>();
    }

    private static T CreateInstance()
    {
        MakeSureManagementSceneIsLoaded();
        var gameObject = new GameObject("(management singleton) " + typeof(T));
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(managementSceneName));
        return gameObject.AddComponent<T>();
    }

    private static void MakeSureManagementSceneIsLoaded()
    {
        if (IsManagementSceneLoaded()) return;

        var currentlyActiveScene = SceneManager.GetActiveScene();
        if (currentlyActiveScene.IsValid())
        {
            while (!currentlyActiveScene.isLoaded) {}
        }

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
