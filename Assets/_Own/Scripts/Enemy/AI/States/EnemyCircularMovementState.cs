using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircularMovementState : FSMState<Enemy>
{
    [SerializeField] private float speed = 4;
    [SerializeField] private float circleRadius = 2;
    private Rigidbody rb;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 towardsCenter = Vector3.Cross(rb.velocity, Vector3.up).normalized;
        rb.AddForce(towardsCenter * (speed * speed)/circleRadius, ForceMode.Acceleration);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
