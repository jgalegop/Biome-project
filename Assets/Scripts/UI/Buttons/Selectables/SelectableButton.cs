using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class SelectableButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private SelectableGroup _selectablesGroup = null;
    [SerializeField]
    private UnityEvent _onButtonSelected = null;
    [SerializeField]
    private bool _selectedByDefault = false;

    public Image ButtonImage { get; private set; }
    public Image InteriorImage { get; private set; }

    private bool _isSelected = false;
    private Vector3 _defaultImageSize;

    private void Start()
    {
        ButtonImage = GetComponent<Image>();
        InteriorImage = transform.GetChild(0).GetComponent<Image>(); // GetComponentInChildren searches first in parent
        _defaultImageSize = InteriorImage.transform.localScale;
        _selectablesGroup.BindSelectable(this);
        if (_selectedByDefault)
        {
            _selectablesGroup.OnButtonSelected(this);
            _onButtonSelected?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _selectablesGroup.OnButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selectablesGroup.OnButtonExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            _selectablesGroup.OnButtonSelected(this);
            _onButtonSelected?.Invoke();
        }
        else
        {
            _selectablesGroup.OnButtonDeselected(this);
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
