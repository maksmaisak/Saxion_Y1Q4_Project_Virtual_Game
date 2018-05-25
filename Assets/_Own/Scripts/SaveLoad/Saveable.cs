using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Saveable : MonoBehaviour
{
    [Tooltip("DO NOT CHANGE THIS")]
    [SerializeField] private string stringGuid;

    public Guid guid
    {
        get
        {
            if (string.IsNullOrEmpty(stringGuid))
            {
                var temp = Guid.NewGuid();
                stringGuid = temp.ToString();
                return temp; 
            }
            return new Guid(stringGuid);
        }
        private set
        {
            stringGuid = value.ToString();
        }
    }

    void Awake()
    {
        CheckGuid();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            CheckGuid();
        }
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
            stringGuid = Guid.NewGuid().ToString();
        }

        int registeredInstanceId;
        bool hasRegistered = GlobalIdManager.Instance.GetRegistered(stringGuid, out registeredInstanceId);
        if (!hasRegistered)
        {
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
        }
        else if (registeredInstanceId != GetInstanceID())
        {
            stringGuid = Guid.NewGuid().ToString();
            GlobalIdManager.Instance.Register(stringGuid, GetInstanceID());
            Debug.Log("Duplicate!");
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
}
