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
        Tooltip.ShowTooltip(_tooltipText, _tooltipPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip();
    }
}
