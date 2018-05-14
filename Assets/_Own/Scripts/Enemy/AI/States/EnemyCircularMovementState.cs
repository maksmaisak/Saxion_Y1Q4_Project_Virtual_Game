using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircularMovementState : FSMState<Enemy>
{
    [SerializeField] private float circleRadius = 2;
    [SerializeField] private float heightAbovePlayer = 3;
    private Rigidbody rb;
    private Vector3 heightToMaintain;
    private SteeringManager steering;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        steering = GetComponent<SteeringManager>();
        heightToMaintain = new Vector3(0, Player.Instance.transform.position.y + heightAbovePlayer, 0);
    }

    private void FixedUpdate()
    {
        Vector3 towardsCenter = Vector3.Cross(rb.velocity, Vector3.up).normalized;
        float speed = rb.velocity.magnitude;
        rb.AddForce(towardsCenter * (speed * speed)/circleRadius, ForceMode.Acceleration);
       
        if (transform.position.y <= Player.Instance.transform.position.y + heightAbovePlayer)
        {
            steering.ThrustUp(100);
        }
      
    }

    public override void Exit()
    {
        base.Exit();
    }
}
