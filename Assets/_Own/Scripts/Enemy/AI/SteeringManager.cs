using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour
{
    [SerializeField] private float maxSteeringForce = 10;
    [SerializeField] private float maxSpeed = 6;
    [SerializeField] private float collisionAvoidanceMultiplier = 20;
    [SerializeField] private float collisionAvoidanceRange = 2;
    [SerializeField] private float maxRotationDegreesPerSecond = 180f;
    [SerializeField] private float separationFactor = 1f;
    [SerializeField] private float separationDistance = 5f;
    [SerializeField] private float circleRadius = 2;
    [SerializeField] private float circleDistance = 2;
    [SerializeField] private float wanderAngle = 20;

    private Vector3 displacement;
    private float initialSteeringForce;
    private Vector3 steering = Vector3.zero;
    private Rigidbody rb;

    private Vector3 previousVelocity;
    private Vector3 previousSteering;

    public Vector3 velocity 
    {
        get 
        {
            return rb.velocity;
        }
    }

    private void Start()
    {
        initialSteeringForce = maxSteeringForce;
        rb = GetComponent<Rigidbody>();
        displacement = Vector3.forward * circleRadius;
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, rb.velocity, Color.red);

        if (maxSteeringForce > 0f)
        {
            steering = Vector3.ClampMagnitude(steering, maxSteeringForce);
        }
        Debug.DrawRay(transform.position, steering, Color.blue);

        rb.AddForce(steering, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        previousVelocity = rb.velocity;
        previousSteering = steering;

        steering = Vector3.zero;
    }

    public void Seek(Vector3 desiredPosition, float slowingRadius = 0f)
    {
        steering += DoSeek(desiredPosition, slowingRadius);
    }

    public void Flee(Vector3 targetToFlee)
    {
        steering += DoFlee(targetToFlee);
    }

    public void SeekOnYAxis(float targetHeight)
    {
        steering += DoSeekOnYAxis(targetHeight);
    }

    public void AvoidObstacles()
    {
        steering += DoObstaclesAvoidance();
    }

    public void FlockingSeparation(IEnumerable<SteeringManager> others)
    {
        steering += DoFlockingSeparation(others);
    }

    public void ThrustUp(float thrustStrength)
    {
        steering += DoThrustUp(thrustStrength);
    }

    public void CompensateExternalForces()
    {
        steering += DoCompensateExternalForces();
    }

    public void Custom(Vector3 customSteeringForce)
    {
        steering += customSteeringForce;
    }


    private Vector3 DoThrustUp(float thrustStrength)
    {
        return new Vector3(0f, thrustStrength, 0f);
    }

    private Vector3 DoSeek(Vector3 target, float slowingRadius = 0f)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 desiredVelocity = toTarget.normalized * maxSpeed;
        float distance = toTarget.magnitude;

        if (distance <= slowingRadius)
        {
            desiredVelocity *= distance / slowingRadius;
        }

        Vector3 force = desiredVelocity - rb.velocity;
        return force;
    }

    private Vector3 DoFlee(Vector3 target)
    {
        Vector3 fromTarget = transform.position - target;
        Vector3 desiredVelocity = fromTarget.normalized * maxSpeed;
        Vector3 force = desiredVelocity - rb.velocity;
        return force;
    }

    private Vector3 DoSeekOnYAxis(float targetHeight)
    {
        float desiredVelocityY = Mathf.Sign(targetHeight - transform.position.y) * maxSpeed;
        Vector3 force = Vector3.up * (desiredVelocityY - rb.velocity.y);

        return force;
    }

    private Vector3 DoFlockingSeparation(IEnumerable<SteeringManager> others)
    {
        Vector3 totalForce = Vector3.zero;
        float sqrSeparationDistance = separationDistance * separationDistance;

        foreach (SteeringManager other in others)
        {
            if (this == other) continue;

            Vector3 position = transform.position;
            Vector3 otherPosition = other.transform.position;

            Vector3 delta = position - otherPosition;
            if (delta.sqrMagnitude <= sqrSeparationDistance)
            {
                totalForce += position - otherPosition;
            }
        }

        totalForce *= separationFactor;

        return totalForce;
    }

    private Vector3 DoObstaclesAvoidance()
    {
        Vector3 force = Vector3.zero;

        RaycastHit hit;
        RaycastHit hitForward;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        if (Physics.SphereCast(transform.position, 2, rb.velocity.normalized, out hit, 0.2f))
        {
            if (hit.transform != this.transform && hit.transform.tag != "Player")
            {
                force += collisionAvoidanceMultiplier * hit.normal;
            }
        }

        if (Physics.SphereCast(transform.position, 0.25f, rb.velocity.normalized, out hitForward, collisionAvoidanceRange))
        {
            if (hitForward.transform != this.transform && hitForward.transform.tag != "Player")
            {
                force += collisionAvoidanceMultiplier * hitForward.normal;
            }
        }

        if (Physics.SphereCast(transform.position, 0.25f, this.transform.right, out hitRight, collisionAvoidanceRange))
        {
            if (hitRight.transform != this.transform && hitRight.transform.tag != "Player")
            {
                force += collisionAvoidanceMultiplier * hitRight.normal;
            }
        }

        if (Physics.SphereCast(transform.position, 0.25f, -this.transform.right, out hitLeft, collisionAvoidanceRange))
        {
            if (hitLeft.transform != this.transform && hitLeft.transform.tag != "Player")
            {
                force += collisionAvoidanceMultiplier * hitLeft.normal;
            }
        }

        return force;
    }

    private Vector3 DoCompensateExternalForces()
    {
        Vector3 acceleration = rb.velocity - previousVelocity;
        Vector3 externalAcceleration = acceleration - previousSteering;
        return -externalAcceleration;
    }

    public void LookWhereGoing()
    {
        if (rb.velocity != Vector3.zero)
        {
            SmoothRotateTowards(Quaternion.LookRotation(rb.velocity));
        }
    }

    public void LookAt(Vector3 targetPosition)
    {
        var targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
        SmoothRotateTowards(targetRotation);
    }

    public void SmoothRotateTowards(Quaternion targetRotation)
    {
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDegreesPerSecond * Time.deltaTime));
    }

    public void SetMaxSteeringForce(float newSteeringForce)
    {
        maxSteeringForce = newSteeringForce;
    }

    public float GetInitalSteeringForce()
    {
        return initialSteeringForce;
    }

    public void Wander()
    {
        steering += DoWander();
    }

    private Vector3 DoWander()
    {
        Vector3 circleCenter = rb.velocity.normalized;
        circleCenter *= circleDistance;

        Vector3 changeOfRotationEulerAngles = new Vector3(Random.Range(-wanderAngle, wanderAngle), Random.Range(-wanderAngle, wanderAngle), Random.Range(-wanderAngle, wanderAngle)) * Time.fixedDeltaTime;
        Quaternion changeOfRotation = Quaternion.Euler(changeOfRotationEulerAngles);

        Quaternion newRotation =  Quaternion.LookRotation(displacement) * changeOfRotation;

        float angle = 0;
        newRotation.ToAngleAxis(out angle, out displacement);
        displacement *= circleRadius;

        return DoSeek(rb.position + circleCenter + transform.TransformVector(displacement));
    }

}

