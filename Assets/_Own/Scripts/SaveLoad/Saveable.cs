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
            if (_guid == Guid.Empty)
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

    protected virtual void Awake()
    {
        if (GlobalIdManager.Instance.IsAlreadyRegistered(guid))
        {
            guid = Guid.NewGuid();
            GlobalIdManager.Instance.Register(guid, this);
        }
    }

    protected virtual void OnDestroy()
    {
        GlobalIdManager.Instance.Unregister(guid);
    }

    public void SaveData<T>(T data)
    {
        GlobalIdManager.Instance.SaveData(guid, data);
    }

    public bool GetSavedData<T>(out T data)
    {
        return GlobalIdManager.Instance.GetSavedData(guid, out data);
    }
}
