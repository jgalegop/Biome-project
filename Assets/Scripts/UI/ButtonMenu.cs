using UnityEngine;
using UnityEngine.UI;

public class ButtonMenu : MonoBehaviour
{
    public int PreferredButtonSize = 100;
    private RectTransform _rect;
    private HorizontalLayoutGroup _layoutGroup;
    private int _childrenNumber;
    private int _horizontalPadding;
    private int _verticalPadding;
    private float _spacing;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        
        SetBackgroundSize();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
    }

    public void SetBackgroundSize()
    {
        SetPaddings();
        _childrenNumber = transform.childCount;
        float width = _childrenNumber * PreferredButtonSize + 2 * _horizontalPadding  + _spacing * (_childrenNumber - 1);
        float height = PreferredButtonSize + 2 * _verticalPadding;

        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        _rect.sizeDelta = new Vector2(width, height);
    }

    private void SetPaddings()
    {
        if (_layoutGroup == null)
            _layoutGroup = GetComponent<HorizontalLayoutGroup>();

        _horizontalPadding = _layoutGroup.padding.left;
        _verticalPadding = _layoutGroup.padding.top;
        _layoutGroup.padding.right = _horizontalPadding;
        _layoutGroup.padding.bottom = _verticalPadding;
        _spacing = _layoutGroup.spacing;
    }
}