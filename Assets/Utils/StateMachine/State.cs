using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract string GetName();
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}