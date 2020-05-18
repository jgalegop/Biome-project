using UnityEngine;
using DG.Tweening;

public class ResetWarning : MonoBehaviour
{
    [SerializeField]
    private SceneController _sceneController = null;

    [SerializeField]
    private GameObject _resetWarning = null;

    private float _tweenTime = 0.4f;

    public void EnableResetWarning()
    {
        if (!_resetWarning.activeSelf)
        {
            _resetWarning.SetActive(true);
            _resetWarning.transform.DOScale(1, _tweenTime)
                                   .SetEase(Ease.OutBack)
                                   .SetUpdate(true);
        }
    }

    public void DisableResetWarning()
    {
        if (_resetWarning.activeSelf)
        {
            _resetWarning.transform.DOScale(0, _tweenTime)
                                   .SetEase(Ease.InBack)
                                   .SetUpdate(true)
                                   .OnComplete(DisableOnComplete);
        }
    }

    private void DisableOnComplete()
    {
        _resetWarning.SetActive(false);
    }

    public void ConfirmReset()
    {
        DOTween.KillAll();
        _sceneController.ResetScene();
    }
}