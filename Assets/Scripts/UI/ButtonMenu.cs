using UnityEngine;
using UnityEngine.UI;

public class ButtonMenu : MonoBehaviour
{
    public Vector2Int PreferredButtonSize = new Vector2Int(100, 100);

    public enum LayoutType {Horizontal, Vertical };
    [SerializeField]
    private LayoutType _layoutType = LayoutType.Horizontal;

    private RectTransform _rect;
    private HorizontalLayoutGroup _hLayoutGroup;
    private VerticalLayoutGroup _vLayoutGroup;
    private int _childrenNumber;
    private int _horizontalPadding;
    private int _verticalPadding;
    private float _spacing;

    [SerializeField]
    private bool _startsHidden = false;

    private void Awake()
    {
        if (_startsHidden)
        {
            transform.localScale = Vector3.zero;
        }
    }

    private void Start()
    {
        _rect = GetComponent<RectTransform>();

        if (_layoutType == LayoutType.Horizontal)
            _hLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        else if (_layoutType == LayoutType.Vertical)
            _vLayoutGroup = GetComponent<VerticalLayoutGroup>();
        else
            Debug.LogError("Invalid layout type.");

        SetBackgroundSize();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
    }

    public void SetBackgroundSize()
    {
        SetPaddings();
        _childrenNumber = transform.childCount;
        float width = 0;
        float height = 0;

        if (_layoutType == LayoutType.Horizontal)
        {
            width = _childrenNumber * PreferredButtonSize.x + 2 * _horizontalPadding + _spacing * (_childrenNumber - 1);
            height = PreferredButtonSize.y + 2 * _verticalPadding;
        }
        else if (_layoutType == LayoutType.Vertical)
        {
            width = PreferredButtonSize.x + 2 * _horizontalPadding;
            height = _childrenNumber * PreferredButtonSize.y + 2 * _verticalPadding + _spacing * (_childrenNumber - 1);
        }


        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        _rect.sizeDelta = new Vector2(width, height);
    }

    private void SetPaddings()
    {
        if (_layoutType == LayoutType.Horizontal)
        {
            if (_hLayoutGroup == null)
                _hLayoutGroup = GetComponent<HorizontalLayoutGroup>();

            _horizontalPadding = _hLayoutGroup.padding.left;
            _verticalPadding = _hLayoutGroup.padding.top;
            _hLayoutGroup.padding.right = _horizontalPadding;
            _hLayoutGroup.padding.bottom = _verticalPadding;
            _spacing = _hLayoutGroup.spacing;
        }
        else if (_layoutType == LayoutType.Vertical)
        {
            if (_vLayoutGroup == null)
                _vLayoutGroup = GetComponent<VerticalLayoutGroup>();

            _horizontalPadding = _vLayoutGroup.padding.left;
            _verticalPadding = _vLayoutGroup.padding.top;
            _vLayoutGroup.padding.right = _horizontalPadding;
            _vLayoutGroup.padding.bottom = _verticalPadding;
            _spacing = _vLayoutGroup.spacing;
        }
    }
}