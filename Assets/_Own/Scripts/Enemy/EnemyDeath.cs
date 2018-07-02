using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] GameObject crashingEnemyEffectsPrefab;

    private Health health;
    private ParticleManager particleManager;

    void Start()
    {
        particleManager = GetComponentInChildren<ParticleManager>();
        health = GetComponent<Health>();

        health.OnDeath += RetractConnectedGrappleHooks;
        health.OnDeath += InstantiateAfterDeathEffect;
        health.OnDeath += UnparentDeathParticleGroup;
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<Enemy>().fsm.ChangeState<EnemyFallingToDeathState>();
        }
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

    private void UnparentDeathParticleGroup(Health sender)
    {
        particleManager.DetachParticleSystemsFromParent();
    }
}
