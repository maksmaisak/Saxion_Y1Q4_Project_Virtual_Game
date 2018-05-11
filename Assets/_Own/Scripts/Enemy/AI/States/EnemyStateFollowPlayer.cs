
using System.Collections; using System.Collections.Generic; using UnityEngine;  public class EnemyStateFollowPlayer : FSMState<Enemy>, IBOID {

    [SerializeField] private float maxSteeringForce;     [SerializeField] private float maxVelocity = 6;     [SerializeField] private float collisionAvoidanceMultiplier = 20;     [SerializeField] private float MaxDistanceToPlayer = 5;
    [SerializeField] private float ShootingRange = 10;     [SerializeField] private float collisionAvoidanceRange = 2;     [SerializeField] private float maxRotationDegreesPerSecond = 180f;     [SerializeField] private float separationFactor = 1f;     [SerializeField] private float separationDistance = 5f;      private Rigidbody rb;     private GameObject target;     private ShootingController shootingController;     private float speed;     private bool reloading;     private float counter;     private bool playerFound;     private SteeringManager steeringManager; 
     void Start()     {         shootingController = GetComponent<ShootingController>();         target = GameObject.FindGameObjectWithTag("Player");         steeringManager = new SteeringManager(this);         rb = GetComponent<Rigidbody>();     }      void Update()     {

        Vector3 steering = Vector3.zero;         Vector3 desiredPos = Vector3.zero;          Debug.DrawRay(transform.position, rb.velocity.normalized * collisionAvoidanceRange, Color.red);          desiredPos = target.transform.position + (transform.position - target.transform.position).normalized * MaxDistanceToPlayer;

        steering += steeringManager.DoSeek(desiredPos, MaxDistanceToPlayer);
        steering += steeringManager.DoObstaclesAvoidance();         steering += steeringManager.DoEnemyAvoidance();

        steering = Vector3.ClampMagnitude(steering, maxSteeringForce);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity + steering, maxVelocity);

         Debug.DrawRay(transform.position, steering, Color.blue);


        Rotate();          if (Vector3.Distance(target.transform.position, transform.position) <= ShootingRange)         {

            playerFound = true;             Shoot();         }

        if (!shootingController.CanShootAt(target))         {             playerFound = false;         }     } 
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
    }      private void Shoot()     {         if (shootingController.CanShootAt(target))         {             if (!reloading)             {                 shootingController.ShootAt(target);                 reloading = true;             }             else             {                 counter += Time.deltaTime;                 if (counter >= 2f)                 {                     reloading = false;                     counter = 0f;                 }             }         }     } 


    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    public float GetMaxVelocity()
    {
        return maxVelocity;
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public float GetMaxForce()
    {
        return maxSteeringForce;
    }

    public float GetSeparationDistance()
    {
        return separationDistance;
    }

    public GameObject GetEnemy()
    {
        return this.gameObject;
    }

    public float GetSeparationFactor()
    {
        return separationFactor;
    }

    public float GetCollisionAvoidanceMultiplier()
    {
        return collisionAvoidanceMultiplier;
    }
    public float GetCollisionAvoidanceRange()
    {
        return collisionAvoidanceRange;
    }

}  