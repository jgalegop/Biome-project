using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class FiniteStateMachine : MonoBehaviour
{
    private Dictionary<Type, State> _availableStates;

    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

    public event Action<State> OnStateChanged;

    // Set the states of the FSM
    public void SetStates(Dictionary<Type, State> states)
    {
        _availableStates = states;
    }

    private void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = _availableStates.Values.First();

            // make sure it's not getting a null from an empty dictionary
            if (CurrentState == null)
            {
                return;
            }
        }

        var nextState = CurrentState.Tick();
        PreviousState = CurrentState;

        if (nextState != null &&
            nextState != CurrentState.GetType())
        {
            SwitchToNewState(nextState);
        }
    }

    private void SwitchToNewState(Type nextState)
    {
        PreviousState = CurrentState;
        CurrentState = _availableStates[nextState];
        if (OnStateChanged != null)
        {
            OnStateChanged.Invoke(CurrentState);
        }
    }
}
