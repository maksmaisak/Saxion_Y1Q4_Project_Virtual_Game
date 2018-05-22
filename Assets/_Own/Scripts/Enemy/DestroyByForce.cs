using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByForce : MonoBehaviour {

    [SerializeField] private float forceNeededToDestroy = 200;
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 impulse = collision.impulse / Time.fixedDeltaTime;

        Debug.Log(impulse.magnitude);

        if (impulse.magnitude >= forceNeededToDestroy)
        {
            Debug.Log("Damaged with impulse: " + impulse.magnitude);
            health.DealDamage(100);
        }
    }
   
}
