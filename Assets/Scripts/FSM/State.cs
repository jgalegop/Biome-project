using System;
using UnityEngine;

public abstract class State
{
    protected GameObject gameObject;
    protected Transform transform;

    public string StateName { get; protected set; }
    public State (GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        StateName = null;
    }

    /// <summary>
    /// State method called in each frame through the Finite State Machine
    /// </summary>
    public abstract Type Tick();
}
