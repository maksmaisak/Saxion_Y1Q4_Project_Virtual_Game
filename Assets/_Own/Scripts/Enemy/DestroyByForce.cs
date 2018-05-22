using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestroyByForce : MonoBehaviour {

    [SerializeField] private float forceNeededToDestroy = 200;
    [SerializeField] LayerMask unaffectedByCollisionsWith = 0;

    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & unaffectedByCollisionsWith) != 0) return;
        float impulseValue = collision.impulse.magnitude / Time.fixedDeltaTime;

        if (impulseValue >= forceNeededToDestroy)
        {
            Debug.Log("Damaged with impulse: " + impulseValue);
            health.SetHealth(0);
        }
    }
   
}
