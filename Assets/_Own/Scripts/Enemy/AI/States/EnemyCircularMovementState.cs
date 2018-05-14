using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircularMovementState : FSMState<Enemy>
{
    [SerializeField] private float rotationForceMultiplier = 4;
    private Rigidbody rb;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 normalOfVelocity = Vector3.Cross(rb.velocity, Vector3.up).normalized;
        rb.AddForce(normalOfVelocity * rotationForceMultiplier, ForceMode.Acceleration);
    }

    public override void Exit()
    {
        base.Exit();
    }
}
