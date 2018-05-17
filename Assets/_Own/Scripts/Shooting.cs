using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    [SerializeField] private float shootingRange = 10;
    [SerializeField] private float reloadTime = 2;
    [SerializeField] private GameObject shootingParticleGroup;

    private float counter;
    private bool reloading;
    private ParticleManager particleManager;
    private ShootingController shootingController;
    private GameObject target;

    void Start()
    {
        shootingController = GetComponent<ShootingController>();
        particleManager = GetComponentInChildren<ParticleManager>();
        target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) <= shootingRange)
        {
            Shoot();
        }
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
                if(reloadTime - counter >= 0.5f)
                {
                    particleManager.ChangeParticleGroup(shootingParticleGroup);
                }
                else
                {
                    particleManager.ChangeParticleGroup(particleManager.GetToPlayerParticalGroup());
                }
                if (counter >= reloadTime)
                {
                    reloading = false;
                    counter = 0f;
                }
            }
        }
    }
}
