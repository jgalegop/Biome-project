using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tooltipText = null;
    [SerializeField]
    private RectTransform _backgroundRect = null;
    private Image _bgImage = null;

    [SerializeField]
    private Color _defaultColor = Color.white;
    [SerializeField]
    private Color _alternativeColor = Color.white;
    [SerializeField]
    private Color _warningColor = Color.white;

    [SerializeField]
    private float _textPadding = 8f;
    [SerializeField]
    private float _maxWidth = 250f;

    private RectTransform _textRect;

    private static Tooltip instance;

    private void Awake()
    {
        instance = this;
        _textRect = _tooltipText.rectTransform;
        _bgImage = _backgroundRect.GetComponent<Image>();
        _bgImage.color = _defaultColor;
        HideThisTooltip();
    }

    private void ShowThisTooltip(string tooltipText, Vector2 pos)
    {
        gameObject.SetActive(true);

        transform.localPosition = pos;
        _textRect.localPosition = new Vector2( _textPadding, -_textPadding);

        UpdateTooltip(tooltipText);
    }

    private void HideThisTooltip()
    {
        if (_bgImage.color != _defaultColor)
            _bgImage.color = _defaultColor;
        gameObject.SetActive(false);
    }

    public static void ShowTooltip(string tooltipText, Vector2 pos)
    {
        instance.ShowThisTooltip(tooltipText, pos);
    }

    public static void ShowTooltip(string tooltipText)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(instance.transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out pos);
        instance.ShowThisTooltip(tooltipText, pos + 0.5f * instance._backgroundRect.sizeDelta);
    }

    public static void SetAlternativeColor()
    {
        instance._bgImage.color = instance._alternativeColor;
    }
    public static void SetWarningColor()
    {
        instance._bgImage.color = instance._warningColor;
    }

    public static void HideTooltip()
    {
        instance.HideThisTooltip();
    }

    private void UpdateThisText(string newText)
    {
        UpdateTooltip(newText);
    }

    public static void UpdateText(string newText)
    {
        instance.UpdateThisText(newText);
    }

    private void UpdateTooltip(string tooltipText)
    {
        _tooltipText.text = tooltipText;
        Vector2 backgroundSize;
        _tooltipText.enableWordWrapping = false;
        if (_tooltipText.preferredWidth - 2f * _textPadding < _maxWidth)
        {
            backgroundSize = new Vector2(_tooltipText.preferredWidth + 2f * _textPadding, _tooltipText.preferredHeight + 2f * _textPadding);
        }
        else
        {
            _tooltipText.enableWordWrapping = true;
            _textRect.sizeDelta = new Vector2(_maxWidth - 2f * _textPadding, 0);
            backgroundSize = new Vector2(_maxWidth, _tooltipText.preferredHeight + 2f * _textPadding);
        }

        _backgroundRect.sizeDelta = backgroundSize;
    }
}
