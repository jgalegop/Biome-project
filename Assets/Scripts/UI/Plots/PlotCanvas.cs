using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlotCanvas : MonoBehaviour
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

    private List<int> _data = new List<int>();

    private List<GameObject> _bars = new List<GameObject>();

    private int _maxYData;
    private int _defaultMaxYData = 12;

    private DataStatistics _dataStats = null;


    private void Awake()
    {
        _dataStats = GetComponent<DataStatistics>();

        if (_plotArea == null)
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

    public void UpdateData()
    {
        if (_dataStats == null)
        {
            Debug.Log("null");
            return;
        }

        _data = _dataStats.GetData();
        RenormalizeData(_data);

        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int i = 0;
        foreach (int y in _data)
        {
            GameObject bar = _bars[i];
            UpdateBar(bar, y);
            i++;
        }
    }

    private void RenormalizeData(List<int> data)
    {
        int maxValue = 0;
        foreach (int d in data)
        {
            if (d > maxValue)
                maxValue = d;
        }

        if (maxValue > _maxYData)
        {
            _maxYData = maxValue;
        }
        else
        {
            _maxYData = _defaultMaxYData;
        }
    }

    private void SetData()
    {
        if (_dataStats == null)
        {
            Debug.Log("null");
            return;
        }

        _data = _dataStats.GetData();
        _maxYData = 10;
        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int x = 0;
        foreach (int y in _data)
        {
            x++;
            GameObject bar = GetBar(x, y);
            _bars.Add(bar);
        }
    }
}