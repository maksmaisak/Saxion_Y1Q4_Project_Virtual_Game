using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private int bulletDamage = 100;
    [SerializeField] private GameObject explosion;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != gameObject)
        {
            DealDamage(collision);
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
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

}