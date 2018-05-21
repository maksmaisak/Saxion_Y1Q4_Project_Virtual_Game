using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

#pragma warning disable 0649

/// <summary>
/// Activates upon collision with the player.
/// Only an unlocked checkpoint can be activated.
/// To unlock a checkpoint, all prerequisite checkpoints must have been activated,
/// and all specified entities with health need to have died.
/// FIXME KNOWN ISSUE: if a required entity is destroyed without dying, the checkpoint still won't
/// </summary>
public class Checkpoint : MonoBehaviour
{
    // FIXME HACK TODO
    // Kinda hacky, must change when we get to the respective user story.
    public static Vector3? LatestActiveCheckpointPosition { get; private set; }

    public event Action<Checkpoint> OnActivated;

    [SerializeField] private UnityEvent onAllPrerequisiteCheckpointsActive = new UnityEvent();
    [Tooltip("Called when both prerequisite checkpoints are active and all entities are dead. A better name would be 'onUnlock', but that would screw up existing settings in prefabs and gameobjects in scenes. Sorry about that.")]
    [SerializeField] private UnityEvent onAllRequiredEntitiesDead = new UnityEvent();
    [SerializeField] private UnityEvent onActivated = new UnityEvent();

    [Tooltip("All the checkpoints that need to be active for this one to unlock. If none are specified, the checkpoint will unlock immediately.")]
    [SerializeField] private List<Checkpoint> prerequisiteCheckpoints;

    [Tooltip("All of these need to die for this one to unlock.")]
    [SerializeField] private List<Health> needToDieToUnlock = new List<Health>();

    [Tooltip("The checkpoint will activate when the player enters this trigger. This field will be filled automatically as long as this gameobject or one of its children has a trigger collider or a TriggerEvents component.")]
    [SerializeField] private TriggerEvents triggerEvents;

    private bool isLocked = true;

    void Start()
    {
        prerequisiteCheckpoints.RemoveAll(checkpoint => checkpoint == null);
        needToDieToUnlock.RemoveAll(health => health == null);

        EnsureTrigger();
        triggerEvents.onPlayerTriggerStay.AddListener(OnPlayerTriggerStay);

        bool anyPrerequisiteCheckpoints = prerequisiteCheckpoints.Count > 0;
        if (!anyPrerequisiteCheckpoints)
        {
            onAllPrerequisiteCheckpointsActive.Invoke();
        }
        else
        {
            foreach (Checkpoint checkpoint in prerequisiteCheckpoints)
            {
                checkpoint.OnActivated += OnPrerequisiteCheckpointActivated;
            }
        }

        bool anyEntitiesNeedToDie = needToDieToUnlock.Count > 0;
        if (!anyEntitiesNeedToDie)
        {
            if (!anyPrerequisiteCheckpoints)
            {
                onAllRequiredEntitiesDead.Invoke();
                Unlock();
            }
        }
        else
        {
            foreach (Health health in needToDieToUnlock)
            {
                health.OnDeath += OnRequiredEntityDied;
            }
        }
    }

    private void OnPlayerTriggerStay()
    {
        if (!isLocked)
        {
            Activate();
        }
    }

    private void Activate()
    {
        // Don't reload the scene to restart, call a custom something
        LatestActiveCheckpointPosition = transform.position;

        if (OnActivated != null) OnActivated(this);
        onActivated.Invoke();

        Lock();
    }

    private void Unlock()
    {
        isLocked = false;
    }

    private void Lock()
    {
        isLocked = true;
    }

    private void OnPrerequisiteCheckpointActivated(Checkpoint checkpoint)
    {
        checkpoint.OnActivated -= OnPrerequisiteCheckpointActivated;

        prerequisiteCheckpoints.Remove(checkpoint);

        if (prerequisiteCheckpoints.Count == 0)
        {
            onAllPrerequisiteCheckpointsActive.Invoke();

            if (needToDieToUnlock.Count == 0)
            {
                onAllRequiredEntitiesDead.Invoke();

                Unlock();
            }
        }
    }

    private void OnRequiredEntityDied(Health sender)
    {
        sender.OnDeath -= OnRequiredEntityDied;

        needToDieToUnlock.Remove(sender);

        if (prerequisiteCheckpoints.Count == 0 && needToDieToUnlock.Count == 0)
        {
            onAllRequiredEntitiesDead.Invoke();
            Unlock();
        }
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
