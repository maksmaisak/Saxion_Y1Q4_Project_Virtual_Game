using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletScript : MonoBehaviour
{
    private GameObject player;
    private Health health;

    [SerializeField] private GameObject explosion;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        health = player.GetComponent<Health>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player)
        {
            if (player != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                health.DealDamage(100);
            }
        }
        if (collision.gameObject != gameObject)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }



}