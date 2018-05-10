using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Exposes events which fire when the gameobject is grappled or released by grapples.
/// OnGrappled is called for every grapple grappling it, 
/// OnUngrappled is called for every grapple ungrappling it,
/// OnReleased is called when all grapples are released.
/// 
/// So if there are two grapples connected to this, when one of them ungrapples,
/// OnUngrappled is called, but OnReleased isn't, because there's still 
/// the other grapple connected to the gameobject.
/// </summary>
public class Grappleable : MonoBehaviour
{
    public event Action<Grappleable> OnGrappled;
    public event Action<Grappleable> OnUngrappled;
    public event Action<Grappleable> OnReleased;

    private int numConnectedGrapples = 0;
    public bool isGrappled { get { return numConnectedGrapples > 0; } }

    public void Grapple()
    {
        //Debug.Log("Grappled");
        numConnectedGrapples += 1;

        if (OnGrappled != null) OnGrappled(this);
    }

    public void Ungrapple()
    {
        if (numConnectedGrapples <= 0)
        {

            Debug.LogError("[Grappleable]: Can't Ungrapple a non-grappled grappleable!");
            return;
        }

        numConnectedGrapples -= 1;

        //Debug.Log("Ungrappled");
        if (OnUngrappled != null) OnUngrappled(this);

        if (numConnectedGrapples == 0)
        {
            //Debug.Log("Released");
            if (OnReleased != null) OnReleased(this);
        }
    }
}
