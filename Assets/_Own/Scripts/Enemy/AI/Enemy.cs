using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{
    public FSM<Enemy> fsm { get; private set; }

    private Grappleable grappleable;

    private Health health;

    private float initialHeight;

    private enum State
    {
        None,
        ThrustUp,
        Shake
    }

    [SerializeField] private State selectedState;

    // Use this for initialization
    void Start()
    {
        initialHeight = transform.position.y;

        fsm = new FSM<Enemy>(this);

        fsm.ChangeState<EnemyPatrolState>();

        grappleable = GetComponent<Grappleable>();
        health = GetComponent<Health>();

        grappleable.OnGrappled   += OnGrapple;
        grappleable.OnUngrappled += OnRelease;

        health.OnDeath += RetractOnDestroy;
    }

    private void OnGrapple(Grappleable sender)
    {
        switch (selectedState)
        {
            case State.ThrustUp:
                fsm.ChangeState<EnemyThrustUpState>();
                break;
            case State.Shake:
                fsm.ChangeState<EnemyShakeState>();
                break;
        }

        GetComponent<Shooting>().enabled = false;
    }

    private void OnRelease(Grappleable sender)
    {
        fsm.ChangeState<EnemyStateFollowPlayer>();
        GetComponent<Shooting>().enabled = true;
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

    public void RetractOnDestroy(Health sender)
    {
        foreach (var grapple in gameObject.GetComponentsInChildren<Grapple>())
        {
            grapple.Retract();
        }
    }
}
