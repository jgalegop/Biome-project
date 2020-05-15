using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TabGroup _tabGroup = null;

    [SerializeField]
    private Color _disabledColor = Color.white;
    [SerializeField]
    private Color _spriteDisabledColor = Color.white;

    public Image TabImage { get; private set; }
    public Image InteriorImage { get; private set; }

    [SerializeField]
    private bool _startsSelected = false;

    [SerializeField]
    private bool _cantBeUnselected = false;

    private bool _isSelected = false;
    private Vector3 _defaultImageSize;

    public bool IsDisabled { get; private set; }

    private OnConditionDisabler _disabler = null;
    private ShowTooltip _showTooltip = null;

    private void Start()
    {
        TabImage = GetComponent<Image>();
        InteriorImage = transform.GetChild(0).GetComponent<Image>(); // GetComponentInChildren searches first in parent
        _defaultImageSize = InteriorImage.transform.localScale;
        _tabGroup.BindTab(this);

        if (_startsSelected)
            _tabGroup.OnTabSelected(this);

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
            _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsDisabled)
            _tabGroup.OnTabExit(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsDisabled)
            return;

        if (!_isSelected)
        {
            _tabGroup.OnTabSelected(this);
        }
        else if (_isSelected && !_cantBeUnselected)
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

    public void DisableButton()
    {
        if (!IsDisabled)
        {
            IsDisabled = true;
            TabImage.color = _disabledColor;
            InteriorImage.color = _spriteDisabledColor;
            if (_showTooltip != null)
            {
                _showTooltip.AddDisabledText();
            }
        }
    }

    public void ReenableButton()
    {
        //IsDisabled = false;
        //TabImage.color = _idleColor;
        //InteriorImage.color = _innerImageIdleColor;
    }
}
