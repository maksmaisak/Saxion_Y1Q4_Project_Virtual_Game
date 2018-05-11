
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateFollowPlayer : FSMState<Enemy>
{
    [SerializeField] private float maxDistanceToPlayer = 5;

    private Rigidbody rb;
    private GameObject target;
    private ShootingController shootingController;
    private SteeringManager steeringManager;

    void Start()
    {
        shootingController = GetComponent<ShootingController>();
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        steeringManager = GetComponent<SteeringManager>();
    }

    private void FixedUpdate()
    {
        Vector3 desiredPos = Vector3.zero;
        desiredPos = target.transform.position + (transform.position - target.transform.position).normalized * maxDistanceToPlayer;
         
        steeringManager.Seek(desiredPos,maxDistanceToPlayer);
        steeringManager.AvoidEnemies();
        steeringManager.AvoidObstacles();
    }


}

