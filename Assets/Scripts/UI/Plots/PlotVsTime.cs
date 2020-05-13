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

    private float _maxTime = 120f;
    private float _maxYData;

    private float _maxPopulation = 29f;
    private float _maxSpeed = 7f;
    private float _maxSenseRadius = 20f;

    public float YAxisPrecision { get; private set; }
    public float XAxisPrecision { get; private set; }

    private float _axisWidth = 5f;
    private float _lineSize = 4f;

    private List<GameObject> _elementsParents = new List<GameObject>();

    private const string _pointName = "Point";
    private const string _pointConnectionName = "PointConnection";
    private const string _tickName = "Tick";

    [SerializeField]
    private StatisticsManager _statsManager = null;

    [SerializeField]
    private TextMeshProUGUI _plotTitle = null;

    public enum PlotMode { Population, Speed, SenseRadius}
    private PlotMode _currentMode;

    [SerializeField]
    private int _maxNumberOfPoints = 100;

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
        XAxisPrecision = 2f;

        CreatePlotElementParents();

        ChangePlotMode(0);

        CreateAxisTicks();

        if (_statsManager.TimeStamps.Count > 1 && !ReachedMaxPoints())
        {
            foreach (float time in _statsManager.TimeStamps)
            {
                WriteDataPoint(time);
            }
        }
        else if (ReachedMaxPoints())
        {
            float lastTime = _statsManager.TimeStamps[_statsManager.TimeStamps.Count - 1];
            _maxTime = (lastTime > _maxTime) ? lastTime : _maxTime;

            for (int i = 0; i < _maxNumberOfPoints; i++)
            {
                float time = (i + 1) / _maxNumberOfPoints * _maxTime;
                CreatePointAndConnection(time);
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
        if (PlotLimitsReached(time))
            UpdateAllPoints();

        if (ReachedMaxPoints())
            return;

        CreatePointAndConnection(time);
    }

    private void CreatePointAndConnection(float time)
    {
        CreatePoint(time);

        if (_dataPointsGO.Count > 1)
        {
            DataPoint dataPointA = _dataPoints[_dataPointsGO[_dataPointsGO.Count - 2]];
            DataPoint dataPointB = _dataPoints[_dataPointsGO[_dataPointsGO.Count - 1]];

            float yVariableA = 0;
            float yVariableB = 0;
            if (_currentMode == PlotMode.Population)
            {
                yVariableA = dataPointA.population;
                yVariableB = dataPointB.population;
            }
            else if (_currentMode == PlotMode.Speed)
            {
                yVariableA = dataPointA.averageSpeed;
                yVariableB = dataPointB.averageSpeed;
            }
            else if (_currentMode == PlotMode.SenseRadius)
            {
                yVariableA = dataPointA.averageSenseRadius;
                yVariableB = dataPointB.averageSenseRadius;
            }

            Vector2 posA = new Vector2(dataPointA.time / _maxTime * _plotWidth - 0.5f * _axisWidth, yVariableA / _maxYData * _plotHeight - 0.5f * _axisWidth);
            Vector2 posB = new Vector2(dataPointB.time / _maxTime * _plotWidth - 0.5f * _axisWidth, yVariableB / _maxYData * _plotHeight - 0.5f * _axisWidth);
            CreateDotConnection(posA, posB);
        }
    }

    private bool PlotLimitsReached(float time)
    {
        bool dataLimitsReached = false;

        if (time > _maxTime)
        {
            _maxTime = time;
            dataLimitsReached = true;
        }

        // if we are looking at population
        if (_statsManager.PopulationInTime[time].population > _maxPopulation)
        {
            _maxPopulation = _statsManager.PopulationInTime[time].population + 1;
            if (_currentMode == PlotMode.Population)
            {
                _maxYData = _maxPopulation;
                dataLimitsReached = true;
                UpdateYTicks();
            }
        }

        if (_statsManager.PopulationInTime[time].averageSpeed > _maxSpeed)
        {
            _maxSpeed = _statsManager.PopulationInTime[time].averageSpeed * 1.2f;
            _statsManager.SetMaxSpeed(_maxSpeed);
            if (_currentMode == PlotMode.Speed)
            {
                _maxYData = _maxSpeed;
                dataLimitsReached = true;
                UpdateYTicks();
            }
        }

        if (_statsManager.PopulationInTime[time].averageSenseRadius > _maxSenseRadius)
        {
            _maxSenseRadius = _statsManager.PopulationInTime[time].averageSenseRadius * 1.4f;
            if (_currentMode == PlotMode.SenseRadius)
            {
                _maxYData = _maxSenseRadius;
                dataLimitsReached = true;
                UpdateYTicks();
            }
        }

        return dataLimitsReached;
    }

    private void CreatePoint(float time)
    {
        GameObject pointGO = new GameObject(_pointName, typeof(Image), typeof(Canvas));
        SetElementParent(pointGO);

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

        float yVariable = 0;
        if (_currentMode == PlotMode.Population)
            yVariable = (float)_statsManager.PopulationInTime[time].population;
        else if (_currentMode == PlotMode.Speed)
            yVariable = _statsManager.PopulationInTime[time].averageSpeed;
        else if (_currentMode == PlotMode.SenseRadius)
            yVariable = _statsManager.PopulationInTime[time].averageSenseRadius;

        Vector2 point = new Vector2(_statsManager.PopulationInTime[time].time, yVariable);
        pointRect.sizeDelta = new Vector2(_lineSize, _lineSize);
        pointRect.anchoredPosition = new Vector2(point.x / _maxTime * _plotWidth - 0.5f * _axisWidth, point.y / _maxYData * _plotHeight - 0.5f * _axisWidth);
        pointRect.anchorMin = Vector2.zero;
        pointRect.anchorMax = Vector2.zero;
        pointRect.SetAsLastSibling();

        _dataPointsGO.Add(pointGO);
        _dataPoints.Add(pointGO, _statsManager.PopulationInTime[time]);
    }

    private void SetElementParent(GameObject element)
    {
        GameObject parent = _elementsParents.Find((x) => x.name == element.name + "s");
        element.transform.SetParent(parent.transform, false);
    }

    private void UpdateAllPoints()
    {
        Vector2 previousPos = Vector2.zero;

        int j = 0;
        for (int i = 0; i < _dataPointsGO.Count; i++)
        {
            GameObject pointGo = _dataPointsGO[i];
            DataPoint dataPoint = _dataPoints[pointGo];

            Vector2 currentPos;

            if (!ReachedMaxPoints())
            {
                float yVariable = 0;
                if (_currentMode == PlotMode.Population)
                    yVariable = (float)dataPoint.population;
                else if (_currentMode == PlotMode.Speed)
                    yVariable = dataPoint.averageSpeed;
                else if (_currentMode == PlotMode.SenseRadius)
                    yVariable = dataPoint.averageSenseRadius;

                // axis correction
                currentPos = new Vector2(dataPoint.time / _maxTime * _plotWidth - 0.5f * _axisWidth, yVariable / _maxYData * _plotHeight - 0.5f * _axisWidth);
            }
            else
            {
                float time = _statsManager.TimeStamps[j];
                float difference = 100;
                float xVal = _dataPointsGO[i].GetComponent<RectTransform>().anchoredPosition.x;
                float yVariable = 0;

                while (Mathf.Abs(xVal - time / _maxTime * _plotWidth - 0.5f * _axisWidth) < difference)
                {
                    if (_currentMode == PlotMode.Population)
                        yVariable = (float)_statsManager.PopulationInTime[time].population;
                    else if (_currentMode == PlotMode.Speed)
                        yVariable = _statsManager.PopulationInTime[time].averageSpeed;
                    else if (_currentMode == PlotMode.SenseRadius)
                        yVariable = _statsManager.PopulationInTime[time].averageSenseRadius;

                    difference = Mathf.Abs(xVal - time / _maxTime * _plotWidth - 0.5f * _axisWidth);

                    j++;
                    if (j >= _statsManager.TimeStamps.Count)
                        break;
                    else
                        time = _statsManager.TimeStamps[j];
                }

                // axis correction
                currentPos = new Vector2(xVal, yVariable / _maxYData * _plotHeight - 0.5f * _axisWidth);
            }

            pointGo.GetComponent<RectTransform>().anchoredPosition = currentPos;

            if (i > 0)
            {
                int k = i - 1;
                UpdateDotConnection(_pointConnections[k], previousPos, currentPos);
            }

            previousPos = currentPos;
        }
    }

    private bool ReachedMaxPoints()
    {
        return _dataPointsGO.Count >= _maxNumberOfPoints;
    }

    private void CreateDotConnection(Vector2 posA, Vector2 posB)
    {
        float distance = Vector2.Distance(posA, posB);
        Vector2 dir = (posB - posA).normalized;

        GameObject pointConnectionGO = new GameObject(_pointConnectionName, typeof(Image), typeof(Canvas));
        SetElementParent(pointConnectionGO);

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
        connectionRect.sizeDelta = new Vector2(distance, _lineSize);
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
        connectionRect.sizeDelta = new Vector2(distance, _lineSize);
        connectionRect.anchoredPosition = posA + dir * distance * 0.5f;
        connectionRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void CreateAxisTicks()
    {
        for (int y = 0; y <= _maxYData / YAxisPrecision; y++)
        {
            _yTicks.Add(CreateTick(true, _plotHeight * (float)(y * YAxisPrecision) / _maxYData, (float)(y * YAxisPrecision)));
        }
    }

    private GameObject CreateTick(bool isVerticalAxis, float axisVal, float val)
    {
        GameObject tickGO = new GameObject(_tickName, typeof(Image), typeof(Canvas));
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
            float value = _plotHeight * (float)(y * YAxisPrecision) / _maxYData;
            tick.GetComponent<RectTransform>().anchoredPosition = new Vector2(-_axisWidth, value - _axisWidth);

            TextMeshProUGUI tickText = tick.GetComponentInChildren<TextMeshProUGUI>();
            if (tickText != null)
                UpdateTickText(tickText, y * YAxisPrecision);

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
                _yTicks.Add(CreateTick(true, _plotHeight * (float)(yy * YAxisPrecision) / _maxYData, (float)(yy * YAxisPrecision)));
            }
        }
    }

    private void UpdateTickText(TextMeshProUGUI textObject, float value)
    {
        if (value != (int)value)
            textObject.text = value.ToString("0.0");
        else
            textObject.text = value.ToString();
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

        UpdateTickText(textTMPro, val);

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

    public void ChangePlotMode(int mode)
    {
        if (mode == 0)
        {
            _plotTitle.text = "Rabbit population in time";
            _currentMode = PlotMode.Population;
            _maxYData = _maxPopulation;
            YAxisPrecision = 10f;
        }
        else if (mode == 1)
        {
            _plotTitle.text = "Average rabbit speed in time";
            _currentMode = PlotMode.Speed;
            _maxYData = _maxSpeed;
            YAxisPrecision = 2f;
        }
        else if (mode == 2)
        {
            _plotTitle.text = "Average rabbit sense radius in time";
            _currentMode = PlotMode.SenseRadius;
            _maxYData = _maxSenseRadius;
            YAxisPrecision = 5f;
        }
        UpdateYTicks();
        UpdateAllPoints();
    }
}