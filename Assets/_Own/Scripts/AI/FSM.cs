using System.Collections.Generic;
using System;
using UnityEngine;

public class FSM<AgentT> where AgentT : Component, IAgent
{

    // Maps the class name of a state to a specific instance of that state
    private Dictionary<Type, FSMState<AgentT>> stateCache;

    // The current state we are in
    private FSMState<AgentT> currentState;

    // Reference to our target so we can pass into our new states.
    private AgentT agent;

    public FSM(AgentT agent)
    {

        this.agent = agent;

        stateCache = new Dictionary<Type, FSMState<AgentT>>();
        DetectExistingStates();
    }

    public void Update()
    {
        //if (currentState != null) Debug.Log("Executing state " + currentState);
    }

    public FSMState<AgentT> GetCurrentState()
    {
        return currentState;
    }

    public void Reset()
    {
        if (currentState != null)
        {
            currentState.Exit();
            currentState.Enter();
        }
    }

    /**
	 * Tells the FSM to enter a state which is a subclass of AbstractState<T>.
	 * So for exampe for FSM<Bob> the state entered must be a subclass of AbstractState<Bob>
	 */
    public void ChangeState<StateT>() where StateT : FSMState<AgentT>
    {
        // Check if a state like this was already in our cache
        if (!stateCache.ContainsKey(typeof(StateT)))
        {
            // If not, create it, passing in the target
            StateT state = agent.gameObject.AddComponent<StateT>();
            state.SetAgent(agent);
            stateCache[typeof(StateT)] = state;
            ChangeState(state);
        }
        else
        {
            FSMState<AgentT> newState = stateCache[typeof(StateT)];
            ChangeState(newState);
        }
    }

    private void ChangeState(FSMState<AgentT> newState)
    {
        if (currentState == newState) return;

        if (currentState != null) currentState.Exit();
        currentState = newState;
        if (currentState != null) currentState.Enter();
    }

    private void DetectExistingStates()
    {
        FSMState<AgentT>[] states = agent.GetComponentsInChildren<FSMState<AgentT>>();
        foreach (FSMState<AgentT> state in states)
        {
            state.enabled = false;
            state.SetAgent(agent);
            stateCache.Add(state.GetType(), state);
        }
    }
}
