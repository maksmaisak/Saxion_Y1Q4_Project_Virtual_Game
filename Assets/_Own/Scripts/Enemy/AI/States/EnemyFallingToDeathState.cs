using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallingToDeathState : FSMState<Enemy> {

    private Rigidbody rb;
    private Health health;

	public override void Enter()
    {
        base.Enter();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    public override void Exit()
    {
        base.Exit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != gameObject)
        {
            health.DealDamage(100);
        }
    }
}
