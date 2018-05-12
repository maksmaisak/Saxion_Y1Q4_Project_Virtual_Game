using System;
using UnityEngine;

public class FSMState<T> : MonoBehaviour where T : class, IAgent
{
    protected T agent;

    public void SetAgent(T agent)
    {

        Debug.Assert(this.agent == null);
        this.agent = agent;
    }

    public virtual void Enter()
    {

        agent.Print("entered state:" + this);
        enabled = true;
    }

    public virtual void Exit()
    {

        agent.Print("exited state:" + this);
        enabled = false;
    }
}


