using UnityEngine;

public abstract class Button : MonoBehaviour
{
    public virtual void ChangeSizeOnEnter()
    {
        transform.localScale *= 1.1f;
    }

    public virtual void ChangeSizeOnExit()
    {
        transform.localScale *= 1 / 1.1f;
    }
}