using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrappledState : FSMState<Enemy>
{
    [SerializeField] private float upThrustStrength = 300;
    private Shooting shooting;
    private Rigidbody rb;
    private SteeringManager steering;


    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        shooting = GetComponent<Shooting>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(200);
        shooting.enabled = false;

    }


    void FixedUpdate()
    {
        steering.ThrustUp(upThrustStrength);
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(5);
        shooting.enabled = true;
    }


}
