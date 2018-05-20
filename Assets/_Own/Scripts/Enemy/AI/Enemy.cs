using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#pragma warning disable 0649

[RequireComponent(typeof(EnemyAudio), typeof(SteeringManager), typeof(Grappleable))]
[RequireComponent(typeof(ShootingController))]
public class Enemy : MonoBehaviour, IAgent
{
    private static List<SteeringManager> steeringManagers = new List<SteeringManager>();
    public static IEnumerable<SteeringManager> allAsSteerables { get { return steeringManagers.AsReadOnly(); } }

    [SerializeField] private GrappleReactionBehaviour grappleReactionBehaviour;
    [SerializeField] private GameObject grappledParticleGroup;

    public FSM<Enemy> fsm { get; private set; }

    new public EnemyAudio audio { get; private set; }
    public ParticleManager particleManager { get; private set; }
    public SteeringManager steering { get; private set; }
    public ShootingController shootingController { get; private set; }

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

        steering = GetComponent<SteeringManager>();
        steeringManagers.Add(steering);

        shootingController = GetComponent<ShootingController>();

        var grappleable = GetComponent<Grappleable>();
        grappleable.OnGrappled += OnGrapple;
        grappleable.OnUngrappled += OnRelease;

        fsm.ChangeState<EnemyMoveRandomlyAroundPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    void OnDestroy()
    {
        steeringManagers.Remove(steering);
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

    public void Print(string message)
    {
        Debug.Log(message);
    }

    public float GetInitialHeight()
    {
        return initialHeight;
    }
}
