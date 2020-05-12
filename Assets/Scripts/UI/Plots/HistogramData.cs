using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HistogramData : MonoBehaviour
{
    [SerializeField]
    private StatisticsManager _statisticsManager = null;

    public int NumberOfBars = 15;

    [SerializeField]
    private int _defaultMaxYData = 12;

    public int MaxYData { get; private set; }

    [SerializeField]
    private float _minMoveSpeed = 1f;
    [SerializeField]
    private float _maxMoveSpeed = 6f;

    public float MinXAxis { get; private set; }
    public float MaxXAxis { get; private set; }

    [SerializeField]
    private List<float> _moveSpeedsList = new List<float>();

    [SerializeField]
    private Dictionary<float, int> _animalNumberWithMoveSpeed = new Dictionary<float, int>();

    [SerializeField]
    private float[] _moveSpeedPoints;

    [SerializeField]
    private int[] _debugAnimalNumbers;

    private PlotHistogram _plotCanvas;

    public bool ChangeXAxis { get; private set; }

    private void Awake()
    {
        StatisticsManager.OnAnimalNumberIncreased += AnimalIsBorn;
        StatisticsManager.OnAnimalNumberDecreased += AnimalHasDied;

        _moveSpeedPoints = new float[NumberOfBars];
        _debugAnimalNumbers = new int[NumberOfBars];
        SetMoveSpeedPoints();
        SetAnimalNumberDictionary();

        MaxYData = _defaultMaxYData;
        ChangeXAxis = false;


        // POSSIBLY CHANGE THIS
        MinXAxis = _minMoveSpeed;
        MaxXAxis = _maxMoveSpeed;
    }

    private void Start()
    {
        _plotCanvas = GetComponent<PlotHistogram>();

        if (_statisticsManager.RabbitData.Count > 0)
        {
            foreach (AnimalData ad in _statisticsManager.RabbitData)
            {
                float msPoint = GetClosestMoveSpeedPoint(ad.speed);
                _animalNumberWithMoveSpeed[msPoint]++;
            }
            _plotCanvas.UpdateData();
        }
    }

    private void OnDestroy()
    {
        StatisticsManager.OnAnimalNumberIncreased -= AnimalIsBorn;
        StatisticsManager.OnAnimalNumberDecreased -= AnimalHasDied;
    }


    private void AnimalIsBorn(AnimalData animalData)
    {
        if (animalData.speed > _maxMoveSpeed)
            IncreaseMaxSpeedAndReset();

        _moveSpeedsList.Add(animalData.speed);

        float msPoint = GetClosestMoveSpeedPoint(animalData.speed);
        _animalNumberWithMoveSpeed[msPoint]++;

        for (int i = 0; i < NumberOfBars; i++)
        {
            _debugAnimalNumbers[i] = _animalNumberWithMoveSpeed[_moveSpeedPoints[i]];
        }

        _plotCanvas.UpdateData();
    }

    private void AnimalHasDied(AnimalData animalData)
    {
        _moveSpeedsList.Remove(animalData.speed);

        float msPoint = GetClosestMoveSpeedPoint(animalData.speed);
        _animalNumberWithMoveSpeed[msPoint]--;

        for (int i = 0; i < NumberOfBars; i++)
        {
            _debugAnimalNumbers[i] = _animalNumberWithMoveSpeed[_moveSpeedPoints[i]];
        }

        _plotCanvas.UpdateData();
    }

    private void SetAnimalNumberDictionary()
    {
        foreach(float ms in _moveSpeedPoints)
        {
            _animalNumberWithMoveSpeed[ms] = 0;
        }
    }

    private void SetMoveSpeedPoints()
    {
        for (int i = 0; i < NumberOfBars; i++)
        {
            _moveSpeedPoints[i] = _minMoveSpeed + i * (_maxMoveSpeed - _minMoveSpeed) / NumberOfBars;
        }
    }


    private float GetClosestMoveSpeedPoint(float moveSpeed)
    {
        float difference = 100;
        for (int i = 0; i < _moveSpeedPoints.Length; i++)
        {
            float msp = _moveSpeedPoints[i];
            if (Mathf.Abs(moveSpeed - msp) < difference)
            {
                difference = Mathf.Abs(moveSpeed - msp);
            }
            else
            {
                return _moveSpeedPoints[i - 1];
            }
        }

        Debug.Log("MoveSpeed " + moveSpeed + " outside of range");
        return _moveSpeedPoints[_moveSpeedPoints.Length - 1];
    }

    public List<int> GetData()
    {
        return _animalNumberWithMoveSpeed.Values.ToList();
    }

    public bool RenormalizeData()
    {
        List<int> data = GetData();

        bool renormalized = false;
        int maxValue = 0;
        foreach (int d in data)
        {
            if (d > maxValue)
                maxValue = d;
        }

        if (maxValue > MaxYData)
        {
            MaxYData = maxValue;
            renormalized = true;
        }
        else if (maxValue < _defaultMaxYData && MaxYData != _defaultMaxYData)
        {
            MaxYData = _defaultMaxYData;
            renormalized = true;
        }
        else if (maxValue < 0.5f * (float)MaxYData && maxValue > _defaultMaxYData)
        {
            MaxYData = (int)(0.5f * (float)MaxYData);
            renormalized = true;
        }

        return renormalized;
    }

    private void IncreaseMaxSpeedAndReset()
    {
        _maxMoveSpeed *= 1.2f;

        MinXAxis = _minMoveSpeed;
        MaxXAxis = _maxMoveSpeed;

        SetMoveSpeedPoints();
        _animalNumberWithMoveSpeed = new Dictionary<float, int>();
        SetAnimalNumberDictionary();

        foreach (AnimalData ad in _statisticsManager.RabbitData)
        {
            float msPoint = GetClosestMoveSpeedPoint(ad.speed);
            _animalNumberWithMoveSpeed[msPoint]++;
        }
        ChangeXAxis = true;
        _plotCanvas.UpdateData();
    }

    private void ChangeMaxSenseRadius(float newMaxSenseRadius)
    {

    }
}