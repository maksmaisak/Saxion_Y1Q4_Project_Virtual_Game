using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{

    public FSM<Enemy> fsm { get; private set; }

    private Grappleable grappleable;

    [SerializeField] private bool thrustBehaviour;
    [SerializeField] private bool shakeBehaviour;

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
        if (thrustBehaviour)
        {
            fsm.ChangeState<EnemyThrustUpState>();
        }
        else if(shakeBehaviour)
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


}
