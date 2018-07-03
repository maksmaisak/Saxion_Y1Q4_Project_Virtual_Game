using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#pragma warning disable 0649

public class BulletScript : MonoBehaviour
{
    [SerializeField] private int bulletDamage = 100;
    [SerializeField] private GameObject explosion;

    [SerializeField] private float onImpactSoundFadeoutDuration = 0.5f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == gameObject) return;
        
        DealDamage(collision);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy();
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
                rb.AddForceAtPosition(-collision.impulse, collision.contacts[0].point, ForceMode.Impulse);
            }
        }
    }

    private void Destroy()
    {
        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>())
        {
            system.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);
        }

        foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>())
        {
            audioSource.DOFade(0f, onImpactSoundFadeoutDuration);
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);

            if (child.GetComponent<AutoDestroyParticleSystem>() == null)
            {
                child.gameObject.AddComponent<AutoDestroyParticleSystem>();
            }
        }
        transform.DetachChildren();

        Destroy(gameObject);
    }
}