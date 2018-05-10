using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exposes events which fire when the gameobject is grappled or released by grapples.
/// OnGrappled is called for every grapple grappling it, 
/// OnReleased is called when all grapples are released.
/// </summary>
public class Grappleable : MonoBehaviour
{
    public event Action<Grappleable> OnGrappled;
    public event Action<Grappleable> OnReleased;

    private int numConnectedGrapples = 0;
    public bool isGrappled { get { return numConnectedGrapples > 0; } }

    public void Grapple()
    {
        Debug.Log("Grappled");
        numConnectedGrapples += 1;

        if (OnGrappled != null)
        {
            OnGrappled(this);
        }
    }

    public void Release()
    {
        if (numConnectedGrapples <= 0) {
            
            Debug.LogError("[Grappleable]: Can't Release a non-grappled grappleable!");
            return;
        }

        numConnectedGrapples -= 1;

        if (numConnectedGrapples == 0 && OnReleased != null)
        {
            Debug.Log("Released");
            OnReleased(this);
        }
    }
}
