using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{
    public FSM<Enemy> fsm { get; private set; }

    private Grappleable grappleable;

    private AudioSource audioSource;

    private ParticleManager particleManager;

    private float initialHeight;

    private enum State
    {
        None,
        ThrustUp,
        Shake,
        PullPlayer
    }

    [SerializeField] private GameObject grappledParticleGroup;
    [SerializeField] private State grappleReactionBehaviour;
    [SerializeField] private AudioClip enemyGrappeledSound;

    // Use this for initialization
    void Start()
    {
        initialHeight = transform.position.y;

        fsm = new FSM<Enemy>(this);

        particleManager = GetComponentInChildren<ParticleManager>();

        fsm.ChangeState<EnemyMoveRandomlyAroundPoint>();

        audioSource = GetComponent<AudioSource>();

        grappleable = GetComponent<Grappleable>();

        grappleable.OnGrappled   += OnGrapple;
        grappleable.OnUngrappled += OnRelease;
    }

    private void OnGrapple(Grappleable sender)
    {
        audioSource.PlayOneShot(enemyGrappeledSound);

        if (fsm.GetCurrentState() != FindObjectOfType<EnemyFallingToDeathState>())
        {
            particleManager.ChangeParticleGroup(grappledParticleGroup);
        }

        switch (grappleReactionBehaviour)
        {
            case State.ThrustUp:
                fsm.ChangeState<EnemyThrustUpState>();
                break;
            case State.Shake:
                fsm.ChangeState<EnemyShakeState>();
                break;
            case State.PullPlayer:
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
