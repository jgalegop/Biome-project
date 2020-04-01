using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using System.Linq;

public class DataStatistics : MonoBehaviour
{
    public int NumberOfBars = 15;

    [SerializeField]
    private float _minMoveSpeed = 1f;
    [SerializeField]
    private float _maxMoveSpeed = 6f;


    [SerializeField]
    private List<Animal> _animalList = new List<Animal>();
    [SerializeField]
    private List<float> _moveSpeedsList = new List<float>();

    [SerializeField]
    private Dictionary<float, int> _animalNumberWithMoveSpeed = new Dictionary<float, int>();

    [SerializeField]
    private float[] _moveSpeedPoints;

    [SerializeField]
    private int[] _debugAnimalNumbers;

    private PlotCanvas _plotCanvas;

    private void Awake()
    {
        _plotCanvas = GetComponent<PlotCanvas>();

        _moveSpeedPoints = new float[NumberOfBars];
        _debugAnimalNumbers = new int[NumberOfBars];
        SetMoveSpeedPoints();
        SetAnimalNumberDictionary();

        StatisticsManager.OnAnimalNumberIncreased += AnimalIsBorn;
        StatisticsManager.OnAnimalNumberDecreased += AnimalHasDied;
    }

    private void AnimalIsBorn(Animal animal)
    {
        _animalList.Add(animal);
        float ms = animal.GetAdultMoveSpeed();
        _moveSpeedsList.Add(ms);

        float msPoint = GetClosestMoveSpeedPoint(ms);
        _animalNumberWithMoveSpeed[msPoint]++;

        for (int i = 0; i < NumberOfBars; i++)
        {
            _debugAnimalNumbers[i] = _animalNumberWithMoveSpeed[_moveSpeedPoints[i]];
        }

        _plotCanvas.UpdateData();
    }

    private void AnimalHasDied(Animal animal)
    {
        _animalList.Remove(animal);
        float ms = animal.GetAdultMoveSpeed();
        _moveSpeedsList.Remove(ms);

        float msPoint = GetClosestMoveSpeedPoint(ms);
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
        return 100;
    }

    public List<int> GetData()
    {
        return _animalNumberWithMoveSpeed.Values.ToList();
    }
}
