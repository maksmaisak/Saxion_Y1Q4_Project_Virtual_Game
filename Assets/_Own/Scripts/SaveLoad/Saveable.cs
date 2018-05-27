using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class Saveable : MonoBehaviour
{
    [Tooltip("DO NOT CHANGE THIS")]
    [SerializeField] private string stringGuid;

    void Start()
    {
        CheckGuid();
    }

    void OnDestroy()
    {
        GlobalIdManager.Instance.Unregister(stringGuid);
    }

    [ContextMenu("MakeGuidsUnique")]
    private void MakeGuidsUnique()
    {
        foreach (var saveable in FindObjectsOfType<Saveable>())
        {
            saveable.CheckGuid();
        }
    }

    private void CheckGuid()
    {
        if (string.IsNullOrEmpty(stringGuid))
        {
            AssignNewGuid();
        }

        int registeredInstanceId;
        bool hasRegistered = GlobalIdManager.Instance.GetRegistered(stringGuid, out registeredInstanceId);
        if (!hasRegistered)
        {
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
            Debug.Log("Guid not registered! Registered: " + stringGuid);
        }
        else if (registeredInstanceId != GetInstanceID())
        {
            string oldGuid = stringGuid;
            AssignNewGuid();
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
            Debug.Log("Duplicate guid (" + oldGuid + ")! Created: " + stringGuid);
        }
    }

    public void SaveData<T>(T data)
    {
        var manager = GlobalIdManager.Instance;
        if (manager == null) return;
        manager.SaveData(stringGuid, data);
    }

    public bool GetSavedData<T>(out T data)
    {
        return GlobalIdManager.Instance.GetSavedData(stringGuid, out data);
    }

    private void AssignNewGuid()
    {
        stringGuid = Guid.NewGuid().ToString();
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
}
