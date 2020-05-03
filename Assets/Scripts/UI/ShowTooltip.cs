using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string _tooltipText = "Example tooltip text";

    [SerializeField]
    private Vector2 _tooltipPosition = Vector2.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // this changes for different resolutions. Have that in mind
        Tooltip.ShowTooltip(_tooltipText, _tooltipPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip();
    }

    public void SetFloatText(float number)
    {
        _tooltipText = number.ToString("0.0");
        Tooltip.UpdateText(_tooltipText);
    }

    public void SetIntText(float number)
    {
        _tooltipText = ((int)number).ToString();
        Tooltip.UpdateText(_tooltipText);
    }

    public void SetCustomText(string text)
    {
        _tooltipText = text;
        Tooltip.UpdateText(_tooltipText);
    }
}