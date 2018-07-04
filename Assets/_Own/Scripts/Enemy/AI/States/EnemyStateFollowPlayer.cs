
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyStateFollowPlayer : FSMState<Enemy>
{
    [SerializeField] private float maxDistanceToPlayer = 5f;
    [SerializeField] private float arriveSlowdownDistance = 5f;
    [SerializeField] private float lookAtPlayerDistance = 10f;
    [SerializeField] private float targetHeightAbovePlayer = 1f;

    private ParticleManager particleManager;
    private GameObject target;
    private Shooting shooting;
    private SteeringManager steeringManager;

    void Start()
    {
        shooting = GetComponent<Shooting>();
        shooting.enabled = true;
        target = Player.Instance.gameObject;
        steeringManager = GetComponent<SteeringManager>();
        particleManager = GetComponentInChildren<ParticleManager>();
        particleManager.SwitchActive();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 fromTarget = transform.position - targetPosition;
        Vector3 offset = Vector3.ProjectOnPlane(fromTarget.normalized, Vector3.up).normalized * maxDistanceToPlayer + Vector3.up * targetHeightAbovePlayer;
        Vector3 desiredPos = targetPosition + offset;
        
        steeringManager.Seek(desiredPos, arriveSlowdownDistance);
        steeringManager.FlockingSeparation(Enemy.allAsSteerables);
        steeringManager.AvoidObstacles();
        steeringManager.Wander();

        if (fromTarget.magnitude > lookAtPlayerDistance)
        {
            steeringManager.LookWhereGoing();
        }
        else
        {
            steeringManager.LookAt(target.transform.position);
        }
    }
    
    public override void Exit()
    {
        base.Exit();

        shooting = shooting ? shooting : GetComponent<Shooting>();
        if (shooting)
        {
            shooting.enabled = false;
        }
    }
}

