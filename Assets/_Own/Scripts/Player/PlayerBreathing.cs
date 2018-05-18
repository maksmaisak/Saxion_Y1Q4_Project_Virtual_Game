using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

#pragma warning disable 0649

public class PlayerBreathing : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] RigidbodyFirstPersonController playerController;
    [Tooltip("The minimum speed with which the player needs to have moved before panting is possible.")]
    [SerializeField] float minSpeed = 10f;
    [SerializeField] float minDelayBetweenBreaths = 10f;

    private bool didExceedMinSpeed;

    IEnumerator Start()
    {
        Assert.IsNotNull(audioSource);
        Assert.IsNotNull(playerController);

        audioSource.loop = false;

        while (true)
        {
            yield return new WaitForSeconds(minDelayBetweenBreaths);
            yield return new WaitUntil(CanPlay);

            audioSource.Play();
            didExceedMinSpeed = false;
        }
    }

    private bool CanPlay()
    {
        return didExceedMinSpeed && playerController.isGrounded;
    }

    void FixedUpdate()
    {
        if (playerController.velocity.sqrMagnitude > minSpeed * minSpeed)
        {
            didExceedMinSpeed = true;
        }
    }
}
