using System;
using UnityEngine;

public abstract class OnConditionDisabler : MonoBehaviour
{
    public event Action OnDisableCondition = delegate { };

    public virtual void ConditionMet()
    {
        OnDisableCondition?.Invoke();
    }
}
