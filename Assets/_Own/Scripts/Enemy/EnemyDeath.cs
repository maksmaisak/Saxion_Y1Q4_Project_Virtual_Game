using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] GameObject crashingEnemyEffectsPrefab;
    [SerializeField] float fallDeathYPos = -50f;

    private Health health;
    private AudioSource audioSource;
    private ParticleManager particleManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        particleManager = GetComponentInChildren<ParticleManager>();
        health = GetComponent<Health>();

        health.OnDeath += RetractConnectedGrappleHooks;
        health.OnDeath += InstantiateAfterDeathEffect;
        health.OnDeath += UnparentDeathParticleGroup;
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

    private void RetractConnectedGrappleHooks(Health sender)
    {
        foreach (var grapple in gameObject.GetComponentsInChildren<Grapple>())
        {
            grapple.RetractImmediate();
        }
    }

    private void InstantiateAfterDeathEffect(Health sender)
    {
        if (crashingEnemyEffectsPrefab == null) return;
        Instantiate(crashingEnemyEffectsPrefab, transform.position, Quaternion.identity);
    }

    private void FallingToAbyssDeath()
    {
        if (transform.position.y <= fallDeathYPos)
        {
            health.DealDamage(100);
        }
    }

    private void UnparentDeathParticleGroup(Health sender)
    {
        particleManager.DetachFromParent();
        //particleManager.UnparentParticleGroup(fallingParticleGroup);
    }
}
