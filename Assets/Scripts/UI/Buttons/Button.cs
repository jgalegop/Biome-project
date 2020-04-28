using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Color _idleColor = Color.white;
    [SerializeField]
    private Color _hoverColor = Color.white;
    [SerializeField]
    private Color _pressColor = Color.white;

    [SerializeField]
    private UnityEvent _onButtonClick = null;

    public Image TabImage { get; private set; }
    public Image InteriorImage { get; private set; }

    private Vector3 _defaultImageSize;

    private void Start()
    {
        TabImage = GetComponent<Image>();
        InteriorImage = transform.GetChild(0).GetComponent<Image>(); // GetComponentInChildren searches first in parent
        _defaultImageSize = InteriorImage.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TabImage.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TabImage.color = _idleColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (DOTween.IsTweening(InteriorImage.transform))
        {
            DOTween.Kill(InteriorImage.transform);
            InteriorImage.transform.localScale = _defaultImageSize;
        }
        InteriorImage.transform.DOPunchScale(0.2f * _defaultImageSize, 0.3f, 2, 0.5f);

        _onButtonClick?.Invoke();
    }

    public void ChangeInImageColor(Color color)
    {
        InteriorImage.color = color;
    }
}