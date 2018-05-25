using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Saveable : MonoBehaviour
{
    [Tooltip("DO NOT CHANGE THIS")]
    [SerializeField] private string stringGuid;

    private Guid _guid;
    public Guid guid
    {
        get
        {
            if (_guid == Guid.Empty || string.IsNullOrEmpty(stringGuid))
            {
                if (!string.IsNullOrEmpty(stringGuid))
                {
                    _guid = new Guid(stringGuid);
                }
                else
                {
                    _guid = Guid.NewGuid();
                    stringGuid = _guid.ToString();
                }
            }

            return _guid;
        }
        private set
        {
            _guid = value;
            stringGuid = _guid.ToString();
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
        GlobalIdManager.Instance.Unregister(guid);
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
        int registeredInstanceId;
        bool hasRegistered = GlobalIdManager.Instance.GetRegistered(guid, out registeredInstanceId);
        if (!hasRegistered) 
        {
            GlobalIdManager.Instance.Register(guid, GetInstanceID());
        }
        else if (registeredInstanceId != GetInstanceID())
        {
            guid = Guid.NewGuid();
            GlobalIdManager.Instance.Register(guid, GetInstanceID());
        }
    }

    public void SaveData<T>(T data)
    {
        var manager = GlobalIdManager.Instance;
        if (manager == null) return;
        manager.SaveData(guid, data);
    }

    public bool GetSavedData<T>(out T data)
    {
        return GlobalIdManager.Instance.GetSavedData(guid, out data);
    }
}
