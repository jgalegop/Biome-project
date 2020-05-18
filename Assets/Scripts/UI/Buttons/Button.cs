using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using System;

[RequireComponent(typeof(Image))]
public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Color _idleColor = Color.white;
    [SerializeField]
    private Color _hoverColor = Color.white;
    [SerializeField]
    private Color _disabledColor = Color.white;
    [SerializeField]
    private Color _innerImageDisabledColor = Color.white;
    private Color _innerImageIdleColor = Color.white;

    [SerializeField]
    private UnityEvent _onButtonClick = null;

    public Image TabImage { get; private set; }
    public Image InteriorImage { get; private set; }
    public TextMeshProUGUI InteriorText { get; private set; }

    private Vector3 _defaultImageSize;
    private Vector3 _defaultTextSize;
    private Color _innerTextIdleColor = Color.white;

    public bool IsDisabled { get; private set; }

    private OnConditionDisabler _disabler = null;
    private ShowTooltip _showTooltip = null;

    private void Start()
    {
        IsDisabled = false;
        TabImage = GetComponent<Image>();
        InteriorImage = transform.GetChild(0).GetComponent<Image>(); // GetComponentInChildren searches first in parent
        if (InteriorImage != null)
        {
            _defaultImageSize = InteriorImage.transform.localScale;
            _innerImageIdleColor = InteriorImage.color;
        }

        InteriorText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (InteriorText != null)
        {
            _defaultTextSize = InteriorText.transform.localScale;
            _innerTextIdleColor = InteriorText.color;
        }

        _disabler = GetComponent<OnConditionDisabler>();
        if (_disabler != null)
            _disabler.OnDisableCondition += DisableButton;

        _showTooltip = GetComponent<ShowTooltip>();
    }

    private void OnDestroy()
    {
        if (_disabler != null)
            _disabler.OnDisableCondition -= DisableButton;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsDisabled)
            TabImage.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsDisabled)
            TabImage.color = _idleColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsDisabled)
            return;

        if (InteriorImage != null)
        {
            if (DOTween.IsTweening(InteriorImage.transform))
            {
                DOTween.Kill(InteriorImage.transform);
                InteriorImage.transform.localScale = _defaultImageSize;
            }
            InteriorImage.transform.DOPunchScale(0.2f * _defaultImageSize, 0.3f, 2, 0.5f)
                                   .SetUpdate(true); ;
        }
        else if (InteriorText != null)
        {
            if (DOTween.IsTweening(InteriorText.transform))
            {
                DOTween.Kill(InteriorText.transform);
                InteriorText.transform.localScale = _defaultTextSize;
            }
            InteriorText.transform.DOPunchScale(0.2f * _defaultTextSize, 0.3f, 2, 0.5f)
                                  .SetUpdate(true); ;
        }

        _onButtonClick?.Invoke();
    }

    public void ChangeInImageColor(Color color)
    {
        InteriorImage.color = color;
    }

    public void DisableButton()
    {
        IsDisabled = true;
        TabImage.color = _disabledColor;
        if (InteriorImage != null)
            InteriorImage.color = _innerImageDisabledColor;
        else if (InteriorText != null)
            InteriorText.color = _innerImageDisabledColor;

        if (_showTooltip != null)
        {
            _showTooltip.AddDisabledText();
        }
    }

    public void ReenableButton()
    {
        IsDisabled = false;
        TabImage.color = _idleColor;
        InteriorImage.color = _innerImageIdleColor;
        if (_showTooltip != null)
        {
            _showTooltip.RemoveDisabledText();
        }
    }
}