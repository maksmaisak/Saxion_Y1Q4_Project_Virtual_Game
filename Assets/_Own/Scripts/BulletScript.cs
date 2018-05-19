using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class BulletScript : MonoBehaviour
{
    [SerializeField] private int bulletDamage = 100;
    [SerializeField] private GameObject explosion;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != gameObject)
        {
            DealDamage(collision);
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy();
        }
    }

    private void DealDamage(Collision collision)
    {
        var health = collision.gameObject.GetComponent<Health>();
        if (health == null) return;

        bool wasAlive = health.isAlive;
        health.DealDamage(bulletDamage);
        bool didDie = health.isDead && wasAlive;

        if (didDie)
        {
            var rb = collision.collider.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(-collision.impulse, ForceMode.Impulse);
            }
        }
    }

    private void Destroy()
    {
        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
        {
            system.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            if (!child.GetComponent<AutoDestroyParticleSystem>() == null)
            {
                child.gameObject.AddComponent<AutoDestroyParticleSystem>();
            }
        }
        transform.DetachChildren();

        Destroy(gameObject);
    }
}