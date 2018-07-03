using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio))]
public class Shooting : MonoBehaviour
{
    [SerializeField] float shootingRange = 10f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] float effectTimeBeforeShooting = 0.2f;
    [SerializeField] bool startWithReloading;

    private float timeTillCanShoot;
    
    private ShootingController shootingController;
    private ParticleManager particleManager;
    private new EnemyAudio audio;

    private GameObject target;

    void Start()
    {
        shootingController = GetComponent<ShootingController>();
        particleManager = GetComponentInChildren<ParticleManager>();
        audio = GetComponent<EnemyAudio>();
        
        Assert.IsNotNull(shootingController);
        Assert.IsNotNull(particleManager);
        Assert.IsNotNull(audio);

        target = Player.Instance.gameObject;
        
        timeTillCanShoot = startWithReloading ? reloadTime : 0f;
    }

    void Update()
    {
        if (timeTillCanShoot > 0f)
        {
            timeTillCanShoot -= Time.deltaTime;

            if (timeTillCanShoot < 0f)
            {
                timeTillCanShoot = 0f;
            }
        }

        bool shouldShoot = target != null &&
                           Vector3.Distance(target.transform.position, transform.position) <= shootingRange &&
                           shootingController.CanShootAt(target);
        
        particleManager.SetShootingEffectsActive(shouldShoot && timeTillCanShoot <= effectTimeBeforeShooting);
        if (shouldShoot && timeTillCanShoot <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {   
        bool didShoot = shootingController.ShootAt(target);
        if (!didShoot) return;
        
        audio.PlayOnShoot();
        timeTillCanShoot = reloadTime;
    }
}
