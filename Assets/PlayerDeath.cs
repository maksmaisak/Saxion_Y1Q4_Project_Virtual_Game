using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private Health health;

    // Use this for initialization
    void Start()
    {
        health = GetComponent<Health>();
        health.OnDeath += Death;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Death(Health sender)
    {
        transform.DetachChildren();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
