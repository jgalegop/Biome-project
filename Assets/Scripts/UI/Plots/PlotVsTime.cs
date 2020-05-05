using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private Sprite _pointImage = null;

    [SerializeField]
    private Sprite _tickImage = null;
    [SerializeField]
    private Color _axisColor = Color.white;
    [SerializeField]
    private TMP_FontAsset _tickFont = null;

    private Dictionary<float, int> _pointsData = new Dictionary<float, int>();
    private List<GameObject> _pointsGO = new List<GameObject>();
    private List<GameObject> _yTicks = new List<GameObject>();

    private int _maxYData;

    private float _maxTime = 120f;
    private float _maxPopulation = 60f;

    public int YAxisPrecision { get; private set; }
    public float XAxisPrecision { get; private set; }

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
    }

    private void WriteDataPoint(float time)
    {
        GameObject pointGO = new GameObject("Point", typeof(Image));
        pointGO.transform.SetParent(_plotArea.transform, false);

        Image pointImg = pointGO.GetComponent<Image>();
        pointImg.sprite = _pointImage;
        pointImg.color = _pointColor;
        pointImg.type = Image.Type.Sliced;
        pointImg.fillCenter = true;
        pointImg.pixelsPerUnitMultiplier = 0f;

        RectTransform pointRect = pointGO.GetComponent<RectTransform>();
        Vector2 point = new Vector2(time, _statsManager.PopulationInTime[time]);
        pointRect.sizeDelta = new Vector2(5, 5);
        pointRect.anchoredPosition = new Vector2(point.x / _maxPopulation * _plotHeight, point.y / _maxTime * _plotWidth);
        pointRect.anchorMin = Vector2.zero;
        pointRect.anchorMax = Vector2.zero;
        pointRect.SetAsFirstSibling();
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