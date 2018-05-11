using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{

    public FSM<Enemy> fsm { get; private set; }

    private Grappleable grappleable;

    private enum State
    {
        NONE,
        THRUSTUP,
        SHAKE
    }

    [SerializeField] private State selectedState;

    // Use this for initialization
    void Start()
    {
        fsm = new FSM<Enemy>(this);
        fsm.ChangeState<EnemyStateFollowPlayer>();

        grappleable = GetComponent<Grappleable>();

        grappleable.OnGrappled += OnGrapple;
        grappleable.OnUngrappled += OnRelease;
    }

    private void OnGrapple(Grappleable sender)
    {
        if (selectedState == State.THRUSTUP)
        {
            fsm.ChangeState<EnemyThrustUpState>();
        }
        else if(selectedState == State.SHAKE)
        {
            fsm.ChangeState<EnemyShakeState>();
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
}
