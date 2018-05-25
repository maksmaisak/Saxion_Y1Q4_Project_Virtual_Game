using UnityEngine;
using System.Collections;

public class SimpleSingleton<T> where T : class, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance = instance ?? new T();
        }
    }
}
