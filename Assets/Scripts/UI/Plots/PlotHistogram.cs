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

    private HistogramData _dataStats = null;

    private float _plotWidth;
    private float _plotHeight;
    private float _barWidth;

    [Header ("Bar properties")]
    [SerializeField]
    private float _barWidthFill = 0.8f;
    [SerializeField]
    private Color _barColor = Color.white;
    [SerializeField]
    private Sprite _barImage = null;

    [Header ("Ticks and axes")]
    [SerializeField]
    private Sprite _tickImage = null;
    [SerializeField]
    private TMP_FontAsset _tickFont = null;
    [SerializeField]
    private Color _axisColor = Color.white;    

    private List<int> _data = new List<int>();
    private List<GameObject> _bars = new List<GameObject>();
    private List<GameObject> _yTicks = new List<GameObject>();
    private List<GameObject> _xTicks = new List<GameObject>();
    
    private int _yAxisPrecision;
    private float _xAxisPrecision;

    private const string _barName = "Bar";
    private const string _tickName = "Tick";

    private List<GameObject> _elementsParents = new List<GameObject>();


    private void Start()
    {
        _dataStats = GetComponent<HistogramData>();

        if (_plotArea == null)
        {
            Debug.LogError("Plot area image object must be assigned");
            return;
        }
        _plotAreaRect = _plotArea.GetComponent<RectTransform>();
        _rect = GetComponent<RectTransform>();

        _plotWidth = _rect.sizeDelta.x - Mathf.Abs(_plotAreaRect.offsetMin.x) - Mathf.Abs(_plotAreaRect.offsetMax.x);
        _plotHeight = _rect.sizeDelta.y - Mathf.Abs(_plotAreaRect.offsetMin.y) - Mathf.Abs(_plotAreaRect.offsetMax.y);

        CreatePlotElementParents();

        CreateEmptyBars();

        // precisions
        _yAxisPrecision = 5;
        _xAxisPrecision = 2f;

        CreateAxisTicks();

        UpdateData();

        // PERHAPS THIS SHOULD BE SOMEWHERE ELSE
        DOTween.SetTweensCapacity(500, 50);
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

    private GameObject GetBar(int xVal, int yVal)
    {
        GameObject barGO = new GameObject(_barName, typeof(Image), typeof(Canvas));
        SetElementParent(barGO);
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
        barRect.sizeDelta = new Vector2(_barWidth, _plotHeight * yVal / _dataStats.MaxYData);
        barRect.pivot = new Vector2(0.5f, 0);
        barRect.anchoredPosition = new Vector2(_plotWidth * xVal / (_dataStats.NumberOfBars + 1), 0);
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.zero;

        return barGO;
    }

    private void CreateAxisTicks()
    {
        for (int y = 0; y <= _dataStats.MaxYData / _yAxisPrecision; y++)
        {
            _yTicks.Add(CreateTick(true, _plotHeight * (float)(y * _yAxisPrecision) / (float)_dataStats.MaxYData, (float)(y * _yAxisPrecision)));
        }
        for (float x = _dataStats.MinXAxis; x < _dataStats.MaxXAxis; x += _xAxisPrecision)
        {
            _xTicks.Add(CreateTick(false, _plotWidth * (x - _dataStats.MinXAxis) / (_dataStats.MaxXAxis - _dataStats.MinXAxis), x));
        }
    }

    private GameObject CreateTick(bool isVerticalAxis, float axisVal, float val)
    {
        GameObject tickGO = new GameObject(_tickName, typeof(Image), typeof(Canvas));
        SetElementParent(tickGO);
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
            textTMPro.text = val.ToString("00.0");
        else
        {
            if (isVerticalAxis)
            {
                textTMPro.text = val.ToString("0");
            }
            else
            {
                if (val < 10)
                    textTMPro.text = val.ToString("0");
                else
                {
                    Debug.Log("why does this not work :(");
                    textTMPro.text = val.ToString("00");
                }
            }
        }

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

    public void UpdateData()
    {
        if (_dataStats == null)
        {
            Debug.Log("null");
            return;
        }

        _data = _dataStats.GetData();
        bool renormalized = _dataStats.RenormalizeData();

        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int i = 0;
        foreach (int y in _data)
        {
            UpdateBar(_bars[i], y);
            if (renormalized)
                UpdateYTicks();
            if (_dataStats.ChangeXAxis)
                UpdateXTicks();
            i++;
        }
    }

    private void UpdateBar(GameObject bar, int yVal)
    {
        RectTransform barRect = bar.GetComponent<RectTransform>();
        Vector2 newSizeDelta = new Vector2(_barWidth, _plotHeight * (float)yVal / (float)_dataStats.MaxYData);
        barRect.DOSizeDelta(newSizeDelta, 0.5f);
    }

    private void UpdateYTicks()
    {
        int y = 0;

        List<GameObject> ticksToRemove = new List<GameObject>();

        foreach (GameObject tick in _yTicks)
        {
            float value = _plotHeight * (float)(y * _yAxisPrecision) / (float)_dataStats.MaxYData;
            tick.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, value);

            // probably generates garbage, better from a pool
            if (y * _yAxisPrecision >= _dataStats.MaxYData)
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

        if (y * _yAxisPrecision < _dataStats.MaxYData)
        {
            for (int yy = y; yy <= _dataStats.MaxYData / _yAxisPrecision; yy++)
            {
                _yTicks.Add(CreateTick(true, _plotHeight * (float)(yy * _yAxisPrecision) / (float)_dataStats.MaxYData, (float)(yy * _yAxisPrecision)));
            }
        }
    }


    private void UpdateXTicks()
    {
        float x = _dataStats.MinXAxis;

        List<GameObject> ticksToRemove = new List<GameObject>();

        foreach (GameObject tick in _xTicks)
        {
            float value = _plotWidth * (x - _dataStats.MinXAxis) / (_dataStats.MaxXAxis - _dataStats.MinXAxis);
            tick.GetComponent<RectTransform>().anchoredPosition = new Vector2(value, 0);

            // probably generates garbage, better from a pool
            if (x >= _dataStats.MaxXAxis)
            {
                Destroy(tick);
                ticksToRemove.Add(tick);
            }
            x += _xAxisPrecision;
        }

        foreach (GameObject tick in ticksToRemove)
        {
            _xTicks.Remove(tick);
        }

        if (x  < _dataStats.MaxXAxis)
        {
            for (float xx = _dataStats.MinXAxis; xx < _dataStats.MaxXAxis; xx += _xAxisPrecision)
            {
                _xTicks.Add(CreateTick(false, _plotWidth * (xx - _dataStats.MinXAxis) / (_dataStats.MaxXAxis - _dataStats.MinXAxis), xx));
            }
        }
    }



    //public void SetAxisPrecision(float precision, bool isVerticalAxis)
    //{
    //    if (isVerticalAxis)
    //        _yAxisPrecision = (int)precision;
    //    else
    //        _xAxisPrecision = precision;
    //}

    private void CreatePlotElementParents()
    {
        var parentNames = new List<string> { _barName + "s", _tickName + "s" };
        foreach (string s in parentNames)
        {
            GameObject parent = new GameObject(s, typeof(RectTransform));
            RectTransform rect = parent.GetComponent<RectTransform>();
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            parent.transform.SetParent(_plotArea.transform, false);
            _elementsParents.Add(parent);
        }
    }

    private void SetElementParent(GameObject element)
    {
        GameObject parent = _elementsParents.Find((x) => x.name == element.name + "s");
        element.transform.SetParent(parent.transform, false);
    }
}