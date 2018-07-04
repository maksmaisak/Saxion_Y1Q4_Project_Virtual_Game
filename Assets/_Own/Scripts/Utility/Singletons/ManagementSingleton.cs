using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System;

public abstract class ManagementSingleton<T> : MonoBehaviour where T : ManagementSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                if (Application.isPlaying && isApplicationQuitting) 
                {
                    Debug.Log("Returning null because of isApplicationQuitting");
                    return null;
                }

                Management.MakeSureManagementIsPresentInTheScene();
                instance = FindInstance();

                if (instance == null)
                {
                    Debug.LogError(typeof(T).Name + " is not present under the management gameobject!");
                }
            }

            return instance;
        }
    }

    private static bool isApplicationQuitting;

    private static T FindInstance()
    {
        return FindObjectOfType<T>();
    }

    protected virtual void OnDestroy()
    {
        if (instance == this && Application.isPlaying)
        {
            isApplicationQuitting = true;
        }
    }
}
