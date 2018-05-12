using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByForce : MonoBehaviour {

    [SerializeField] private float forceNeededToDestroy = 500;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("wall"))
        {
            Vector3 force = collision.impulse / Time.fixedDeltaTime;

            Debug.Log(force.magnitude);

            if (force.magnitude >= forceNeededToDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
   
}
