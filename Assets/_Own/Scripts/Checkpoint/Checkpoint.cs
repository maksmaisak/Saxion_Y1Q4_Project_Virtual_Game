using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    [Tooltip("All the checkpoints that need to be active for this one to become useable. If none are specified, the checkpoint will activate immediately.")]
    [SerializeField] private List<Checkpoint> prerequisiteCheckpoints;

    [Tooltip("All of these need to die for this one to become useable.")]
    [SerializeField] private List<Health> needToDieToActivate = new List<Health>();

    [Tooltip("The checkpoint will activate when the player enters this trigger. This field will be filled automatically as long as this gameobject or one of its children has a trigger collider or a TriggerEvents component.")]
    [SerializeField] private TriggerEvents triggerEvents;

    private ParticleSystem[] particleSystems;

    private bool isUseable;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        EnsureTrigger();
        triggerEvents.onPlayerTriggerEnter.AddListener(OnPlayerTriggerEnter);

        if (ShouldBeUseable())
        {
            MakeUseable();
        }
        else
        {
            foreach (Checkpoint checkpoint in prerequisiteCheckpoints)
            {
                checkpoint.OnActivated += OnPrerequisiteCheckpointActivated;
            }

            foreach (Health health in needToDieToActivate)
            {
                health.OnDeath += OnRequiredEnemyDied;
            }
        }
    }

    private void OnPlayerTriggerEnter()
    {
        if (isUseable)
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

        prerequisiteCheckpoints.Remove(checkpoint);
        if (ShouldBeUseable())
        {
            MakeUseable();
        }
    }

    private void OnRequiredEnemyDied(Health sender)
    {
        sender.OnDeath -= OnRequiredEnemyDied;

        needToDieToActivate.Remove(sender);
        if (ShouldBeUseable())
        {
            MakeUseable();
        }
    }

    private bool ShouldBeUseable()
    {
        return
            prerequisiteCheckpoints.Count == 0 &&
            needToDieToActivate.Count == 0;
    }

    /// <summary>
    /// Makes sure triggerEvents is not null.
    /// </summary>
    private void EnsureTrigger()
    {
        if (triggerEvents == null)
        {
            triggerEvents = GetComponentInChildren<TriggerEvents>();

            if (triggerEvents == null)
            {
                var trigger = GetComponentsInChildren<Collider>().FirstOrDefault(col => col.isTrigger);
                if (trigger != null)
                {
                    triggerEvents = trigger.gameObject.AddComponent<TriggerEvents>();
                    Debug.Log("TriggerEvents was not set in the inspector and not found. Found a trigger collider in children and added TriggerEvents component to it. Found trigger: " + trigger);
                }
            }
        }

        Assert.IsNotNull(triggerEvents, "[" + this + "]: TriggerEvents was not set in the inspector and not found! Could not find a trigger collider in children!");
    }
}
