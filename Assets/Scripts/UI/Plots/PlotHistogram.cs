using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlotHistogram : MonoBehaviour
{
    [SerializeField]
    private Image _plotArea = null;
    private RectTransform _plotAreaRect = null;

    private RectTransform _rect = null;

    private float _plotWidth;
    private float _plotHeight;

    private float _barWidth;

    [SerializeField]
    private float _barWidthFill = 0.8f;

    [SerializeField]
    private Color _barColor = Color.white;

    [SerializeField]
    private Sprite _barImage = null;

    [SerializeField]
    private Sprite _tickImage = null;
    [SerializeField]
    private Color _axisColor = Color.white;
    [SerializeField]
    private TMP_FontAsset _tickFont = null;

    private List<int> _data = new List<int>();

    private List<GameObject> _bars = new List<GameObject>();
    private List<GameObject> _yTicks = new List<GameObject>();
    private List<GameObject> _xTicks = new List<GameObject>();

    private int _maxYData;
    private int _defaultMaxYData = 12;

    private DataStatistics _dataStats = null;

    public int YAxisPrecision { get; private set; }
    public float XAxisPrecision { get; private set; }


    private void Start()
    {
        _dataStats = GetComponent<DataStatistics>();

        if (_plotArea == null && false)
        {
            Debug.LogError("Plot area image object must be assigned");
            return;
        }
        _plotAreaRect = _plotArea.GetComponent<RectTransform>();
        _rect = GetComponent<RectTransform>();

        _plotWidth = _rect.sizeDelta.x - Mathf.Abs(_plotAreaRect.offsetMin.x) - Mathf.Abs(_plotAreaRect.offsetMax.x);
        _plotHeight = _rect.sizeDelta.y - Mathf.Abs(_plotAreaRect.offsetMin.y) - Mathf.Abs(_plotAreaRect.offsetMax.y);

        _maxYData = _defaultMaxYData;

        CreateEmptyBars();

        // precisions
        YAxisPrecision = 5;
        XAxisPrecision = 2f;

        CreateAxisTicks();

        UpdateData();

        // PERHAPS THIS SHOULD BE SOMEWHERE ELSE
        DOTween.SetTweensCapacity(500, 50);
    }

    private GameObject GetBar(int xVal, int yVal)
    {
        GameObject barGO = new GameObject("Bar", typeof(Image));
        barGO.transform.SetParent(_plotArea.transform, false);
        Image barImage = barGO.GetComponent<Image>();
        barImage.color = _barColor;
        if (_barImage != null)
        {
            barImage.sprite = _barImage;
            barImage.type = Image.Type.Sliced;
            barImage.fillCenter = true;
            barImage.pixelsPerUnitMultiplier = 10f;
        }

        RectTransform barRect = barGO.GetComponent<RectTransform>();
        barRect.sizeDelta = new Vector2(_barWidth, _plotHeight * yVal / _maxYData);
        barRect.pivot = new Vector2(0.5f, 0);
        barRect.anchoredPosition = new Vector2(_plotWidth * xVal / (_dataStats.NumberOfBars + 1), 0);
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.zero;

        barRect.SetAsFirstSibling();

        return barGO;
    }

    private void UpdateBar(GameObject bar, int yVal)
    {
        RectTransform barRect = bar.GetComponent<RectTransform>();
        Vector2 newSizeDelta = new Vector2(_barWidth, _plotHeight * (float)yVal / (float)_maxYData);
        barRect.DOSizeDelta(newSizeDelta, 0.5f);
    }

    private void CreateEmptyBars()
    {
        _barWidth = (_plotWidth / _dataStats.NumberOfBars + 1) * _barWidthFill;

        for (int i = 0; i <= _dataStats.NumberOfBars; i++)
        {
            GameObject bar = GetBar(i + 1, 0);
            _bars.Add(bar);
        }
    }

    private void CreateAxisTicks()
    {
        for (int y = 0; y <= _maxYData / YAxisPrecision; y++)
        {
            _yTicks.Add(CreateTick(true, _plotHeight * (float)(y * YAxisPrecision) / (float)_maxYData, (float)(y * YAxisPrecision)));
        }
        for (float x = _dataStats.MinXAxis; x < _dataStats.MaxXAxis; x += XAxisPrecision)
        {
            _xTicks.Add(CreateTick(false, _plotWidth * (x - _dataStats.MinXAxis) / (_dataStats.MaxXAxis - _dataStats.MinXAxis), x));
        }
    }

    public void UpdateData()
    {
        if (_dataStats == null)
        {
            Debug.Log("null");
            return;
        }

        _data = _dataStats.GetData();
        bool renormalized = RenormalizeData(_data);

        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int i = 0;
        foreach (int y in _data)
        {
            GameObject bar = _bars[i];
            UpdateBar(bar, y);
            if (renormalized)
                UpdateYTicks();
            i++;
        }
    }

    private bool RenormalizeData(List<int> data)
    {
        bool renormalized = false;
        int maxValue = 0;
        foreach (int d in data)
        {
            if (d > maxValue)
                maxValue = d;
        }

        if (maxValue > _maxYData)
        {
            _maxYData = maxValue;
            renormalized = true;
        }
        else if (maxValue < _defaultMaxYData && _maxYData != _defaultMaxYData)
        {
            _maxYData = _defaultMaxYData;
            renormalized = true;
        }
        else if (maxValue < 0.5f * (float)_maxYData && maxValue > _defaultMaxYData)
        {
            _maxYData = (int)(0.5f * (float)_maxYData);
            renormalized = true;
        }

        return renormalized;
    }

    private GameObject CreateTick(bool isVerticalAxis, float axisVal, float val)
    {
        GameObject tickGO = new GameObject("Tick", typeof(Image));
        tickGO.transform.SetParent(_plotArea.transform, false);
        Image tickImage = tickGO.GetComponent<Image>();
        tickImage.color = _axisColor;

        if (_tickImage != null)
        {
            tickImage.sprite = _tickImage;
            tickImage.type = Image.Type.Sliced;
            tickImage.fillCenter = true;
            tickImage.pixelsPerUnitMultiplier = 20f;
        }

        RectTransform tickRect = tickGO.GetComponent<RectTransform>();
        if (isVerticalAxis)
        {
            tickRect.sizeDelta = new Vector2(15f, 5f);
            tickRect.pivot = new Vector2(0, 0f);
            tickRect.anchoredPosition = new Vector2(0, axisVal);
        }
        else
        {
            tickRect.sizeDelta = new Vector2(5f, 15f);
            tickRect.pivot = new Vector2(0, 0);
            tickRect.anchoredPosition = new Vector2(axisVal, 0);
        }
        tickRect.anchorMin = Vector2.zero;
        tickRect.anchorMax = Vector2.zero;

        CreateTickText(isVerticalAxis, val, tickGO);

        return tickGO;
    }

    private void UpdateYTicks()
    {
        int y = 0;

        List<GameObject> ticksToRemove = new List<GameObject>();

        foreach (GameObject tick in _yTicks)
        {
            float value = _plotHeight * (float)(y * YAxisPrecision) / (float)_maxYData;
            tick.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, value);

            // probably generates garbage, better from a pool
            if (y * YAxisPrecision >= _maxYData)
            {
                Destroy(tick);
                ticksToRemove.Add(tick);
            }
            y++;
        }

        foreach (GameObject tick in ticksToRemove)
        {
            _yTicks.Remove(tick);
        }

        if (y * YAxisPrecision < _maxYData)
        {
            for (int yy = y; yy <= _maxYData / YAxisPrecision; yy++)
            {
                _yTicks.Add(CreateTick(true, _plotHeight * (float)(yy * YAxisPrecision) / (float)_maxYData, (float)(yy * YAxisPrecision)));
            }
        }
    }

    private GameObject CreateTickText(bool isVerticalAxis, float val, GameObject parent)
    {
        GameObject textGO = new GameObject("Text", typeof(TextMeshProUGUI));
        TextMeshProUGUI textTMPro = textGO.GetComponent<TextMeshProUGUI>();
        textTMPro.color = _axisColor;
        textTMPro.enableAutoSizing = true;
        textTMPro.fontSizeMin = 12f;
        textTMPro.font = _tickFont;
        textTMPro.enableWordWrapping = false;

        if (val != (int)val)
            textTMPro.text = val.ToString("0.0");
        else
            textTMPro.text = val.ToString();

        textGO.transform.SetParent(parent.transform);

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        if (isVerticalAxis)
        {
            textRect.sizeDelta = new Vector2(15f, 8f);
            textRect.pivot = new Vector2(1, 0.25f);
            textRect.anchoredPosition = new Vector2(-3, 0);
            textTMPro.alignment = TextAlignmentOptions.MidlineRight;
            textRect.anchorMax = Vector2.zero;
        }
        else
        {
            textRect.sizeDelta = new Vector2(15f, 8f);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = new Vector2(0, -3);
            textTMPro.alignment = TextAlignmentOptions.Center;
            textRect.anchorMax = Vector2.right;
        }
        textRect.anchorMin = Vector2.zero;
        
        textRect.localScale = Vector3.one;

        return textGO;
    }

    public void SetAxisPrecision(float precision, bool isVerticalAxis)
    {
        if (isVerticalAxis)
            YAxisPrecision = (int)precision;
        else
            XAxisPrecision = precision;
    }
}