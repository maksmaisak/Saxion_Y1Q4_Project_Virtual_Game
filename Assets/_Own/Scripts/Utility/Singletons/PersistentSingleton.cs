using UnityEngine;
using System.Collections;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
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
        var gameObject = new GameObject("(Persistent Singleton) " + typeof(T));
        DontDestroyOnLoad(gameObject);
        return gameObject.AddComponent<T>();
    }

    protected virtual void OnDestroy()
    {
        isApplicationQuitting = true;
    }
}