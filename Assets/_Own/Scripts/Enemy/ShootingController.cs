using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(AudioSource))]
public class ShootingController : MonoBehaviour {

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float muzzleSpeed = 100f;
    [SerializeField] AudioClip shootSound;

    [SerializeField] LayerMask obstacleDetectionLayerMask;

    AudioSource audioSource;

    void Start() {

        audioSource = GetComponent<AudioSource>();
    }

    public bool CanShootAt(GameObject target) {

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
            radius: 0.2f,
            direction: delta.normalized,
            hitInfo: out hit,
            maxDistance: delta.magnitude,
            layerMask: obstacleDetectionLayerMask & ~(1 << target.layer)
        );

        if (didHit && hit.collider.gameObject != gameObject) return false;

        return true;
    }

    public bool ShootAt(GameObject target) {

        /*Vector3 delta = target.transform.position - transform.position;
        Vector3 direction = delta.normalized;

        Shoot(
            position: transform.position + direction * 1.2f, 
            startVelocity: direction * muzzleSpeed
        );*/

        Vector3 delta = target.transform.position - transform.position;
        Vector3 flatDelta = new Vector3(delta.x, 0f, delta.z);
        Vector3 direction = flatDelta.normalized;

        Vector3 shootPosition = transform.position + direction * 1.2f;

        Vector3? startVelocity = Ballistics.GetStartVelocity(
            start: shootPosition,
            target: target.transform.position,
            muzzleSpeed: muzzleSpeed
        );

        if (!startVelocity.HasValue) return false;

        Shoot(
            position: shootPosition,
            startVelocity: startVelocity.Value
        );

        return true;
    }

    private void Shoot(Vector3 position, Vector3 startVelocity) {

        Instantiate(bulletPrefab, position, Quaternion.identity)
            .GetComponent<Rigidbody>()
            .AddForce(startVelocity, ForceMode.VelocityChange);

        if (shootSound != null) {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
