
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFollowPlayer : FSMState<Enemy>
{
    [SerializeField] private float maxDistanceToPlayer = 5f;
    [SerializeField] private float arriveSlowdownDistance = 5f;
    [SerializeField] private float lookAtPlayerDistance = 10f;

    private Rigidbody rb;
    private GameObject target;
    private Shooting shootingController;
    private SteeringManager steeringManager;

    void Start()
    {
        shootingController = GetComponent<Shooting>();
        shootingController.enabled = true;
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        steeringManager = GetComponent<SteeringManager>();

    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 toPlayer = target.transform.position - transform.position;
            Vector3 desiredPos = target.transform.position - toPlayer.normalized * maxDistanceToPlayer;


            steeringManager.Seek(desiredPos, arriveSlowdownDistance);
            steeringManager.AvoidEnemies();
            steeringManager.AvoidObstacles();

            if (toPlayer.magnitude > lookAtPlayerDistance)
            {
                steeringManager.LookWhereGoing();
            }
            else
            {
                steeringManager.LookAt(target.transform.position);
            }
        }
    }

}

