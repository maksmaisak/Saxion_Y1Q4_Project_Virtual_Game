using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{

    public FSM<Enemy> fsm { get; private set; }

    private Grappleable grappleable;

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
        fsm.ChangeState<EnemyGrappledState>();
    }

    private void OnRelease(Grappleable sender)
    {
        fsm.ChangeState<EnemyStateFollowPlayer>();
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


}
