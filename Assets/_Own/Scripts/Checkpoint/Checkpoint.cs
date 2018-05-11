using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

/// <summary>
/// Activates upon collision with the player.
/// Only a useable checkpoint can be activated.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    // FIXME HACK TODO
    // Kinda hacky, must change when we get to the respective user story.
    public static Vector3? LatestActiveCheckpointPosition { get; private set; }

    public event Action<Checkpoint> OnActivated;

    [Tooltip("All the checkpoints that need to be active for this one to become enabled.")]
    [SerializeField] private Checkpoint[] prerequisiteCheckpoints;

    private ParticleSystem[] particleSystems;

    private int numPrerequisiteCheckpointsLeft;
    private bool isUseable;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        numPrerequisiteCheckpointsLeft = prerequisiteCheckpoints.Length;
        if (numPrerequisiteCheckpointsLeft == 0)
        {
            MakeUseable();
        }
        else
        {
            foreach (Checkpoint checkpoint in prerequisiteCheckpoints)
            {
                checkpoint.OnActivated += OnPrerequisiteCheckpointActivated;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isUseable && collision.gameObject.tag == "Player")
        {
            Activate();
        }
    }

    private void Activate()
    {
        // Don't reload the scene to restart, call a custom something
        LatestActiveCheckpointPosition = transform.position;

        if (OnActivated != null) OnActivated(this);

        MakeUnuseable();
    }

    private void MakeUseable()
    {
        foreach (var system in particleSystems)
        {
            if (!system.isPlaying) system.Play();
        }

        isUseable = true;
    }

    private void MakeUnuseable()
    {
        foreach (var system in particleSystems)
        {
            if (!system.isStopped) system.Stop();
        }

        isUseable = false;
    }

    private void OnPrerequisiteCheckpointActivated(Checkpoint checkpoint)
    {
        checkpoint.OnActivated -= OnPrerequisiteCheckpointActivated;

        numPrerequisiteCheckpointsLeft -= 1;
        if (numPrerequisiteCheckpointsLeft <= 0)
        {
            MakeUseable();
        }
    }
}
