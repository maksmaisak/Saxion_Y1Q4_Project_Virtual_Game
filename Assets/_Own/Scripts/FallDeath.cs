using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class FallDeath : MonoBehaviour {

    [SerializeField] private float fallDeathYPos = -50;
	// Update is called once per frame
	void Update () 
    {
        if(transform.position.y <= fallDeathYPos)
        {
            GetComponent<Health>().DealDamage(100);
        }
	}
}
