using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

[RequireComponent(typeof(Grapple), typeof(AudioSource), typeof(Rigidbody))]
public class GrappleChainAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] rattleClips;
    [SerializeField] Rigidbody playerRigidbody;
    [Space]
    [SerializeField] float timeTillMaxVolume = 0.1f;
    [SerializeField] [Range(0f, 1f)] float maxVolume = 1f;
    [SerializeField] float playProbabilityPerFrame = 0.01f;
    [SerializeField] float minRelativeSpeed = 2f;
    [SerializeField] float pitchMin = 1f;
    [SerializeField] float pitchMax = 1f;

    private Grapple grapple;
    private Rigidbody ownRigidbody;

    private bool grappleWasConnectedLastFrame;

    private float targetVolume;

    void Start()
    {
        grapple = GetComponent<Grapple>();

        audioSource = audioSource ?? GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);

        Assert.IsNotNull(rattleClips);
        Assert.IsTrue(rattleClips.Length > 0);

        Assert.IsNotNull(playerRigidbody);

        ownRigidbody = GetComponent<Rigidbody>();

        audioSource.volume = 0f;
        if (audioSource.isPlaying) audioSource.Stop();
    }

    void Update()
    {
        PlayIfNeeded();
        AdjustVolume();
    }

    public void Play()
    {
        if (audioSource.isPlaying) return;

        targetVolume = maxVolume;
        audioSource.volume = 0f;
        audioSource.pitch = Random.Range(pitchMin, pitchMax);

        AudioClip clip = rattleClips[Random.Range(0, rattleClips.Length)];
        audioSource.PlayOneShot(clip);
    }

    private void PlayIfNeeded()
    {
        if (audioSource.isPlaying) return;

        if (grapple.isConnected)
        {
            if (!grappleWasConnectedLastFrame)
            {
                Play();
            }
            else
            {
                Vector3 relativeVelocity = playerRigidbody.velocity - ownRigidbody.velocity;
                if (Random.value < playProbabilityPerFrame && relativeVelocity.sqrMagnitude > minRelativeSpeed * minRelativeSpeed)
                {
                    Play();
                } 
            }
        }

        grappleWasConnectedLastFrame = grapple.isConnected;
    }

    private void AdjustVolume()
    {
        if (!audioSource.isPlaying) return;
        if (grapple.isRetracted)
        {
            targetVolume = 0f;
            return;
        }

        float maxVolumeChange = (maxVolume / timeTillMaxVolume) * Time.deltaTime;

        audioSource.volume = Mathf.MoveTowards(
            audioSource.volume,
            targetVolume,
            maxVolumeChange
        );
    }
}
