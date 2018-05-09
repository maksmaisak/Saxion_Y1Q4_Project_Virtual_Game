using System;
using UnityEngine;

#pragma warning disable 0649

public class PathFinding : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;

    private GameObject target;

    private ShootingController shootingController;

    private bool reloading;

    private float counter;

    private bool playerFound;


    [SerializeField] private float seperationFactor;
    [SerializeField] private float seperationDistance;


    void Start()
    {

        shootingController = GetComponent<ShootingController>();
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

    }


    void Update()
    {
        var dir = (target.transform.position + new Vector3(0, -1, 1) - transform.position).normalized;

        RaycastHit _hit;

        if (!playerFound)
        {
            if (Physics.SphereCast(transform.position, 2, transform.forward, out _hit, 20))
            {
                if (_hit.transform != transform)
                {
                    dir += _hit.normal * 50;
                }
            }
        }


        var rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        rb.position += transform.forward * speed * Time.deltaTime;


        if (Vector3.Distance(target.transform.position, transform.position) <= 7)
        {

            speed--;
            if (speed <= 0) speed = 0;
            playerFound = true;
            Shoot();
        }
        else
        {
            speed++;
            if (speed > 6) speed = 6;
        }

        if (!shootingController.CanShootAt(target))
        {
            speed = 6;
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
                if (counter >= 2)
                {
                    reloading = false;
                    counter = 0;
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

        rb.AddForce(totalForce * seperationFactor, ForceMode.Acceleration);
    }
}

