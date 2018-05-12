using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// Adjusts the volume of a given audioSource to play a wind sound when moving fast.
public class AudioWindManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Rigidbody rigidbodyToTrack;
    [Space]
    [Tooltip("Volume is 0 when moving slower than this.")]
    [SerializeField] float minSpeed = 1f;
    [Tooltip("Volume is 1 when moving faster than this.")]
    [SerializeField] float maxSpeed = 10f;

    // Use this for initialization
    void Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(rigidbodyToTrack);

        audioSource.loop = true;
        if (!audioSource.isPlaying) audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = GetDesiredVolume();
    }

    private float GetDesiredVolume()
    {
        float speed = rigidbodyToTrack.velocity.magnitude;
        return Mathf.Clamp01(Mathf.InverseLerp(minSpeed, maxSpeed, speed));
    }
}
