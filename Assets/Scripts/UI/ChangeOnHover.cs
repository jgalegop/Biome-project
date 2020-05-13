using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ChangeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color _onHoverColor = Color.white;
    private Color _defaultColor = Color.white;
    private Image _image = null;

    private void Start()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = _onHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = _defaultColor;
    }

    public void SetHoverColor(Color hoverColor)
    {
        _onHoverColor = hoverColor;
    }
}
