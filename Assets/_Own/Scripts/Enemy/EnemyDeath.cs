using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject crashingEnemyEffectsPrefab;

    [SerializeField]
    private float fallDeathYPos = -50;

    private Health health;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();

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
        Instantiate(crashingEnemyEffectsPrefab, transform.position, Quaternion.identity);
    }

    private void FallingToAbyssDeath()
    {
        if(transform.position.y <= fallDeathYPos)
        {
            health.DealDamage(100);
        }
    }

}
