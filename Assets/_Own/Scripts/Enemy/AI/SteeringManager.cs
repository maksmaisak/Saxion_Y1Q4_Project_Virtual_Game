using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringManager 
{

    public IBOID host;

    public SteeringManager(IBOID host)
    {
        this.host = host;
    }

    public Vector3 DoSeek(Vector3 target, float slowingRadius = 0)
    {
        Vector3 force = Vector3.zero;
        Vector3 desired = Vector3.zero;

        desired = target - host.GetPosition();

        float distance = desired.magnitude;

        Vector3.Normalize(desired);

        if (distance <= slowingRadius)
        {
            desired *= host.GetMaxVelocity() * distance / slowingRadius;
        }
        else
        {
            desired *= host.GetMaxVelocity();
        }

        force = desired - host.GetVelocity();

        return force;
    }

    public Vector3 DoEnemyAvoidance()
    {
        Vector3 totalForce = Vector3.zero;
        foreach (GameObject enemy in FlockManager.enemyArray)
        {
            if (host.GetEnemy() != enemy && Vector3.Distance(host.GetPosition(), enemy.transform.position) <= host.GetSeparationDistance())
            {
                Vector3 headingVector = host.GetPosition() - enemy.transform.position;
                totalForce += headingVector;
            }
        }
        totalForce *= host.GetSeparationFactor();
        return totalForce;
    }

    
    public Vector3 DoObstaclesAvoidance()
    {
        Vector3 force = Vector3.zero;

        RaycastHit hit;
        RaycastHit hitForward;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        if (Physics.SphereCast(host.GetPosition(), 2, host.GetVelocity().normalized, out hit, 0.2f))
        {
            if (hit.transform != host.GetEnemy().transform && hit.transform.tag != "Player")
            {
                force += host.GetCollisionAvoidanceMultiplier() * hit.normal;
            }
        }

        if (Physics.SphereCast(host.GetPosition(), 0.25f, host.GetVelocity().normalized, out hitForward, host.GetCollisionAvoidanceRange()))
        {
            if (hitForward.transform != host.GetEnemy().transform && hitForward.transform.tag != "Player")
            {
                force += host.GetCollisionAvoidanceMultiplier() * hitForward.normal;
            }
        }

        if (Physics.SphereCast(host.GetPosition(), 0.25f, host.GetEnemy().transform.right, out hitRight, host.GetCollisionAvoidanceRange()))
        {
            if (hitRight.transform != host.GetEnemy().transform && hitRight.transform.tag != "Player")
            {
                force += host.GetCollisionAvoidanceMultiplier() * hitRight.normal;
            }
        }

        if (Physics.SphereCast(host.GetPosition(), 0.25f, -host.GetEnemy().transform.right, out hitLeft, host.GetCollisionAvoidanceRange()))
        {
            if (hitLeft.transform != host.GetEnemy().transform && hitLeft.transform.tag != "Player")
            {
                force += host.GetCollisionAvoidanceMultiplier() * hitLeft.normal;
            }
        }

        return force;
    }


}
