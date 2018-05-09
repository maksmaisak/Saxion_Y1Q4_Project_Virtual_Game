using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Ok this here needs work. It keeps moving towards the player in some weird roundabout way.

public class EnemyStateFollowPlayer : FSMState<Enemy>
{
    public float speed;

    [SerializeField] private float maxRotationDegreesPerSecond = 180f;
    [SerializeField] private float separationFactor = 1f;
    [SerializeField] private float separationDistance = 5f;

    private Rigidbody rb;
    private GameObject target;
    private ShootingController shootingController;
    private bool reloading;
    private float counter;
    private bool playerFound;

    void Start()
    {
        shootingController = GetComponent<ShootingController>();
        target = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var dir = (target.transform.position + new Vector3(0, -1f, 1f) - transform.position).normalized;

        RaycastHit _hit;

        if (!playerFound)
        {
            if (Physics.SphereCast(transform.position, 2f, transform.forward, out _hit, 20f))
            {
                if (_hit.transform != transform)
                {
                    dir += _hit.normal * 50f;
                }
            }
        }

        var rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, maxRotationDegreesPerSecond * Time.deltaTime); //Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        rb.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(target.transform.position, transform.position) <= 7f)
        {
            speed--;
            if (speed < 0f) speed = 0f;
            playerFound = true;
            Shoot();
        }
        else
        {
            speed++;
            if (speed > 6f) speed = 6f;
        }

        if (!shootingController.CanShootAt(target))
        {
            speed = 6f;
            playerFound = false;
        }

        AvoidOtherEnemies();
    }

    private void Shoot()
    {
        if (shootingController.CanShootAt(target))
        {
            if (!reloading)
            {
                shootingController.ShootAt(target);
                reloading = true;
            }
            else
            {
                counter += Time.deltaTime;
                if (counter >= 2f)
                {
                    reloading = false;
                    counter = 0f;
                }
            }
        }
    }

    private void AvoidOtherEnemies()
    {
        Vector3 totalForce = Vector3.zero;
        foreach (GameObject enemy in FlockManager.enemyArray)
        {
            if (this != enemy && Vector3.Distance(transform.position, enemy.transform.position) <= 3f)
            {
                Vector3 headingVector = transform.position - enemy.transform.position;
                totalForce += headingVector;
            }
        }

        rb.AddForce(totalForce * separationFactor, ForceMode.Acceleration);
    }
}
