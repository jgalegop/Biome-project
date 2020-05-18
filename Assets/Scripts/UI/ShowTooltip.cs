using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string _tooltipText = "Example tooltip text";
    private string _defaultTooltipText;

    [SerializeField]
    private Vector2 _tooltipPosition = Vector2.zero;

    [SerializeField]
    private bool _followPointerPosition = false;
    [SerializeField]
    private bool _useAltColor = false;
    [SerializeField]
    private bool _warning = false;

    private bool _pointerInObject = false;

    private void Start()
    {
        _defaultTooltipText = _tooltipText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerInObject = true;
        if (_useAltColor)
            Tooltip.SetAlternativeColor();
        if (_warning)
            Tooltip.SetWarningColor();

        // this changes for different resolutions. Have that in mind
        if (_followPointerPosition)
        {
            Tooltip.ShowTooltip(_tooltipText);
        }
        else
        {
            Tooltip.ShowTooltip(_tooltipText, _tooltipPosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerInObject = false;
        Tooltip.HideTooltip();
    }

    public void SetFloatText(float number)
    {
        _tooltipText = number.ToString("0.0");
        Tooltip.UpdateText(_tooltipText);
    }

    public void UpdateText()
    {
        if (_pointerInObject)
        {
            Tooltip.UpdateText(_tooltipText);
        }
    }

    public void SetIntText(float number)
    {
        _tooltipText = ((int)number).ToString();
        UpdateText();
    }

    public void SetCustomText(string text)
    {
        _tooltipText = text;
        UpdateText();
    }

    public void AddDisabledText()
    {
        _tooltipText += " (disabled)";
        UpdateText();
    }

    public void RemoveDisabledText()
    {
        _tooltipText = _defaultTooltipText;
        UpdateText();
    }

    public void SetOptions(bool followMouse, bool altColor)
    {
        _followPointerPosition = followMouse;
        _useAltColor = altColor;
    }
}