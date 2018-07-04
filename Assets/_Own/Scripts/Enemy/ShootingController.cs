using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

/// A ballistics-based projectile shooting helper.
public class ShootingController : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float muzzleSpeed = 100f;
    [SerializeField] float spawnPointOffset = 1.2f;
    [Space]
    [SerializeField] LayerMask obstacleDetectionLayerMask;
    [SerializeField] float sphereCastRadius = 0.1f;

    public bool CanShootAt(GameObject target)
    {
        Vector3? startVelocity = Ballistics.GetStartVelocity(
            transform.position,
            target.transform.position,
            muzzleSpeed
        );

        if (!startVelocity.HasValue) return false;

        Vector3 delta = target.transform.position - transform.position;

        RaycastHit hit;
        bool didHit = Physics.SphereCast(
            origin: transform.position,
            radius: sphereCastRadius,
            direction: delta.normalized,
            hitInfo: out hit,
            maxDistance: delta.magnitude,
            layerMask: obstacleDetectionLayerMask & ~(1 << target.layer),
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );

        return !didHit || hit.collider.gameObject == gameObject;
    }

    /// Returns true if successful
    public bool ShootAt(GameObject target)
    {
        Vector3 delta = target.transform.position - transform.position;
        Vector3 direction = delta.normalized;

        Vector3 projectileSpawnPosition = transform.position + direction * spawnPointOffset;

        Vector3? startVelocity = Ballistics.GetStartVelocity(
            start: projectileSpawnPosition,
            target: target.transform.position,
            muzzleSpeed: muzzleSpeed
        );

        if (!startVelocity.HasValue) return false;

        Shoot(
            position: projectileSpawnPosition,
            startVelocity: startVelocity.Value
        );

        return true;
    }

    private void Shoot(Vector3 position, Vector3 startVelocity)
    {
        Instantiate(bulletPrefab, position, Quaternion.identity)
            .GetComponent<Rigidbody>()
            .AddForce(startVelocity, ForceMode.VelocityChange);
    }
}
