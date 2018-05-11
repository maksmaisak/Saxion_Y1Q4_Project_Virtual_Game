using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager : MonoBehaviour
{

    [SerializeField] private float maxSteeringForce = 5;
    [SerializeField] private float maxVelocity = 6;
    [SerializeField] private float collisionAvoidanceMultiplier = 20;
    [SerializeField] private float collisionAvoidanceRange = 2;
    [SerializeField] private float maxRotationDegreesPerSecond = 180f;
    [SerializeField] private float separationFactor = 1f;
    [SerializeField] private float separationDistance = 5f;

    Vector3 steering = Vector3.zero;
    private Rigidbody rb;
    private GameObject target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, rb.velocity.normalized * collisionAvoidanceRange, Color.red);

        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        rb.AddForce(steering, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

        Debug.DrawRay(transform.position, steering, Color.blue);

        Rotate();

    }

    public void Seek(Vector3 desiredPosition,float maxDistanceToPlayer)
    {
        steering += DoSeek(desiredPosition, maxDistanceToPlayer);
    }
    public void AvoidObstacles()
    {
        steering += DoObstaclesAvoidance();
    }
    public void AvoidEnemies()
    {
        steering += DoEnemyAvoidance();
    }

    public Vector3 DoSeek(Vector3 target, float slowingRadius = 0)
    {
        Vector3 force = Vector3.zero;
        Vector3 desired = Vector3.zero;

        desired = target - this.transform.position;

        float distance = desired.magnitude;

        Vector3.Normalize(desired);

        if (distance <= slowingRadius)
        {
            desired *= maxVelocity * distance / slowingRadius;
        }
        else
        {
            desired *= maxVelocity;
        }

        force = desired - rb.velocity;

        return force;
    }

    public Vector3 DoEnemyAvoidance()
    {
        Vector3 totalForce = Vector3.zero;
        foreach (GameObject enemy in FlockManager.enemyArray)
        {
            if (gameObject != enemy && Vector3.Distance(transform.position, enemy.transform.position) <= separationDistance)
            {
                Vector3 headingVector = transform.position - enemy.transform.position;
                totalForce += headingVector;
            }
        }
        totalForce *= separationFactor;
        return totalForce;
    }


    public Vector3 DoObstaclesAvoidance()
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


    private void Rotate()
    {
        Quaternion rotation;
        if (rb.velocity != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        }
        else
        {
            rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        }

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, maxRotationDegreesPerSecond * Time.deltaTime));
    }
}
