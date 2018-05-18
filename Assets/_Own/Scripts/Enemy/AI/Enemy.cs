using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{
    public FSM<Enemy> fsm { get; private set; }

    private AudioSource audioSource;

    private ParticleManager particleManager;

    private float initialHeight;

    private enum GrappleReactionBehaviour
    {
        None,
        ThrustUp,
        Shake,
        PullPlayer
    }

    [SerializeField] private GameObject grappledParticleGroup;
    [SerializeField] private GrappleReactionBehaviour grappleReactionBehaviour;
    [SerializeField] private AudioClip enemyGrappledSound;

    // Use this for initialization
    void Start()
    {
        initialHeight = transform.position.y;

        fsm = new FSM<Enemy>(this);

        particleManager = GetComponentInChildren<ParticleManager>();

        fsm.ChangeState<EnemyMoveRandomlyAroundPoint>();

        audioSource = GetComponent<AudioSource>();

        var grappleable = GetComponent<Grappleable>();

        grappleable.OnGrappled   += OnGrapple;
        grappleable.OnUngrappled += OnRelease;
    }

    private void OnGrapple(Grappleable sender)
    {
        if (fsm.GetCurrentState().GetType() == typeof(EnemyFallingToDeathState))
        {
            return;
        }

        audioSource.PlayOneShot(enemyGrappledSound);
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
