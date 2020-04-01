using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlotCanvas : MonoBehaviour
{
    [SerializeField]
    private Image _plotArea = null;

    private float _plotWidth;
    private float _plotHeight;

    private float _barWidth;

    [SerializeField]
    private float _barWidthFill = 0.9f;

    [SerializeField]
    private Color _barColor = Color.white;

    private List<int> _data = new List<int>();

    private List<GameObject> _bars = new List<GameObject>();

    private int _maxYData = 6;

    private DataStatistics _dataStats = null;


    private void Awake()
    {
        _dataStats = GetComponent<DataStatistics>();

        if (_plotArea == null)
        {
            Debug.LogError("Plot area image object must be assigned");
            return;
        }

        _plotWidth = _plotArea.GetComponent<RectTransform>().sizeDelta.x;
        _plotHeight = _plotArea.GetComponent<RectTransform>().sizeDelta.y;

        CreateEmptyBars();

        // PERHAPS THIS SHOULD BE SOMEWHERE ELSE
        DOTween.SetTweensCapacity(500, 50);
    }


    private void Update()
    {
        foreach (GameObject bar in _bars)
        {
            bar.GetComponent<Image>().color = _barColor;
        }
    }

    private GameObject GetBar(int xVal, int yVal)
    {
        GameObject barGO = new GameObject("Bar", typeof(Image));
        barGO.transform.SetParent(_plotArea.transform, false);
        barGO.GetComponent<Image>().color = _barColor;

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
        Vector2 newSizeDelta = new Vector2(_barWidth, _plotHeight * yVal / _maxYData);
        barRect.DOSizeDelta(newSizeDelta, 0.5f);
    }


    private void CreateEmptyBars()
    {
        _maxYData = 10;
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
        _maxYData = 10;
        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int i = 0;
        foreach (int y in _data)
        {
            GameObject bar = _bars[i];
            UpdateBar(bar, y);
            i++;
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