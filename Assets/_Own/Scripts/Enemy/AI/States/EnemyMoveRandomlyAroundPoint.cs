using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveRandomlyAroundPoint : FSMState<Enemy>
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float spottingPlayerDistance = 10f;
    [SerializeField] private float patrolTime = 5;
    [SerializeField] private float newSteeringForce = 50;
    [SerializeField] private GameObject patrolPoint;

    private SteeringManager steering;
    private ShootingController shootingController;
    Vector3 newPos;
    private bool patrol;
    private float counter;

    public override void Enter()
    {
        base.Enter();
        GetComponent<Shooting>().enabled = false;
        shootingController = GetComponent<ShootingController>();
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(newSteeringForce);
        counter = patrolTime;
    }

    private void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;

        if (counter >= patrolTime)
        {
            newPos = Random.onUnitSphere * patrolRadius + patrolPoint.transform.position;
            patrol = true;
            counter = 0f;
        }

        if(Vector3.Distance(transform.position,newPos) <= 0f)
        {
            counter = patrolTime;
        }

        if (patrol)
        {
            Patrol(newPos);
        }

        CheckForDistanceWithPlayer();
       
    }

    public override void Exit()
    {
        base.Exit();
        GetComponent<Shooting>().enabled = true;

        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
    }

    private void Patrol(Vector3 randomPos)
    {
        steering.Seek(randomPos, 0f);
        steering.AvoidEnemies();
        steering.AvoidObstacles();
        steering.LookWhereGoing();
    }

    private void CheckForDistanceWithPlayer()
    {
        float distance = (Player.Instance.transform.position - transform.position).magnitude;

        if (distance <= spottingPlayerDistance && shootingController.CanShootAt(Player.Instance.gameObject))
        {
            Debug.Log("distance from player: " + distance);
            agent.fsm.ChangeState<EnemyStateFollowPlayer>();
        }
    }
}
