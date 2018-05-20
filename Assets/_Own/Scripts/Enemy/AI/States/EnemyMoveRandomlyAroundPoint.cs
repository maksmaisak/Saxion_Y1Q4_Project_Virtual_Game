using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyMoveRandomlyAroundPoint : FSMState<Enemy>
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float spottingPlayerDistance = 10f;
    [SerializeField] private float patrolTime = 5;
    [SerializeField] private float newSteeringForce = 50;
    [SerializeField] private float minDistanceFromAreaBorder = 4f;
    [SerializeField] private Transform patrolPoint;
    [SerializeField] private GameObject wanderingParticleGroup;

    private ParticleManager particleManager;
    private ShootingController shootingController;
    private SteeringManager steering;
    Vector3 newPos;
    private bool patrol;
    private float counter;

    public override void Enter()
    {
        base.Enter();
        GetComponent<Shooting>().enabled = false;
        particleManager = GetComponentInChildren<ParticleManager>();
        shootingController = GetComponent<ShootingController>();
        steering = agent.steering;

        steering.SetMaxSteeringForce(newSteeringForce);
        counter = patrolTime;
        particleManager.ChangeParticleGroup(wanderingParticleGroup);
    }

    private void FixedUpdate()
    {
        /*counter += Time.fixedDeltaTime;

        if (counter >= patrolTime)
        {
            newPos = Random.onUnitSphere * patrolRadius + patrolPoint.position;
            patrol = true;
            counter = 0f;
        }

        if (Vector3.Distance(transform.position, newPos) <= 0f)
        {
            counter = patrolTime;
        }

        if (patrol)
        {
            Patrol(newPos);
        }*/

        SteerToStayInArea();
        steering.Wander();

        steering.AvoidObstacles();
        steering.FlockingSeparation(Enemy.allAsSteerables);

        steering.LookWhereGoing();

        CheckDetectPlayer();
    }

    public override void Exit()
    {
        base.Exit();
        GetComponent<Shooting>().enabled = true;
        steering.SetMaxSteeringForce(steering.GetInitalSteeringForce());
        particleManager.DisableAllParticleGroups();
    }

    private void Patrol(Vector3 randomPos)
    {
        steering.Seek(randomPos, 0f);
        steering.FlockingSeparation(Enemy.allAsSteerables);
        steering.AvoidObstacles();
        steering.LookWhereGoing();
    }

    private void CheckDetectPlayer()
    {
        float distance = (Player.Instance.transform.position - transform.position).magnitude;

        if (distance <= spottingPlayerDistance && shootingController.CanShootAt(Player.Instance.gameObject))
        {
            Debug.Log("Detected the player, distance: " + distance);

            agent.audio.PlayOnDetectedPlayer();
            agent.fsm.ChangeState<EnemyStateFollowPlayer>();
        }
    }

    private void SteerToStayInArea()
    {
        Vector3 toCenter = patrolPoint.position - transform.position;
        float distance = toCenter.magnitude;
        if (distance >= patrolRadius - minDistanceFromAreaBorder)
        {
            steering.Seek(patrolPoint.position);
        }
    }
}
