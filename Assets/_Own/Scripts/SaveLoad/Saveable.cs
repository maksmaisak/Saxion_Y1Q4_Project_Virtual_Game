using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class Saveable : MonoBehaviour
{
    [Tooltip("DO NOT CHANGE THIS")]
    [SerializeField] string stringGuid;

    void Start()
    {
        CheckGuid();
    }

    void OnDestroy()
    {
        if (GlobalIdManager.Instance != null)
        {
            GlobalIdManager.Instance.Unregister(stringGuid);
        }
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
        if (/*string.IsNullOrEmpty(stringGuid)*/ !Application.isPlaying)
        {
            AssignNewGuid();
        }

        int registeredInstanceId;
        bool hasRegistered = GlobalIdManager.Instance.GetRegistered(stringGuid, out registeredInstanceId);
        if (!hasRegistered)
        {
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
            //Debug.Log(gameObject.name + ": Guid not registered! Registered: " + stringGuid);
        }
        else if (registeredInstanceId != GetInstanceID())
        {
            string oldGuid = stringGuid;
            AssignNewGuid();
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
            Debug.Log(gameObject.name + "Duplicate guid (" + oldGuid + ")! Created: " + stringGuid);
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
        //Debug.Log(gameObject.name + " assigned new guid: " + stringGuid);
        if (!Application.isPlaying)
        {
            // Note: SetDirty has to be used here even though the documentation 
            // doesn't recommend it. Without it this object won't be affected
            // when you save the scene. Kinda makes sense, 
            // now that I think about it.
            UnityEditor.EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
        else
        {
            Debug.LogError(gameObject.name + " assigned a new guid while not in edit mode! This should never happen! New guid: " + stringGuid);
        }
    }
}
