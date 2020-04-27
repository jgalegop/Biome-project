using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TabGroup _tabGroup;

    public Image TabImage { get; private set; }

    private bool _isSelected = false;

    private void Start()
    {
        TabImage = GetComponent<Image>();
        _tabGroup.BindTab(this);
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
    }

    public void Select()
    {
        _isSelected = true;
    }
}
