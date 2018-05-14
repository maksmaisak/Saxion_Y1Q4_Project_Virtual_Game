using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyThrustUpState : FSMState<Enemy>
{
    [SerializeField] private float thrustHeightLimit = 20;
    [SerializeField] private float upThrustStrength = 300;
    [SerializeField] private float thrustTime = 5;
    [SerializeField] private float newSteeringStrength = 200;

    private float counter = 0;
    private bool isThrusting;
    private SteeringManager steering;
    private Enemy enemyComponent;

    public override void Enter()
    {
        base.Enter();
        steering = GetComponent<SteeringManager>();
        enemyComponent = GetComponent<Enemy>();
        steering.SetMaxSteeringForce(newSteeringStrength);
        isThrusting = true;
    }

    void FixedUpdate()
    {
        if (isThrusting && transform.position.y <= thrustHeightLimit + enemyComponent.GetInitialHeight())
        {
            steering.ThrustUp(upThrustStrength);
            counter += Time.fixedDeltaTime;
            if (counter >= thrustTime)
            {
                isThrusting = false;
                counter = 0;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }
}
