using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyPullPlayerState : FSMState<Enemy> {

    [SerializeField] private float newSteeringForce;
    private SteeringManager steering;

	public override void Enter()
    {
        base.Enter();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringForce);
    }

    private void FixedUpdate()
    {
        steering.Wander();
        steering.Flee(Player.Instance.transform.position);
        steering.AvoidEnemies();
        steering.AvoidObstacles();
        steering.CompensateExternalForces();

        steering.LookWhereGoing();
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }
}
