
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    State currentState;

    public void Start()
    {
        currentState = GetInitialState();
        currentState.Enter();
    }

    public void Update()
    {
        currentState.Update();
    }

    public void LateUpdate()
    {
        currentState.FixedUpdate();
    }

    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void OnGUI()
    {
        string content = currentState != null ? currentState.GetName() : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    protected virtual State GetInitialState()
    {
        return null;
    }
}