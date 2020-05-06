using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TabGroup _tabGroup = null;

    public Image TabImage { get; private set; }
    public Image InteriorImage { get; private set; }

    [SerializeField]
    private bool _startsSelected = false;

    private bool _isSelected = false;
    private Vector3 _defaultImageSize;

    private void Start()
    {
        TabImage = GetComponent<Image>();
        InteriorImage = transform.GetChild(0).GetComponent<Image>(); // GetComponentInChildren searches first in parent
        _defaultImageSize = InteriorImage.transform.localScale;
        _tabGroup.BindTab(this);

        if (_startsSelected)
            _tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _tabGroup.OnTabSelected(this);
        }
        else
        {
            _tabGroup.OnTabDeselected(this);
        }
    }

    public void Deselect()
    {
        _isSelected = false;
        InteriorImage.transform.localScale = _defaultImageSize;
    }

    public void Select()
    {
        _isSelected = true;
        InteriorImage.transform.localScale = 1.1f * _defaultImageSize;
    }

    public void ChangeInImageColor(Color color)
    {
        InteriorImage.color = color;
    }
}
