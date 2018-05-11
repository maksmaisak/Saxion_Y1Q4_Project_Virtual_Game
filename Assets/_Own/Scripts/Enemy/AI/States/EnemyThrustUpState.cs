using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyThrustUpState : FSMState<Enemy>
{  
    [SerializeField] private float upThrustStrength = 300;
    [SerializeField] private float ThrustTime = 5;
    [SerializeField] private float newSteeringStrength = 200;

    private float counter = 0;
    private bool thrustingBehaviour;
    private Rigidbody rb;
    private SteeringManager steering;


    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringStrength);
        thrustingBehaviour = true;
    }


    void Update()
    {
        if (thrustingBehaviour)
        {
            steering.ThrustUp(upThrustStrength);
            counter += Time.deltaTime;
            if (counter >= ThrustTime)
            {
                thrustingBehaviour = false;
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
