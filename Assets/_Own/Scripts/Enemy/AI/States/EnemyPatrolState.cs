using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : FSMState<Enemy>
{
    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private float spottingPlayerDistance = 10f;

    private SteeringManager steering;

    // This is literally never used.
    private Rigidbody rb;

    // Why store this? Just use Player.Instance
    private GameObject target;

    // You don't need to store it, just use agent.fsm (`agent` is a protected variable of FSMState, look into it).
    private FSM<Enemy> fsm;
    private ShootingController shootingController;

    Vector3 newPos;
    private bool patrol;
    private float counter;

    public override void Enter()
    {
        base.Enter();
        GetComponent<Shooting>().enabled = false;
        shootingController = GetComponent<ShootingController>();
        fsm = agent.fsm;
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(50);
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        counter += Time.fixedDeltaTime;

        // Don't hardcode values.
        // This also makes the enemy wait 5 seconds after entering the state before moving.
        if (counter >= 5f)
        {
            // This here is a position near the origin (0, 0, 0) of the scene.
            // Should be somewhere close a designated position.
            newPos = Random.onUnitSphere * patrolRadius;
            patrol = true;
            counter = 0f;
        }

        if (patrol)
        {
            Patrol(newPos);
        }

        float distance = (target.transform.position - transform.position).magnitude;

        if (distance <= spottingPlayerDistance && shootingController.CanShootAt(target))
        {
            fsm.ChangeState<EnemyStateFollowPlayer>();
        }
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
}
