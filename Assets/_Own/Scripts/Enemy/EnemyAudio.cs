using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

/// A centralized audio controller of an enemy.
public class EnemyAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSourcePersistent;
    [SerializeField] AudioSource audioSourceScreams;
    [Space]
    [SerializeField] AudioClip whileActive;
    [SerializeField] AudioClip onDetectedPlayer;
    [SerializeField] AudioClip onShoot;
    [SerializeField] AudioClip onGrappled;
    [Space]
    [SerializeField] AudioClip screamWhileGrappled;
    [SerializeField] AudioClip screamWhileFallingToDeath;
    [SerializeField] float pitchMin = 1f;
    [SerializeField] float pitchMax = 1f;

    private float defaultScreamsPitch;

    void Start()
    {
        Assert.IsNotNull(audioSourcePersistent);
        Assert.IsNotNull(audioSourceScreams);

        Assert.IsNotNull(onDetectedPlayer);
        Assert.IsNotNull(onShoot);
        Assert.IsNotNull(onGrappled);

        RecordDefaults();
        PlayActivePersistentSound();
    }

    public void PlayActivePersistentSound()
    {
        audioSourcePersistent.clip = whileActive;
        audioSourcePersistent.loop = true;
        audioSourcePersistent.Play();
    }

    public void PlayOnDetectedPlayer()
    {
        audioSourceScreams.pitch = defaultScreamsPitch;
        audioSourceScreams.PlayOneShot(onDetectedPlayer);
    }

    public void PlayOnShoot()
    {
        audioSourceScreams.pitch = defaultScreamsPitch;
        audioSourceScreams.PlayOneShot(onShoot);
    }

    public void PlayOnGrappled()
    {
        audioSourceScreams.pitch = defaultScreamsPitch;
        audioSourceScreams.PlayOneShot(onGrappled);
    }

    public void PlayScreamWhileGrappled()
    {
        audioSourceScreams.pitch = Random.Range(pitchMin, pitchMax);
        audioSourceScreams.PlayOneShot(screamWhileGrappled);
    }

    public void PlayScreamWhileFallingToDeath()
    {
        audioSourceScreams.pitch = Random.Range(pitchMin, pitchMax);
        audioSourceScreams.PlayOneShot(screamWhileFallingToDeath);
    }

    private void RecordDefaults()
    {
        defaultScreamsPitch = audioSourceScreams.pitch;
    }
}
