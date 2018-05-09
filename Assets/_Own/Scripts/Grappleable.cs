using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappleable : MonoBehaviour
{
    public event Action<Grappleable> OnGrappled;
    public event Action<Grappleable> OnReleased;

    public void Grapple()
    {
        if (OnGrappled != null) OnGrappled(this);
    }

    public void Release()
    {
        if (OnReleased != null) OnReleased(this);
    }
}
