using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject crashingEnemyEffect;

    private Health health;

    private void Start()
    {
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
}
