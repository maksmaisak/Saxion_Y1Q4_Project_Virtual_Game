using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByForce : MonoBehaviour {

    [SerializeField] private float forceNeededToDestroy = 500;

    private void OnCollisionEnter(Collision collision)
    {
        // FIXME String comparisons are slooow. Use .CompareTag instead
        if (collision.transform.tag == "Player" || collision.transform.tag == "wall")
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
