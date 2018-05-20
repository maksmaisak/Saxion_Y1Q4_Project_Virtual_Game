using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio))]
public class Shooting : MonoBehaviour
{
    [SerializeField] float shootingRange = 10f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float effectTimeBeforeShooting = 0.2f;
    [SerializeField] bool startWithReloading;

    private float counter;
    private bool isReloading;
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

        isReloading = startWithReloading;
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
            if (!isReloading)
            {
                shootingController.ShootAt(target);
                audio.PlayOnShoot();

                isReloading = true;
            }
            else
            {
                counter += Time.deltaTime;

                particleManager.SetShootingEffectsActive(reloadTime - counter <= effectTimeBeforeShooting);

                if (counter >= reloadTime)
                {
                    isReloading = false;
                    counter = 0f;
                }
            }
        }
    }
}
