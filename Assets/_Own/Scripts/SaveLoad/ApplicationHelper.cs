using UnityEngine;
using System.Collections;

public class ApplicationHelper : PersistentSingleton<ApplicationHelper>
{
    public bool isApplicationQuitting { get; private set; }

    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }
}
