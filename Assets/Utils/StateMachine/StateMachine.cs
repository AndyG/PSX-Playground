
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State currentState;

    public void Start()
    {
        currentState = GetInitialState();
        currentState.Enter();
    }

    public void Update()
    {
        currentState.Update();
    }

    public void ChangeState(State newState)
    {
        OnStateChanged(currentState, newState);
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void OnGUI()
    {
        // string content = currentState != null ? currentState.GetName() : "(no current state)";
        // GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    protected virtual State GetInitialState()
    {
        return null;
    }

    protected virtual void OnStateChanged(State oldState, State newState)
    {

    }
}