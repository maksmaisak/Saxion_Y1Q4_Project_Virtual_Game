using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //steering.Wander();
        steering.Flee(Player.Instance.transform.position);
        steering.LookWhereGoing();
        steering.AvoidEnemies();
        steering.AvoidObstacles();
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }
}
