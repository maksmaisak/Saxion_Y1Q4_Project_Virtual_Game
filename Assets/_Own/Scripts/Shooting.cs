using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio))]
public class Shooting : MonoBehaviour
{
    [SerializeField] private float shootingRange = 10;
    [SerializeField] private float reloadTime = 2;
    [SerializeField] private GameObject shootingParticleGroup;

    private float counter;
    private bool reloading;
    private ShootingController shootingController;
    private ParticleManager particleManager;
    new private EnemyAudio audio;

    private GameObject target;

    void Start()
    {
        shootingController = GetComponent<ShootingController>();
        particleManager = GetComponentInChildren<ParticleManager>();
        audio = GetComponent<EnemyAudio>();

        target = Player.Instance.gameObject;
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
                audio.PlayOnShoot();

                reloading = true;
            }
            else
            {
                counter += Time.deltaTime;

                shootingParticleGroup.SetActive(reloadTime - counter >= 0.5f);

                if (counter >= reloadTime)
                {
                    reloading = false;
                    counter = 0f;
                }
            }
        }
    }
}
