using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour
{
    public static event Action<Animal> OnAnimalNumberIncreased = delegate { };
    public static event Action<Animal> OnAnimalNumberDecreased = delegate { };

    private static StatisticsManager instance;

    private int _rabbitNumber = 0;
    private float _initialTime;

    public float DataTimeInterval = 10f;

    public Dictionary<float, DataPoint> PopulationInTime = new Dictionary<float, DataPoint>();

    public static event Action<float> OnTimeDataSaved = delegate { };

    private void Awake()
    {
        instance = this;
        _initialTime = Time.time;
    }

    private void Start()
    {
        StartCoroutine(GatherData());
    }

    private IEnumerator GatherData()
    {
        while (true)
        {
            float time = Time.time - _initialTime;
            PopulationInTime.Add(time, new DataPoint(time, GetRabbitNumber()));
            OnTimeDataSaved?.Invoke(time);
            yield return new WaitForSeconds(DataTimeInterval);
        }
    }

    public static void AnimalIsBorn(Animal animal)
    {
        OnAnimalNumberIncreased?.Invoke(animal);
        instance._rabbitNumber++;
    }

    public static void AnimalHasDied(Animal animal)
    {
        OnAnimalNumberDecreased?.Invoke(animal);
        instance._rabbitNumber--;
    }

    public static int GetRabbitNumber()
    {
        return instance._rabbitNumber;
    }
}


public struct DataPoint
{
    public float Time;
    public int Population;

    public DataPoint(float time, int pop)
    {
        Time = time;
        Population = pop;
    }
}