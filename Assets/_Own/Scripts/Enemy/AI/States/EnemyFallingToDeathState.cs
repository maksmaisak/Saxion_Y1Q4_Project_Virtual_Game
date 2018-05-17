using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallingToDeathState : FSMState<Enemy> {

    [SerializeField] private GameObject fallingParticleGroup;

    private Rigidbody rb;
    private Health health;
    private ParticleManager particleManager;

	public override void Enter()
    {
        base.Enter();
        particleManager = GetComponentInChildren<ParticleManager>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();
        particleManager.ChangeParticleGroup(fallingParticleGroup);
        rb.useGravity = true;
    }

    public override void Exit()
    {
        base.Exit();
        rb.useGravity = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != gameObject && collision.gameObject.layer != 10)
        {
            if (health != null)
            {
                health.DealDamage(100);
            }
        }
    }
}
