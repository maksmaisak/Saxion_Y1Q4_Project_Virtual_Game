using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IAgent
{

    public FSM<Enemy> fsm { get; private set; }

    // Use this for initialization
    void Start()
    {
        fsm = new FSM<Enemy>(this);
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
