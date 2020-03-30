using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Color _barColor;

    private List<int> _data = new List<int>() { 1, 2, 3, 5, 4, 2, 2, 1, 0, 2, 4, 5, 1, 3};

    private int _maxYData = 6;

    // Start is called before the first frame update
    void Start()
    {
        if (_plotArea == null)
        {
            Debug.LogError("Plot area image object must be assigned");
            return;
        }

        _plotWidth = _plotArea.GetComponent<RectTransform>().sizeDelta.x;
        _plotHeight = _plotArea.GetComponent<RectTransform>().sizeDelta.y;

        _barWidth = (_plotWidth / _data.Count + 1) * _barWidthFill;

        int x = 0;
        foreach (int y in _data)
        {
            x++;
            GetBar(x, y);
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
        barRect.anchoredPosition = new Vector2(_plotWidth * xVal / (_data.Count + 1), 0);
        barRect.anchorMin = Vector2.zero;
        barRect.anchorMax = Vector2.zero;

        barRect.SetAsFirstSibling();

        return barGO;
    }
}
