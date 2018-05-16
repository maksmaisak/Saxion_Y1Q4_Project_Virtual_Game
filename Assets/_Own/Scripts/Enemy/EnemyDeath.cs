using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject crashingEnemyEffect;
    [SerializeField]
    private float fallDeathYPos = -50;
    [SerializeField]
    private AudioClip deathSound;

    private Health health;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();

        health.OnDeath += PlayDeathSound;
        health.OnDeath += InstantiateExplosionEffectOnDeath;
        health.OnDeath += RetractOnDestroy;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            health.DealDamage(100);
        }

        FallingToAbyssDeath();
    }

    public void RetractOnDestroy(Health sender)
    {
        foreach (var grapple in gameObject.GetComponentsInChildren<Grapple>())
        {
            grapple.Retract();
        }
    }

    public void InstantiateExplosionEffectOnDeath(Health sender)
    {
        Instantiate(crashingEnemyEffect, transform.position, Quaternion.identity);
    }

    public void PlayDeathSound(Health sender)
    {
        audioSource.PlayOneShot(deathSound);
    }

    private void FallingToAbyssDeath()
    {
        if(transform.position.y <= fallDeathYPos)
        {
            health.DealDamage(100);
        }
    }

}
