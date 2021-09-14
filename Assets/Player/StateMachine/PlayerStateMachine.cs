using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public PlayerState_Grounded groundedState;
    public PlayerState_Airborne airborneState;
    public PlayerState_Wipeout wipeoutState;

    [HideInInspector]
    public Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;
        groundedState = new PlayerState_Grounded(this);
        airborneState = new PlayerState_Airborne(this);
        wipeoutState = new PlayerState_Wipeout(this);
    }

    protected override State GetInitialState()
    {
        return airborneState;
    }

    protected override void OnStateChanged(State oldState, State newState)
    {
        if (oldState != null)
        {
            Debug.Log("State change: " + oldState.GetName() + " to " + newState.GetName());
        }
    }
}
