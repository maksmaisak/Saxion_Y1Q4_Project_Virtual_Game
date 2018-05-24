using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 0649

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] float timeTillRestart = 2f;
    [SerializeField] float timeScaleMultiplier = 0.4f;

    private bool didDie;
    private float originalTimeScale;
    private float originalFixedDeltaTime;

    void Start()
    {
        var health = GetComponent<Health>();
        health.OnDeath += OnDeathHandler;
    }

    void OnDestroy()
    {
        if (didDie)
        {
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }

    private void OnDeathHandler(Health sender)
    {
        var firstPersonController = GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>();
        if (firstPersonController != null) firstPersonController.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.freezeRotation = false;
        }

        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale *= timeScaleMultiplier;
        Time.fixedDeltaTime *= timeScaleMultiplier;

        Invoke("Restart", timeTillRestart);

        didDie = true;
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
