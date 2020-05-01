using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _tooltipText = null;
    [SerializeField]
    private RectTransform _backgroundRect = null;

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
        HideThisTooltip();
    }

    private void ShowThisTooltip(string tooltipText, Vector2 pos)
    {
        gameObject.SetActive(true);

        _tooltipText.text = tooltipText;
        transform.localPosition = pos;
        _textRect.localPosition = new Vector2( _textPadding, -_textPadding);

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

    private void HideThisTooltip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip(string tooltipText, Vector2 pos)
    {
        instance.ShowThisTooltip(tooltipText, pos);
    }

    public static void HideTooltip()
    {
        instance.HideThisTooltip();
    }
}
