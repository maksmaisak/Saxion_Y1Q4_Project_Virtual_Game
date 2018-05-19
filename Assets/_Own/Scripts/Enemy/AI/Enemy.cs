using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio))]
public class Enemy : MonoBehaviour, IAgent
{
    [SerializeField] private GrappleReactionBehaviour grappleReactionBehaviour;
    [SerializeField] private GameObject grappledParticleGroup;

    public FSM<Enemy> fsm { get; private set; }

    new public EnemyAudio audio { get; private set; }
    public ParticleManager particleManager { get; private set; }

    private AudioSource audioSource;

    private float initialHeight;

    private enum GrappleReactionBehaviour
    {
        None,
        ThrustUp,
        Shake,
        PullPlayer
    }

    // Use this for initialization
    void Start()
    {
        initialHeight = transform.position.y;

        fsm = new FSM<Enemy>(this);

        audio = GetComponent<EnemyAudio>();
        particleManager = GetComponentInChildren<ParticleManager>();

        fsm.ChangeState<EnemyMoveRandomlyAroundPoint>();

        audioSource = GetComponent<AudioSource>();

        var grappleable = GetComponent<Grappleable>();
        grappleable.OnGrappled   += OnGrapple;
        grappleable.OnUngrappled += OnRelease;
    }

    private void OnGrapple(Grappleable sender)
    {
        // Meh, but works.
        if (fsm.GetCurrentState().GetType() == typeof(EnemyFallingToDeathState))
        {
            return;
        }

        audio.PlayOnGrappled();
        particleManager.ChangeParticleGroup(grappledParticleGroup);

        switch (grappleReactionBehaviour)
        {
            case GrappleReactionBehaviour.ThrustUp:
                fsm.ChangeState<EnemyThrustUpState>();
                break;
            case GrappleReactionBehaviour.Shake:
                fsm.ChangeState<EnemyShakeState>();
                break;
            case GrappleReactionBehaviour.PullPlayer:
                fsm.ChangeState<EnemyPullPlayerState>();
                break;
        }

        GetComponent<Shooting>().enabled = false;
    }

    private void OnRelease(Grappleable sender)
    {
        fsm.ChangeState<EnemyFallingToDeathState>();
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    public void Print(string message)
    {
        Debug.Log(message);
    }

    private void OnDestroy()
    {
        FlockManager.enemyList.Remove(gameObject);
    }

    public float GetInitialHeight()
    {
        return initialHeight;
    }
}
