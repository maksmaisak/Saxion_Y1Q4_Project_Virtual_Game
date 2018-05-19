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

    void Start()
    {
        Assert.IsNotNull(audioSourcePersistent);
        Assert.IsNotNull(audioSourceScreams);

        Assert.IsNotNull(onDetectedPlayer);
        Assert.IsNotNull(onShoot);
        Assert.IsNotNull(onGrappled);

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
        audioSourceScreams.PlayOneShot(onDetectedPlayer);
    }

    public void PlayOnShoot()
    {
        audioSourceScreams.PlayOneShot(onShoot);
    }

    public void PlayOnGrappled()
    {
        audioSourceScreams.PlayOneShot(onGrappled);
    }
}
