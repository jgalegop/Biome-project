using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class NoLandWarning : MonoBehaviour
{
    [SerializeField]
    private float _stayTime = 5f;
    [SerializeField]
    private float _fadeTime = 2f;
    private Image _background = null;
    private TextMeshProUGUI _TMProText = null;

    private bool _reducingAlpha = false;
    private Color _fontColor;
    private Color _backgroundColor;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _backgroundColor = _background.color;
        _TMProText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _fontColor = _TMProText.color;
    }

    private void OnEnable()
    {
        transform.DOScale(1, 0.3f);
        StayActive();
    }

    private void Update()
    {
        if (_reducingAlpha)
        {
            float alpha = _background.color.a;
            _TMProText.color = new Color(_fontColor.r, _fontColor.g, _fontColor.b, alpha);
        }
    }

    private void StayActive()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(_stayTime);
        _reducingAlpha = true;
        _background.DOFade(0, _fadeTime).OnComplete(CompleteFade);
    }

    private void CompleteFade()
    {
        gameObject.SetActive(false);
        _background.color = _backgroundColor;
        _TMProText.color = _fontColor;
        _reducingAlpha = false;
    }
}