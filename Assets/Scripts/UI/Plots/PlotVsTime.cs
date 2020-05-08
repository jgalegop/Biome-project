using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlotVsTime : MonoBehaviour
{
    [SerializeField]
    private Image _plotArea = null;
    private RectTransform _plotAreaRect = null;

    private RectTransform _rect = null;

    private float _plotWidth;
    private float _plotHeight;

    [SerializeField]
    private Color _pointColor = Color.white;
    [SerializeField]
    private Color _pointConnectionColor = Color.white;

    [SerializeField]
    private Sprite _pointImage = null;

    [SerializeField]
    private Sprite _tickImage = null;
    [SerializeField]
    private Color _axisColor = Color.white;
    [SerializeField]
    private TMP_FontAsset _tickFont = null;

    private List<GameObject> _dataPointsGO = new List<GameObject>();
    private Dictionary<GameObject, DataPoint> _dataPoints = new Dictionary<GameObject, DataPoint>();

    private List<GameObject> _pointConnections = new List<GameObject>();

    private List<GameObject> _yTicks = new List<GameObject>();

    private int _maxYData;

    private float _maxTime = 120f;
    private float _maxPopulation = 59f;

    public int YAxisPrecision { get; private set; }
    public float XAxisPrecision { get; private set; }

    private float _axisWidth = 5f;

    private List<GameObject> _createdElements = new List<GameObject>();
    private List<GameObject> _elementsParents = new List<GameObject>();

    private const string _pointName = "Point";
    private const string _pointConnectionName = "PointConnection";
    private const string _tickName = "Tick";

    [SerializeField]
    private StatisticsManager _statsManager = null;

    private void Awake()
    {
        StatisticsManager.OnTimeDataSaved += WriteDataPoint;
    }

    private void OnDestroy()
    {
        StatisticsManager.OnTimeDataSaved -= WriteDataPoint;
    }


    private void Start()
    {
        if (_plotArea == null)
        {
            Debug.LogError("Plot area image object must be assigned");
            return;
        }
        _plotAreaRect = _plotArea.GetComponent<RectTransform>();
        _rect = GetComponent<RectTransform>();

        _plotWidth = _rect.sizeDelta.x - Mathf.Abs(_plotAreaRect.offsetMin.x) - Mathf.Abs(_plotAreaRect.offsetMax.x);
        _plotHeight = _rect.sizeDelta.y - Mathf.Abs(_plotAreaRect.offsetMin.y) - Mathf.Abs(_plotAreaRect.offsetMax.y);

        // precisions
        YAxisPrecision = 10;
        XAxisPrecision = 2f;

        CreatePlotElementParents();

        _maxYData = (int)_maxPopulation;
        CreateAxisTicks();

        if (_statsManager.TimeStamps.Count > 1)
        {
            foreach (float time in _statsManager.TimeStamps)
            {
                WriteDataPoint(time);
            }
        }
    }

    private void CreatePlotElementParents()
    {
        var parentNames = new List<string> { _pointName + "s", _pointConnectionName + "s", _tickName + "s"};
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

    private void WriteDataPoint(float time)
    {
        bool dataLimitsReached = false;

        if (time > _maxTime )
        {
            _maxTime = time;
            dataLimitsReached = true;
        }
        if (_statsManager.PopulationInTime[time].Population > _maxPopulation)
        {
            _maxPopulation = _statsManager.PopulationInTime[time].Population + 1;
            _maxYData = (int)_maxPopulation;
            dataLimitsReached = true;
            UpdateYTicks();
        }

        if (dataLimitsReached)
            UpdateAllPoints();

        GameObject pointGO = new GameObject(_pointName, typeof(Image), typeof(Canvas));
        SetElementParent(pointGO);
        //pointGO.transform.SetParent(_plotArea.transform, false);

        Canvas pointCanvas = pointGO.GetComponent<Canvas>();
        pointCanvas.overrideSorting = true;
        pointCanvas.sortingOrder = 2;

        Image pointImg = pointGO.GetComponent<Image>();
        pointImg.sprite = _pointImage;
        pointImg.color = _pointColor;
        pointImg.type = Image.Type.Sliced;
        pointImg.fillCenter = true;
        pointImg.pixelsPerUnitMultiplier = 0f;

        RectTransform pointRect = pointGO.GetComponent<RectTransform>();
        Vector2 point = new Vector2(_statsManager.PopulationInTime[time].Time, _statsManager.PopulationInTime[time].Population);
        pointRect.sizeDelta = new Vector2(5, 5);
        pointRect.anchoredPosition = new Vector2(point.x / _maxTime * _plotWidth - 0.5f * _axisWidth, point.y / _maxPopulation * _plotHeight - 0.5f * _axisWidth);
        pointRect.anchorMin = Vector2.zero;
        pointRect.anchorMax = Vector2.zero;
        pointRect.SetAsLastSibling();

        _dataPointsGO.Add(pointGO);
        _dataPoints.Add(pointGO, _statsManager.PopulationInTime[time]);

        if (_dataPointsGO.Count > 1)
        {
            DataPoint dataPointA = _dataPoints[_dataPointsGO[_dataPointsGO.Count - 2]];
            DataPoint dataPointB = _dataPoints[_dataPointsGO[_dataPointsGO.Count - 1]];
            Vector2 posA = new Vector2(dataPointA.Time / _maxTime * _plotWidth - 0.5f * _axisWidth, dataPointA.Population / _maxPopulation * _plotHeight - 0.5f * _axisWidth);
            Vector2 posB = new Vector2(dataPointB.Time / _maxTime * _plotWidth - 0.5f * _axisWidth, dataPointB.Population / _maxPopulation * _plotHeight - 0.5f * _axisWidth);
            CreateDotConnection(posA, posB);
        }
    }

    private void SetElementParent(GameObject element)
    {
        GameObject parent = _elementsParents.Find((x) => x.name == element.name + "s");
        element.transform.SetParent(parent.transform, false);
    }

    private void UpdateAllPoints()
    {
        Vector2 previousPos = Vector2.zero;

        for (int i = 0; i < _dataPointsGO.Count; i++)
        {
            GameObject pointGo = _dataPointsGO[i];
            DataPoint dataPoint = _dataPoints[pointGo];
            Vector2 currentPos = new Vector2(dataPoint.Time / _maxTime * _plotWidth - 0.5f * _axisWidth, dataPoint.Population / _maxPopulation * _plotHeight - 0.5f * _axisWidth);
            pointGo.GetComponent<RectTransform>().anchoredPosition = currentPos; // axis correction
                                    
            if (i > 0)
            {
                int j = i - 1;
                UpdateDotConnection(_pointConnections[j], previousPos, currentPos);
            }

            previousPos = currentPos;
        }
    }

    private void CreateDotConnection(Vector2 posA, Vector2 posB)
    {
        float distance = Vector2.Distance(posA, posB);
        Vector2 dir = (posB - posA).normalized;

        GameObject pointConnectionGO = new GameObject(_pointConnectionName, typeof(Image), typeof(Canvas));
        SetElementParent(pointConnectionGO);
        //pointConnectionGO.transform.SetParent(_plotArea.transform, false);

        Canvas pointConnectionCanvas = pointConnectionGO.GetComponent<Canvas>();
        pointConnectionCanvas.overrideSorting = true;
        pointConnectionCanvas.sortingOrder = 1;

        Image connectionImage = pointConnectionGO.GetComponent<Image>();
        connectionImage.color = _pointConnectionColor;
        if (connectionImage != null)
        {
            connectionImage.sprite = _pointImage;
            connectionImage.type = Image.Type.Sliced;
            connectionImage.fillCenter = true;
            connectionImage.pixelsPerUnitMultiplier = 30f;
        }

        RectTransform connectionRect = pointConnectionGO.GetComponent<RectTransform>();
        connectionRect.sizeDelta = new Vector2(distance, 2f);
        connectionRect.anchorMin = Vector2.zero;
        connectionRect.anchorMax = Vector2.zero;
        connectionRect.anchoredPosition = posA + dir * distance * 0.5f;
        connectionRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        _pointConnections.Add(pointConnectionGO);
    }

    private void UpdateDotConnection(GameObject connectionGO, Vector2 posA, Vector2 posB)
    {
        float distance = Vector2.Distance(posA, posB);
        Vector2 dir = (posB - posA).normalized;

        RectTransform connectionRect = connectionGO.GetComponent<RectTransform>();
        connectionRect.sizeDelta = new Vector2(distance, 2f);
        connectionRect.anchoredPosition = posA + dir * distance * 0.5f;
        connectionRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void CreateAxisTicks()
    {
        for (int y = 0; y <= _maxYData / YAxisPrecision; y++)
        {
            _yTicks.Add(CreateTick(true, _plotHeight * (float)(y * YAxisPrecision) / (float)_maxYData, (float)(y * YAxisPrecision)));
        }
    }

    private GameObject CreateTick(bool isVerticalAxis, float axisVal, float val)
    {
        GameObject tickGO = new GameObject(_tickName, typeof(Image), typeof(Canvas));
        //tickGO.transform.SetParent(_plotArea.transform, false);
        SetElementParent(tickGO);

        Canvas tickCanvas = tickGO.GetComponent<Canvas>();
        tickCanvas.overrideSorting = true;
        tickCanvas.sortingOrder = 10;

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
            tickRect.sizeDelta = new Vector2(15f, _axisWidth);
            tickRect.pivot = new Vector2(0, 0f);
            tickRect.anchoredPosition = new Vector2(-_axisWidth, axisVal - _axisWidth); // substract axis width
        }
        else
        {
            tickRect.sizeDelta = new Vector2(_axisWidth, 15f);
            tickRect.pivot = new Vector2(0, 0);
            tickRect.anchoredPosition = new Vector2(axisVal - _axisWidth, -_axisWidth);
        }
        tickRect.anchorMin = Vector2.zero;
        tickRect.anchorMax = Vector2.zero;

        tickRect.SetAsFirstSibling();

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
            tick.GetComponent<RectTransform>().anchoredPosition = new Vector2(-_axisWidth, value - _axisWidth);

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