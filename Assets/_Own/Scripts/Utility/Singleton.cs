using UnityEngine;
using System.Collections;
using System;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindInstance() ?? CreateInstance();
            }

            return _instance;
        }
    }

    private static T FindInstance()
    {
        return FindObjectOfType<T>();
    }

    private static T CreateInstance()
    {
        var gameObject = new GameObject();
        gameObject.name = typeof(T).ToString();
        return gameObject.AddComponent<T>();
    }
}
