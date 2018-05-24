using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobalId : MonoBehaviour
{
    public Guid guid;

    void Awake()
    {
        if (guid == Guid.Empty || GlobalIdMananger.Instance.IsAlreadyRegistered(guid))
        {
            guid = Guid.NewGuid();
            GlobalIdMananger.Instance.Register(guid, this);
        }
    }

    void OnDestroy()
    {
        GlobalIdMananger.Instance.Unregister(guid);
    }
}
