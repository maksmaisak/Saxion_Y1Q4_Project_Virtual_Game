using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : FSMState<Enemy> {

    [SerializeField] private float patrolRadius= 5;
    [SerializeField] private float spotingPlayerDistance = 10;

    private SteeringManager steering;
    private Rigidbody rb;
    private GameObject target;
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
        fsm = GetComponent<Enemy>().fsm;
        steering = GetComponent<SteeringManager>();
        steering.SetMaxSteeringForce(50);
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
       
        counter += Time.fixedDeltaTime;

        if (counter >= 5)
        {
             newPos = Random.onUnitSphere * patrolRadius;
            patrol = true;
            counter = 0;
        }

       
        if(patrol)
        {
            Patrol(newPos);
        }

        float distance = (target.transform.position - transform.position).magnitude; 

        if(distance <= spotingPlayerDistance && shootingController.CanShootAt(target))
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
        steering.Seek(randomPos,0);
        steering.AvoidEnemies();
        steering.AvoidObstacles();
        steering.LookWhereGoing();
    }

   
}
