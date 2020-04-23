using UnityEngine;
using DG.Tweening;

public abstract class Button : MonoBehaviour
{
    private Vector3 _defaultScale;
    public virtual void Awake()
    {
        _defaultScale = transform.localScale;
    }

    public virtual void ChangeSizeOnEnter()
    {
        transform.DOScale(1.1f * _defaultScale, 0.1f)
                 .SetEase(Ease.OutQuint)
                 .SetUpdate(true);
    }

    public virtual void ChangeSizeOnExit()
    {
        transform.DOScale(0.9090909f * _defaultScale, 0.1f)
                 .SetEase(Ease.OutQuint)
                 .SetUpdate(true);
    }
}