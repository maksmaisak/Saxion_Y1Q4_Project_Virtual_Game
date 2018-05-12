using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyThrustUpState : FSMState<Enemy>
{  
    [SerializeField] private float upThrustStrength = 300;
    [SerializeField] private float thrustTime = 5;
    [SerializeField] private float newSteeringStrength = 200;

    private float counter = 0;
    private bool isThrusting;
    private Rigidbody rb;
    private SteeringManager steering;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringStrength);
        isThrusting = true;
    }

    void FixedUpdate()
    {
        if (isThrusting)
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
