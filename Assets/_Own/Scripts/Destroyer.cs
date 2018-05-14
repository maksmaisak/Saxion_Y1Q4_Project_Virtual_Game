using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();

    }

    // Update is called once per frame
    void Update () {
		
        if(Input.GetKeyDown(KeyCode.F))
        {
            health.DealDamage(100);
        }
	}
}
