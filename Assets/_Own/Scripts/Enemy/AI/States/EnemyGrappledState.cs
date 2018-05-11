using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrappledState : FSMState<Enemy>
{
    [SerializeField] private float upThrustStrength = 300;
    [SerializeField] private float ThrustTime = 5;
    [SerializeField] private float newSteeringStrength = 200;

    private float counter = 0;
    private bool thrusting = false;

    private Shooting shooting;
    private Rigidbody rb;
    private SteeringManager steering;


    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        shooting = GetComponent<Shooting>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringStrength);
        shooting.enabled = false;
        thrusting = true;
    }


    void Update()
    {
        if (thrusting)
        {
            steering.ThrustUp(upThrustStrength);
            counter += Time.deltaTime;
            if(counter >= ThrustTime)
            {
                thrusting = false;
                counter = 0;
            }
        }
        Debug.Log(thrusting);
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
        shooting.enabled = true;
    }


}
