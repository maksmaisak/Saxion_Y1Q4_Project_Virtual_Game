using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShakeState : FSMState<Enemy>
{
    [SerializeField] private float maxShakingForce = 5;
    [SerializeField] private float maxSteeringForce = 1000f;

    private SteeringManager steering;
    new private Rigidbody rigidbody;

    public override void Enter()
    {
        base.Enter();
        rigidbody = agent.rigidbody;
        steering  = agent.steering;

        steering.SetMaxSteeringForce(maxSteeringForce);
    }

    void FixedUpdate()
    {
        Shake();
        steering.FlockingSeparation(Enemy.allAsSteerables);
        steering.AvoidObstacles();
        steering.CompensateExternalForces();
        steering.LookWhereGoing();
    }

    private void Shake()
    {
        rigidbody.AddForce(Random.onUnitSphere * maxShakingForce);
    }

    public override void Exit()
    {
        base.Exit();

        rigidbody.useGravity = false;
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }
}
