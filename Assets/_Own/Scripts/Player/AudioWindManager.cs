using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AudioWindManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Rigidbody rigidbodyToTrack;
    [Space]
    [SerializeField] float minSpeed = 1f;
    [SerializeField] float maxSpeed = 10f;

    // Use this for initialization
    void Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(rigidbodyToTrack);
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
