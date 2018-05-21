using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyFallingToDeathState : FSMState<Enemy> 
{
    private Rigidbody rb;
    private Health health;
    private ParticleManager particleManager;
    private SteeringManager steeringManager;

	public override void Enter()
    {
        base.Enter();

        particleManager = GetComponentInChildren<ParticleManager>();
        particleManager.SwitchToFalling();

        health = agent.health;
        rb = GetComponent<Rigidbody>();
        steeringManager = agent.steering;
        rb.useGravity = true;

        StartCoroutine(WhileFallingScreamCoroutine());
    }

    void FixedUpdate()
    {
        steeringManager.LookWhereGoing();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != gameObject && collision.gameObject.layer != 10) // FIXME THIS IS HORRIBLE! DON'T hardcode layer numbers jesus...
        {
            if (health != null)
            {
                health.DealDamage(100);
            }
        }
    }

    IEnumerator WhileFallingScreamCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            agent.audio.PlayScreamWhileFallingToDeath();
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        rb.useGravity = false;

        StopAllCoroutines();
    }
}
