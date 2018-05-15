using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircularMovementState : FSMState<Enemy>
{
    [SerializeField] private float circleRadius = 2;
    [SerializeField] private float heightAbovePlayer = 3;
    [SerializeField] private float newSteeringForce = 70;
    //[SerializeField] private float desiredTangentSpeed = 10;

    private Rigidbody rb;
    private SteeringManager steering;

    public override void Enter()
    {
        base.Enter();
        rb = GetComponent<Rigidbody>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringForce);
        //rb.velocity = new Vector3(4, 0, 4);
    }

    private void FixedUpdate()
    {
        float heightToMaintain = Player.Instance.transform.position.y + heightAbovePlayer;
        if(transform.position.y < heightToMaintain)
        {
            steering.SeekOnYAxis(heightToMaintain);   
        }

        Vector3 towardsCenter = Vector3.Cross(rb.velocity, Vector3.up).normalized;
        float speed = rb.velocity.magnitude;
        rb.AddForce(towardsCenter * (speed * speed) / circleRadius, ForceMode.Acceleration);

        Vector3 tangent = Vector3.Cross(towardsCenter, Vector3.up);
        Vector3 alongTangent = Vector3.Project(rb.velocity, tangent);
        Vector3 desiredVelocityHorizontal = Vector3.ProjectOnPlane(alongTangent, Vector3.up);
        Vector3 forceHorizontal = desiredVelocityHorizontal - Vector3.ProjectOnPlane(rb.velocity, Vector3.up);
        rb.AddForce(forceHorizontal * 40f, ForceMode.Acceleration);

        //float tangentSpeed = alongTangent.magnitude;
        /*if (tangentSpeed < desiredTangentSpeed)
        {
            Vector3 tangentForce = alongTangent.normalized * (desiredTangentSpeed - tangentSpeed);
            rb.AddForce(tangentForce, ForceMode.Acceleration);
        }*/
    }

    public override void Exit()
    {
        base.Exit();
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }
}
